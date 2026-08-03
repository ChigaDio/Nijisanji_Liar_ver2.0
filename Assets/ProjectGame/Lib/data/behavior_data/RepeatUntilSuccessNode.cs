
using System;

namespace GameCore.Behavior
{
    public class RepeatUntilSuccessNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public RepeatUntilSuccessNode(TEnum customNodeID,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            NodeCategory = BehaviorNodeCategory.Decorator;
            NodeID = BehaviorNodeID.RepeatUntilSuccess;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            if (Children.Count == 0) return BehaviorResultStatus.Failure;
            CheckResetExecute(Children[0],blackboard);
            var result = Children[0].OnTick(blackboard);
            return result == BehaviorResultStatus.Success ? BehaviorResultStatus.Success : BehaviorResultStatus.InProgress;
        }

        public override void OnReset(TBlackboard blackboard)
        {
        }
    }
}
