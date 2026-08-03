
using System;

namespace GameCore.Behavior
{
    public class RepeaterNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public int Count { get; set; } = 3;
        private int _current = 0;

        public RepeaterNode(TEnum customNodeID, int valueCount,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            Count = valueCount;
            NodeCategory = BehaviorNodeCategory.Decorator;
            NodeID = BehaviorNodeID.Repeater;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            if (Children.Count == 0) return BehaviorResultStatus.Failure;
            if (_current >= Count) return BehaviorResultStatus.Success;
            CheckResetExecute(Children[0],blackboard);
            var result = Children[0].OnTick(blackboard);
            if (result == BehaviorResultStatus.Success || result == BehaviorResultStatus.Failure)
                _current++;

            return _current >= Count ? BehaviorResultStatus.Success : BehaviorResultStatus.InProgress;
        }

        public override void OnReset(TBlackboard blackboard)
        {
            _current = 0;
        }
    }
}
