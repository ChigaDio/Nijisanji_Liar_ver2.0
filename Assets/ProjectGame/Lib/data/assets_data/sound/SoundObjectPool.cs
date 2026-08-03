
//===================================================================
// SoundObjectPool.cs 
//==================================================================
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using GameCore.Enums;
using GameCore.Sound;
using GameCore.SaveSystem;

namespace GameCore.Sound
{
    // =============================================================
    // メインクラス：SoundObjectPool
    // =============================================================
    public sealed partial class SoundObjectPool : BaseSingleton<SoundObjectPool>
    {
        private readonly Dictionary<(SoundGroup group, SoundID id), SoundPool> sePools = new();
        private readonly ConcurrentDictionary<(SoundGroup group, SoundID id), UniTask<SoundPool>> creatingPools = new();
        private readonly BGMChannel[] bgmChannels = new BGMChannel[4];

        // キャンセルトークン（全UniTaskをシーン遷移時に即殺）
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

            for (int i = 0; i < bgmChannels.Length; i++)
                bgmChannels[i] = new BGMChannel(i, combinedToken);
        }
        // =============================================================
        // 全停止＆全キャンセル（シーン遷移時に絶対呼ぶ！）
        // =============================================================
        public void StopAllAndCancelAllTasks()
        {
            manualCancelSource.Cancel();
            manualCancelSource.Dispose();
            manualCancelSource = new CancellationTokenSource();
            combinedToken = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, manualCancelSource.Token).Token;

            foreach (var pool in sePools.Values)
                pool?.StopAllImmediately();

            foreach (var channel in bgmChannels)
                channel?.StopImmediately();

           
            creatingPools.Clear();

        }

        // =============================================================
        // 個別SE停止（特定Group+ID or 全SE）
        // =============================================================
        public void StopSE(SoundGroup group, SoundID id)
        {
            var key = (group, id);
            if (sePools.TryGetValue(key, out var pool))
                pool.StopAllImmediately();
        }

        public void StopAllSE()
        {
            foreach (var pool in sePools.Values)
                pool?.StopAllImmediately();
        }

        // =============================================================
        // SE再生
        // =============================================================
        public static async UniTask<SoundHandle> PlaySE(
            SoundGroup group,
            SoundID id,
            Vector3 position,
            float volume = 1f,
            float pitch = 1f,
            float forceDuration = -1f,
            float distance = 0f,
            Action<SoundHandle> onCompleted = null)
        {
            volume = (volume * SoundCore.Instance.GetSoundVolume(group, id)) * SaveManagerCore.instance.SystemSettings.seVolume;
            var pool = await Instance.GetOrCreateSEPool(group, id);
            if (pool == null) return default;

            await pool.WaitForAvailableAsync(Instance.combinedToken);
            return pool.PlayImmediately(position, volume, pitch, forceDuration, distance, onCompleted);
        }

        // =============================================================
        // BGM再生・停止
        // =============================================================
        public static async UniTask PlayBGM(int channel, SoundGroup group, SoundID id, float fadeIn = 1f, float volume = 1f)
        {
            if (channel < 0 || channel >= Instance.bgmChannels.Length) return;
            volume = (volume * SoundCore.Instance.GetSoundVolume(group, id)) * SaveManagerCore.instance.SystemSettings.bgmVolume;
            await Instance.bgmChannels[channel].Play(group, id, fadeIn, volume, Instance.combinedToken);
        }

        public static async UniTask StopBGM(int channel, float fadeOut = 1f, Action onComplete = null)
        {
            if (channel < 0 || channel >= Instance.bgmChannels.Length) return;
            await Instance.bgmChannels[channel].StopAsync(fadeOut, onComplete, Instance.combinedToken);
        }

        public static void StopBGMImmediately(int channel)
        {
            if (channel >= 0 && channel < Instance.bgmChannels.Length)
                Instance.bgmChannels[channel].StopImmediately();
        }

        private async UniTask<SoundPool> GetOrCreateSEPool(SoundGroup group, SoundID id)
        {
            var key = (group, id);
            if (sePools.TryGetValue(key, out var pool))
                return pool;

            var creationTask = creatingPools.GetOrAdd(key, k =>
            {
                var tcs = new UniTaskCompletionSource<SoundPool>();
                CreatePoolAsync(k, tcs).Forget();
                return tcs.Task;
            });

            return await creationTask;
        }

        private async UniTask CreatePoolAsync((SoundGroup group, SoundID id) key, UniTaskCompletionSource<SoundPool> tcs)
        {
            try
            {
                var pool = new SoundPool(key.group, key.id, combinedToken);
                await pool.InitializeAsync(combinedToken);
                sePools[key] = pool;
                creatingPools.TryRemove(key, out _);
                tcs.TrySetResult(pool);
            }
            catch (Exception e)
            {
                creatingPools.TryRemove(key, out _);
                tcs.TrySetException(e);
            }
        }

        private void OnDestroy()
        {
            StopAllAndCancelAllTasks();
            foreach (var pool in sePools.Values) pool?.Dispose();
            foreach (var channel in bgmChannels) channel?.Dispose();
            sePools.Clear();
            creatingPools.Clear();
            manualCancelSource?.Cancel();
            manualCancelSource?.Dispose();
        }
    }

    // =============================================================
    // BGMチャンネル
    // =============================================================
    internal sealed class BGMChannel : IDisposable
    {
        public int ChannelID { get; }
        private AudioSource current;
        private AudioSource next;
        private readonly CancellationToken channelToken;

        public BGMChannel(int id, CancellationToken token)
        {
            ChannelID = id;
            channelToken = token;
        }

        public async UniTask Play(SoundGroup group, SoundID id, float fadeIn, float volume, CancellationToken ct)
        {
            var clip = SoundCore.Instance.GetSoundClip(group, id);
            if (!clip) return;

            var go = new GameObject($"BGM_Channel{ChannelID}");
            go.transform.SetParent(SoundObjectPool.Instance.transform);
            var source = go.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.playOnAwake = false;
            source.volume = 0f;

            if (current != null && fadeIn > 0f)
            {
                next = source;
                await CrossFadeAsync(fadeIn, volume, ct);
            }
            else
            {
                if (current) GameObject.Destroy(current.gameObject);
                current = source;
                current.volume = volume;
                current.Play();
            }
        }

        public async UniTask StopAsync(float fadeOut, Action onComplete, CancellationToken ct)
        {
            if (current == null)
            {
                onComplete?.Invoke();
                return;
            }

            await FadeOutAsync(current, fadeOut, ct);
            if (current) GameObject.Destroy(current.gameObject);
            current = null;
            onComplete?.Invoke();
        }

        public void StopImmediately()
        {
            if (current != null)
            {
                current.Stop();
                GameObject.Destroy(current.gameObject);
                current = null;
            }
            if (next != null)
            {
                next.Stop();
                GameObject.Destroy(next.gameObject);
                next = null;
            }
        }

        private async UniTask CrossFadeAsync(float duration, float targetVolume, CancellationToken ct)
        {
            if (next == null) return;

            next.Play();
            float timer = 0f;
            float startVol = current != null ? current.volume : 0f;

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(timer / duration);
                if (current) current.volume = Mathf.Lerp(startVol, 0f, t);
                next.volume = Mathf.Lerp(0f, targetVolume, t);
                await UniTask.Yield(ct);
            }

            if (current) GameObject.Destroy(current.gameObject);
            current = next;
            next = null;
        }

        private async UniTask FadeOutAsync(AudioSource source, float duration, CancellationToken ct)
        {
            if (source == null) return;

            float startVol = source.volume;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVol, 0f, timer / duration);
                await UniTask.Yield(ct);
            }

            source.volume = 0f;
            source.Stop();
        }

        public void Dispose()
        {
            StopImmediately();
        }
    }

    // =============================================================
    // SEプール
    // =============================================================
    internal sealed class SoundPool : IDisposable
    {
        private readonly SoundGroup group;
        private readonly SoundID id;
        private AudioClip clip;
        private readonly List<PooledAudioObject> pool = new();
        private readonly Queue<PooledAudioObject> freeQueue = new();
        private readonly HashSet<PooledAudioObject> activeSet = new();
        private readonly SemaphoreSlim expandSemaphore = new(1, 1);
        private int peakUsage = 0;
        private float lastShrinkTime = 0f;
        private readonly CancellationToken poolToken;

        private const float ShrinkInterval = 30f;
        private const float ShrinkThreshold = 0.6f;
        private const int MinCapacity = 32;

        public SoundPool(SoundGroup group, SoundID id, CancellationToken token)
        {
            this.group = group;
            this.id = id;
            this.poolToken = token;
        }

        public async UniTask InitializeAsync(CancellationToken ct = default)
        {
            clip = SoundCore.Instance.GetSoundClip(group, id);
            if (!clip) throw new Exception($"[SoundPool] Clip not found: {group}/{id}");
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

        public SoundHandle PlayImmediately(Vector3 position, float volume, float pitch, float forceDuration, float distance, Action<SoundHandle> onCompleted)
        {
            while (freeQueue.Count > 0 && freeQueue.Peek().isDestroyed)
                freeQueue.Dequeue();

            var obj = freeQueue.Dequeue();
            activeSet.Add(obj);
            obj.isActive = true;
            obj.isDestroyed = false;

            var go = obj.gameObject;
            var source = obj.source;
            go.transform.position = position;
            source.volume = volume;
            source.pitch = pitch;
            source.spatialBlend = distance > 0f ? 1f : 0f;
            source.minDistance = 0.5f;
            source.maxDistance = distance;
            go.SetActive(true);
            source.Play();

            float lifetime = forceDuration > 0f ? forceDuration : clip.length / Mathf.Abs(pitch);
            var handle = new SoundHandle(this, obj, lifetime);
            onCompleted?.Invoke(handle);
            return handle;
        }

        internal void TryReturn(PooledAudioObject obj, int generation)
        {
            if (obj == null || obj.generation != generation || !activeSet.Remove(obj)) return;

            obj.source.Stop();
            obj.source.volume = 1f;
            obj.source.pitch = 1f;
            obj.gameObject.SetActive(false);
            obj.isActive = false;
            obj.gameObject.transform.SetParent(SoundObjectPool.Instance.transform);
            obj.generation++;
            freeQueue.Enqueue(obj);
            TryScheduleShrink();
        }

        public void StopAllImmediately()
        {
            foreach (var obj in activeSet)
            {
                if (obj.source != null && obj.source.isPlaying)
                {
                    obj.source.Stop();
                    obj.gameObject.SetActive(false);
                }
                obj.isActive = false;
            }

            while (activeSet.Count > 0)
            {
                var obj = activeSet.FirstOrDefault();
                if (obj != null)
                {
                    activeSet.Remove(obj);
                    if (!obj.isDestroyed) freeQueue.Enqueue(obj);
                }
            }
        }

        private async UniTask ExpandAsync(int count, CancellationToken ct)
        {
            var template = new GameObject($"PooledSE_Temp:{clip.name}");
            template.transform.SetParent(SoundObjectPool.Instance.transform);
            var op = UnityEngine.Object.InstantiateAsync(template, count);
            await op.WithCancellation(ct);
            GameObject.Destroy(template);

            foreach (var go in op.Result)
            {
                go.transform.SetParent(SoundObjectPool.Instance.transform);
                go.SetActive(false);
                var source = go.AddComponent<AudioSource>();
                source.playOnAwake = false;
                source.clip = clip;
                var pooled = new PooledAudioObject
                {
                    gameObject = go,
                    source = source,
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
    // SoundHandle（ゾンビ化防止）
    // =============================================================
    public readonly struct SoundHandle : IDisposable
    {
        private readonly SoundPool pool;
        private readonly PooledAudioObject pooledObject;
        private readonly int generation;

        internal SoundHandle(SoundPool pool, PooledAudioObject pooledObject, float lifetime)
        {
            this.pool = pool;
            this.pooledObject = pooledObject;
            this.generation = pooledObject.generation;
            var localGeneration = this.generation;
            if (lifetime > 0f)
            {
                UniTask.Delay(TimeSpan.FromSeconds(lifetime))
                    .ContinueWith(() => pool?.TryReturn(pooledObject, localGeneration))
                    .Forget();
            }
        }

        public void Stop() => pool?.TryReturn(pooledObject, generation);
        public void Dispose() => Stop();

        public bool IsValid => pooledObject != null && pooledObject.generation == generation;
        public bool IsPlaying => pooledObject?.source != null && pooledObject.source.isPlaying;
        public float Volume { get => pooledObject?.source?.volume ?? 0f; set { if (pooledObject?.source) pooledObject.source.volume = value; } }
        public float Pitch { get => pooledObject?.source?.pitch ?? 1f; set { if (pooledObject?.source) pooledObject.source.pitch = value; } }
    }

    // =============================================================
    // プールオブジェクト
    // =============================================================
    internal sealed class PooledAudioObject
    {
        public GameObject gameObject;
        public AudioSource source;
        public int generation = 0;
        public bool isActive = false;
        public bool isDestroyed = false;
    }
}
