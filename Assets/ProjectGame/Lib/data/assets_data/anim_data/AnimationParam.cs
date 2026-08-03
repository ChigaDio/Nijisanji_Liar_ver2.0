
using System;
using UnityEngine;
namespace GameCore.GameAnimator
{
    public abstract class AnimationParam<TType, TParamEnum>
        where TType : struct
        where TParamEnum : Enum
    {
        protected Animator animator;
        public void SetAnimator(Animator anim) => animator = anim;
    }


    public sealed class FloatParam<TParamEnum> : AnimationParam<float, TParamEnum>
        where TParamEnum : Enum
    {
        public void Set(TParamEnum param, float value) => animator.SetFloat(param.ToString(), value);
        public float Get(TParamEnum param) => animator.GetFloat(param.ToString());
    }

    public sealed class IntParam<TParamEnum> : AnimationParam<int, TParamEnum>
        where TParamEnum : Enum
    {
        public void Set(TParamEnum param, int value) => animator.SetInteger(param.ToString(), value);
        public int Get(TParamEnum param) => animator.GetInteger(param.ToString());
    }

    public sealed class BoolParam<TParamEnum> : AnimationParam<bool, TParamEnum>
        where TParamEnum : Enum
    {
        public void Set(TParamEnum param, bool value) => animator.SetBool(param.ToString(), value);
        public bool Get(TParamEnum param) => animator.GetBool(param.ToString());
    }

    public sealed class TriggerParam<TParamEnum> : AnimationParam<bool, TParamEnum>
        where TParamEnum : Enum
    {
        public void Trigger(TParamEnum param) => animator.SetTrigger(param.ToString());
    }
}
        