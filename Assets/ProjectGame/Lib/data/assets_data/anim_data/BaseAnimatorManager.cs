





// ========================================
// GameCore/Animator/AnimatorManager.cs
// ========================================
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameCore.GameAnimator
{
    public abstract class BaseAnimatorManager<TLayerEnum, TStateEnum,TParam> : OriginAnimatorManager
        where TLayerEnum : Enum
        where TStateEnum : Enum 
        where TParam : class,new()
    {
        // ─────────────────────────────────────
        // 内部クラス：レイヤーごとのステート管理
        // ─────────────────────────────────────
        public class BaseTLayer
        {
            protected int index = -1;
            public Dictionary<TStateEnum, string> stateDict = new();

            public int Index => index;
            public IReadOnlyDictionary<TStateEnum, string> States => stateDict;

            internal void SetIndex(int idx) => index = idx;
        }

        // ─────────────────────────────────────
        // フィールド
        // ─────────────────────────────────────
        protected static Dictionary<TLayerEnum, BaseTLayer> animationKey;
        protected Animator animator;
        public readonly TParam param = new();
        private struct PlayRecord
        {
            public TLayerEnum Layer;
            public TStateEnum State;
            public bool Reverse;
            public float StartNormalizedTime;
            public CancellationTokenSource Cts;
            public Action OnFinish;
        }

        private float animation_speed = 1.0f;
        public float SetAnimationSpeed(float set) => animation_speed = set;
        private PlayRecord? current;
        public void CancelUnitasl()
        {
            current?.Cts?.Cancel();
            current?.Cts.Dispose();
        }
        // ─────────────────────────────────────
        // SetUp
        // ─────────────────────────────────────
        public void SetUp(GameObject gameObject)
        {
            if (animator != null)
            {
                return;
            }
            animator = gameObject.GetComponent<Animator>();
            if (animator == null)
            {
                animator = gameObject.GetComponentInChildren<Animator>();
            }
            if (animator == null)
                throw new Exception($"Animator not found on {gameObject.name}");

            KeySetUp();

            int idx = 0;
            foreach (var kvp in animationKey)
                kvp.Value.SetIndex(idx++);
        }

        public void SetUp(Animator value)
        {
            if (animator != null)
            {
                return;
            }
            animator = value;
            KeySetUp();

            int idx = 0;
            foreach (var kvp in animationKey)
                kvp.Value.SetIndex(idx++);
        }

        public abstract void KeySetUp();

        // ─────────────────────────────────────
        // Play
        // ─────────────────────────────────────
        public void PlayAnimation(TStateEnum state, float crossFade = 0.2f, Action onFinish = null, bool reverse = false)
            => PlayAnimationAsync(state, crossFade, onFinish, reverse).Forget();

        public async UniTask PlayAnimationAsync(TStateEnum state, float crossFade = 0.2f,
            Action onFinish = null, bool reverse = false, CancellationTokenSource customCts = null)
        {
            if (!TryGetLayerAndClip(state, out TLayerEnum layer, out string clipName, out int layerIndex))
            {
                Debug.LogError($"[BaseAnimatorManager] AnimationID not registered: {state}");
                onFinish?.Invoke();
                return;
            }
            if (animator != null)
            {
                animator.speed = 1f;
            }

            current?.Cts?.Cancel();
            current?.Cts?.Dispose();

            var cts = customCts != null
                ? CancellationTokenSource.CreateLinkedTokenSource(customCts.Token, animator.gameObject.GetCancellationTokenOnDestroy())
                : CancellationTokenSource.CreateLinkedTokenSource(animator.gameObject.GetCancellationTokenOnDestroy());

            current = new PlayRecord
            {
                Layer = layer,
                State = state,
                Reverse = reverse,
                StartNormalizedTime = reverse ? 1f : 0f,
                Cts = cts,
                OnFinish = onFinish
            };

            if (crossFade > 0f)
                animator.CrossFade(clipName, crossFade, layerIndex, reverse ? 1f : 0f);
            else
                animator.Play(clipName, layerIndex, reverse ? 1f : 0f);


            await WaitAnimationComplete(layerIndex, clipName, reverse, cts.Token);
        }

        private async UniTask WaitAnimationComplete(int layerIndex, string clipName, bool reverse, CancellationToken ct)
        {
            try
            {
                while (!ct.IsCancellationRequested)
                {
                    await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, ct);
                    var info = animator.GetCurrentAnimatorStateInfo(layerIndex);
                    if (info.shortNameHash != Animator.StringToHash(clipName)) break;

                    animator.SetFloat("AnimSpeed", reverse ? animation_speed * -1f : animation_speed * 1f);
                    
                    float norm = info.normalizedTime > 1.0f || info.normalizedTime < 0.0f ? info.normalizedTime % 1f : info.normalizedTime;

                    if ((reverse && norm <= 0f) || (!reverse && norm >= 1f)) break;
                }

                // 逆再生が終わったなら、速度を 0 にしてその場に留める
                if (reverse)
                {
                    animator.SetFloat("AnimSpeed", 0f);
                }
                else
                {
                    animator.SetFloat("AnimSpeed", animation_speed); // 正転時は通常通り（または0にするかはお好みで）
                }

                var callback = current?.OnFinish;
                current = null;
                callback?.Invoke();
            }
            catch (OperationCanceledException) { }

        }

        // ─────────────────────────────────────
        // 状態取得
        // ─────────────────────────────────────
        public TStateEnum GetCurrentState(TLayerEnum layer)
        {
            if (!animationKey.TryGetValue(layer, out var layerData)) return default;
            if (layerData.Index < 0) return default;

            var info = animator.GetCurrentAnimatorStateInfo(layerData.Index);
            foreach (var kvp in layerData.States)
                if (Animator.StringToHash(kvp.Value) == info.shortNameHash)
                    return kvp.Key;
            return default;
        }

        public bool IsPlaying(TLayerEnum layer)
        {
            if (!animationKey.TryGetValue(layer, out var layerData)) return false;
            if (layerData.Index < 0) return false;

            var info = animator.GetCurrentAnimatorStateInfo(layerData.Index);
            float norm = info.normalizedTime % 1f;
            if (current?.Reverse == true) norm = 1f - norm;
            return norm < 1f;
        }

        public void Stop()
        {
            current?.Cts?.Cancel();
            current?.Cts?.Dispose();
            animator.speed = 1f;
        }

        // ─────────────────────────────────────
        // 内部検索
        // ─────────────────────────────────────
        private bool TryGetLayerAndClip(TStateEnum state, out TLayerEnum layer, out string clipName, out int layerIndex)
        {
            layer = default; clipName = null; layerIndex = -1;

            foreach (var kvp in animationKey)
            {
                if (kvp.Value.States.TryGetValue(state, out clipName))
                {
                    layer = kvp.Key;
                    layerIndex = kvp.Value.Index;
                    return true;
                }
            }
            return false;
        }
    }
}


        

        
        

        