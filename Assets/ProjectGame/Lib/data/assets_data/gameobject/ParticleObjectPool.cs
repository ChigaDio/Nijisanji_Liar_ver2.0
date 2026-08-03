
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using GameCore.Enums;
using GameCore.Gameobject;

namespace GameCore.Gameobject
{
    public sealed partial class ParticleObjectPool : BaseSingleton<ParticleObjectPool>
    {
        private readonly Dictionary<GameObjectID, ParticlePool> pools = new();
        private readonly ConcurrentDictionary<GameObjectID, UniTask<ParticlePool>> creatingPools = new();

        // キャンセルトークン（全タスクをシーン遷移時に即殺）
        private CancellationToken destroyToken;
        private CancellationTokenSource manualCancelSource = new();
        private CancellationToken combinedToken;

        public override void AwakeSingleton()
        {
            base.AwakeSingleton();
            DontDestroyOnLoad(gameObject);

            destroyToken = this.GetCancellationTokenOnDestroy();
            manualCancelSource = new CancellationTokenSource();
            combinedToken = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, manualCancelSource.Token).Token;
        }

        // 全停止＆全キャンセル（シーン遷移時に必ず呼ぶ！）
        public void StopAllAndCancelAllTasks()
        {
            manualCancelSource.Cancel();
            manualCancelSource.Dispose();
            manualCancelSource = new CancellationTokenSource();
            combinedToken = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, manualCancelSource.Token).Token;

            foreach (var pool in pools.Values)
                pool?.StopAllImmediately();

            creatingPools.Clear();
        }

        // 個別ID停止
        public void StopParticle(GameObjectID id)
        {
            if (pools.TryGetValue(id, out var pool))
                pool.StopAllImmediately();
            var a = new MaterialPropertyBlock();

        }

        // 全パーティクル停止
        public void StopAllParticles()
        {
            foreach (var pool in pools.Values)
                pool?.StopAllImmediately();
        }

        public static async UniTask<ParticleHandle> Play(
            GameObjectID id,
            Vector3 position,
            Quaternion rotation = default,
            Transform parent = null,
            float forceDuration = -1f,
            TimedAction[] timedActions = null,
            Action<ParticleHandle> onCompleted = null)
        {
            if (rotation == default) rotation = Quaternion.identity;

            var pool = await Instance.GetOrCreatePool(id);
            if (pool == null) return default;

            await pool.WaitForAvailableAsync(Instance.combinedToken);
            return pool.PlayImmediately(position, rotation, parent, forceDuration, timedActions, onCompleted);
        }

        private async UniTask<ParticlePool> GetOrCreatePool(GameObjectID id)
        {
            if (pools.TryGetValue(id, out var pool))
                return pool;

            var creationTask = creatingPools.GetOrAdd(id, k =>
            {
                var tcs = new UniTaskCompletionSource<ParticlePool>();
                CreatePoolAsync(k, tcs).Forget();
                return tcs.Task;
            });

            return await creationTask;
        }

        private async UniTask CreatePoolAsync(GameObjectID id, UniTaskCompletionSource<ParticlePool> tcs)
        {
            try
            {
                var pool = new ParticlePool(id, combinedToken);
                await pool.InitializeAsync(combinedToken);
                pools[id] = pool;
                creatingPools.TryRemove(id, out _);
                tcs.TrySetResult(pool);
            }
            catch (Exception e)
            {
                creatingPools.TryRemove(id, out _);
                tcs.TrySetException(e);
            }
        }

        private void OnDestroy()
        {
            StopAllAndCancelAllTasks();
            foreach (var pool in pools.Values) pool?.Dispose();
            pools.Clear();
            creatingPools.Clear();
            manualCancelSource?.Cancel();
            manualCancelSource?.Dispose();
        }
    }

    // =============================================================
    // ParticlePool
    // =============================================================
    internal sealed class ParticlePool : IDisposable
    {
        private readonly GameObjectID id;
        private GameObject template;
        private float duration;
        private bool isLoop;
        private readonly List<PooledParticleObject> pool = new();
        private readonly Queue<PooledParticleObject> freeQueue = new();
        private readonly HashSet<PooledParticleObject> activeSet = new();
        private readonly SemaphoreSlim expandSemaphore = new(1, 1);
        private int peakUsage = 0;
        private float lastShrinkTime = 0f;
        private readonly CancellationToken poolToken;

        private const float ShrinkInterval = 30f;
        private const float ShrinkThreshold = 0.6f;
        private const int MinCapacity = 32;

        public ParticlePool(GameObjectID id, CancellationToken token)
        {
            this.id = id;
            this.poolToken = token;
        }

        public async UniTask InitializeAsync(CancellationToken ct)
        {
            template = GameObjectCore.Instance.GetGameObject(GameObjectGroup.Particle, id);
            if (template == null) throw new Exception($"[ParticlePool] Template not found: {id}");

            var ps = template.GetComponentInChildren<ParticleSystem>(true);
            if (ps == null) throw new Exception($"[ParticlePool] No ParticleSystem on {id}");

            var main = ps.main;
            duration = main.duration + main.startLifetime.constantMax + 0.5f;
            isLoop = main.loop;

            await ExpandAsync(32, ct);
        }

        public async UniTask WaitForAvailableAsync(CancellationToken ct)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, poolToken);

            while (true)
            {
                linkedCts.Token.ThrowIfCancellationRequested();

                while (freeQueue.Count > 0 && freeQueue.Peek().isDestroyed)
                    freeQueue.Dequeue();

                if (freeQueue.Count > 0) break;

                await expandSemaphore.WaitAsync(linkedCts.Token);
                try
                {
                    if (freeQueue.Count > 0) break;
                    await ExpandAsync(Mathf.Max(8, activeSet.Count + 8), linkedCts.Token);
                }
                finally
                {
                    expandSemaphore.Release();
                }
            }
        }

        public ParticleHandle PlayImmediately(
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            float forceDuration,
            TimedAction[] timedActions,
            Action<ParticleHandle> onCompleted)
        {
            while (freeQueue.Count > 0 && freeQueue.Peek().isDestroyed)
                freeQueue.Dequeue();

            var obj = freeQueue.Dequeue();
            activeSet.Add(obj);
            obj.isActive = true;
            obj.isDestroyed = false;

            var go = obj.gameObject;
            var ps = obj.particleSystem;

            if (parent != null) go.transform.SetParent(parent, false);
            else go.transform.SetParent(ParticleObjectPool.Instance.transform, false);

            go.transform.SetPositionAndRotation(position, rotation);
            go.SetActive(true);

            ps.Clear(true);
            ps.Play(true);

            float lifetime = forceDuration > 0f ? forceDuration : (isLoop ? -1f : duration);
            var handle = new ParticleHandle(this, obj, lifetime, timedActions, poolToken);
            onCompleted?.Invoke(handle);

            if (activeSet.Count > peakUsage) peakUsage = activeSet.Count;

            return handle;
        }

        internal void TryReturn(PooledParticleObject obj, int generation)
        {
            if (obj == null || obj.generation != generation || !activeSet.Remove(obj)) return;

            obj.particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            obj.gameObject.SetActive(false);
            obj.isActive = false;
            obj.gameObject.transform.SetParent(ParticleObjectPool.Instance.transform, false);
            obj.generation++;
            freeQueue.Enqueue(obj);
            TryScheduleShrink();
        }

        public void StopAllImmediately()
        {
            foreach (var obj in activeSet.ToList())
            {
                if (obj.particleSystem != null)
                {
                    obj.particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                    obj.gameObject.SetActive(false);
                }
                obj.isActive = false;
                TryReturn(obj, obj.generation);
            }
        }

        private async UniTask ExpandAsync(int count, CancellationToken ct)
        {
            var op = UnityEngine.Object.InstantiateAsync(template, count);
            await op.WithCancellation(ct);

            foreach (var go in op.Result)
            {
                go.transform.SetParent(ParticleObjectPool.Instance.transform, false);
                go.SetActive(false);
                var ps = go.GetComponentInChildren<ParticleSystem>(true);
                var pooled = new PooledParticleObject
                {
                    gameObject = go,
                    particleSystem = ps,
                    generation = 0,
                    isActive = false,
                    isDestroyed = false
                };
                pool.Add(pooled);
                freeQueue.Enqueue(pooled);
            }
        }

        private void TryScheduleShrink()
        {
            if (Time.unscaledTime - lastShrinkTime < ShrinkInterval) return;
            lastShrinkTime = Time.unscaledTime;

            if (activeSet.Count <= (int)(peakUsage * ShrinkThreshold) && pool.Count > MinCapacity)
                ShrinkAsync().Forget();
        }

        private async UniTask ShrinkAsync()
        {
            await expandSemaphore.WaitAsync();
            try
            {
                int target = Mathf.Max(MinCapacity, activeSet.Count + 16);
                if (pool.Count <= target) return;

                int toDestroy = pool.Count - target;
                int destroyed = 0;

                for (int i = pool.Count - 1; i >= 0 && destroyed < toDestroy; i--)
                {
                    var obj = pool[i];
                    if (!obj.isActive && !obj.isDestroyed)
                    {
                        obj.isDestroyed = true;
                        if (obj.gameObject) GameObject.Destroy(obj.gameObject);
                        pool.RemoveAt(i);
                        destroyed++;
                    }
                }
            }
            finally
            {
                expandSemaphore.Release();
            }
        }

        public void Dispose()
        {
            StopAllImmediately();
            foreach (var obj in pool)
                if (obj.gameObject) GameObject.Destroy(obj.gameObject);
            pool.Clear();
            freeQueue.Clear();
            activeSet.Clear();
        }
    }

    // =============================================================
    // ParticleHandle（完全安全・TimedActionもキャンセル対応）
    // =============================================================
    public readonly struct ParticleHandle : IDisposable
    {
        private readonly ParticlePool pool;
        private readonly PooledParticleObject pooledObject;
        private readonly int generation;
        private readonly CancellationTokenSource lifetimeCts;

        internal ParticleHandle(ParticlePool pool, PooledParticleObject pooledObject, float lifetime, TimedAction[] timedActions, CancellationToken externalToken)
        {
            this.pool = pool;
            this.pooledObject = pooledObject;
            this.generation = pooledObject.generation;
            this.lifetimeCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);

            var localPool = pool;
            var localObj = pooledObject;
            var localGen = this.generation;
            var localHandle = this;

            // 自動返却
            if (lifetime > 0f)
            {
                UniTask.Delay(TimeSpan.FromSeconds(lifetime), cancellationToken: lifetimeCts.Token)
                    .ContinueWith(() => localPool?.TryReturn(localObj, localGen))
                    .Forget();
            }

            // TimedAction
            if (timedActions != null)
            {
                foreach (var action in timedActions)
                {
                    float delay = action.usePercentage && lifetime > 0f
                        ? lifetime * (action.timeOrPercent / 100f)
                        : action.timeOrPercent;

                    if (delay < 0f) continue;

                    var capturedAction = action.action;
                    UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: lifetimeCts.Token)
                        .ContinueWith(() => capturedAction?.Invoke(localHandle))
                        .Forget();
                }
            }
        }

        public void Stop()
        {
            lifetimeCts.Cancel();
            pool?.TryReturn(pooledObject, generation);
        }

        public void Dispose() => Stop();

        public bool IsValid => pooledObject != null && pooledObject.generation == generation;
        public bool IsPlaying => pooledObject?.particleSystem != null && pooledObject.particleSystem.isPlaying;
        public GameObject GameObject => pooledObject?.gameObject;
        public ParticleSystem ParticleSystem => pooledObject?.particleSystem;
    }

    // =============================================================
    // 内部クラス
    // =============================================================
    internal sealed class PooledParticleObject
    {
        public GameObject gameObject;
        public ParticleSystem particleSystem;
        public int generation = 0;
        public bool isActive = false;
        public bool isDestroyed = false;
    }

    public struct TimedAction
    {
        public float timeOrPercent;
        public bool usePercentage;
        public Action<ParticleHandle> action;

        public TimedAction(float value, bool isPercent, Action<ParticleHandle> action)
        {
            timeOrPercent = value;
            usePercentage = isPercent;
            this.action = action;
        }
    }
}
        