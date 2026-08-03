
using System;

namespace GameCore.Behavior
{
    public class ConditionNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public ConditionNode(TEnum customNodeID,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            NodeCategory = BehaviorNodeCategory.Leaf;
            NodeID = BehaviorNodeID.Condition;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            bool result = Compare(blackboard);
            return result ? BehaviorResultStatus.Success : BehaviorResultStatus.Failure;
        }

        public override void OnReset(TBlackboard blackboard) { }

        protected virtual bool Compare(TBlackboard blackboard)
        {
            return false;
        }
    }
}
