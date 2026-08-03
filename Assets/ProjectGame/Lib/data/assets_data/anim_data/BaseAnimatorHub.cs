

using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

namespace GameCore.GameAnimator
{
    public class BaseAnimatorHub<TAnimatorManager, TLayerEnum, TStateEnum,TParam> : OriginAnimatorHub
        where TAnimatorManager : BaseAnimatorManager<TLayerEnum, TStateEnum,TParam>, new()
        where TLayerEnum : struct, Enum
        where TStateEnum : struct, Enum
        where TParam : class,new()
    {
        protected Animator animator;
        protected TAnimatorManager animationManager;

        public override void SetUp()
        {
            animator = GetComponent<Animator>();
            if (animator.Equals(null))
            {
                animator = GetComponentInChildren<Animator>();
            }

            animationManager = new TAnimatorManager();
            animationManager.SetUp(animator);
        }

        public override void ReleaseHub()
        {
            animationManager?.Stop();
        }

        public TParam Param()
        {
            return animationManager.param;
        }

        public void PlayAnimation(TStateEnum state, float crossFade = 0.2f, Action onFinish = null, bool reverse = false)
         => animationManager.PlayAnimation(state, crossFade, onFinish, reverse);

        public async UniTask PlayAnimationAsync(TStateEnum state, float crossFade = 0.2f,
            Action onFinish = null, bool reverse = false, CancellationTokenSource customCts = null)
            => await animationManager.PlayAnimationAsync(state, crossFade, onFinish, reverse);

        public TStateEnum GetCurrentState(TLayerEnum layer) => animationManager.GetCurrentState(layer);

        public bool IsPlaying(TLayerEnum layer) => animationManager.IsPlaying(layer);

        public void Stop() => animationManager.Stop();

        public void SetAnimationSpeed(float speed)
        {
            animationManager.SetAnimationSpeed(speed);
        }
    }


}

        
        

        
        
        

        