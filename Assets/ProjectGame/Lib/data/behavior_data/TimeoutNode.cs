
using System;

namespace GameCore.Behavior
{
    public class TimeoutNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public float Seconds { get; set; } = 5.0f;
        private float _timer = 0f;
        private bool _started = false;

        public TimeoutNode(TEnum customNodeID, float valueSeconds,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            Seconds = valueSeconds;
            NodeCategory = BehaviorNodeCategory.Decorator;
            NodeID = BehaviorNodeID.Timeout;
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
            CheckResetExecute(Children[0],blackboard);
            var result = Children[0].OnTick(blackboard);
            if (result != BehaviorResultStatus.InProgress)
            {
                _started = false;
                return result;
            }

            if (_timer >= Seconds)
            {
                _started = false;
                return BehaviorResultStatus.Timeout;
            }

            return BehaviorResultStatus.InProgress;
        }

        public override void OnReset(TBlackboard blackboard)
        {
            _timer = 0f;
            _started = false;;
        }
    }
}
