
using System;

namespace GameCore.Behavior
{
    public class DelayNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public float Seconds { get; set; } = 1.0f;
        private float _timer = 0f;
        private bool _started = false;

        public DelayNode(TEnum customNodeID, float valueSeconds,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            NodeCategory = BehaviorNodeCategory.Decorator;
            NodeID = BehaviorNodeID.Delay;
            Seconds = valueSeconds;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            if (Children.Count == 0) return BehaviorResultStatus.Failure;

            if (!_started)
            {
                _timer = 0f;
                _started = true;
            }

            _timer += UnityEngine.Time.deltaTime;
            if (_timer < Seconds)
                return BehaviorResultStatus.InProgress;
            CheckResetExecute(Children[0],blackboard);
            var result = Children[0].OnTick(blackboard);
            _started = false;
            return result;
        }

        public override void OnReset(TBlackboard blackboard)
        {
            _timer = 0f;
            _started = false;
        }
    }
}
