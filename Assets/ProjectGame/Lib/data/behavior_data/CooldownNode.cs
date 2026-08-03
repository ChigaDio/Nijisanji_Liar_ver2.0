
using System;

namespace GameCore.Behavior
{
    public class CooldownNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public float Seconds { get; set; } = 1.0f;
        private float _timer = 0f;
        private bool _inCooldown = false;
        private BehaviorResultStatus _lastResult = BehaviorResultStatus.Success;

        public CooldownNode(TEnum customNodeID, float valueSeconds,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            Seconds = valueSeconds;
            NodeCategory = BehaviorNodeCategory.Decorator;
            NodeID = BehaviorNodeID.Cooldown;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            if (Children.Count == 0) return BehaviorResultStatus.Failure;

            if (_inCooldown)
            {
                _timer += UnityEngine.Time.deltaTime;
                if (_timer >= Seconds)
                {
                    _inCooldown = false;
                    _timer = 0f;
                }
                return _lastResult;
            }
            CheckResetExecute(Children[0],blackboard);
            var result = Children[0].OnTick(blackboard);
            if (result == BehaviorResultStatus.Success || result == BehaviorResultStatus.Failure)
            {
                _inCooldown = true;
                _timer = 0f;
                _lastResult = result;
            }
            return result;
        }

        public override void OnReset(TBlackboard blackboard)
        {
            _timer = 0f;
            _inCooldown = false;
        }
    }
}
