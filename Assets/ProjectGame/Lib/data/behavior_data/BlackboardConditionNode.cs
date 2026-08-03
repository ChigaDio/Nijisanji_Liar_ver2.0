
using System;

namespace GameCore.Behavior
{
    public class BlackboardConditionNode<TBlackboard, TEnum> : ConditionNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public BlackboardConditionNode(TEnum customNodeID,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            NodeCategory = BehaviorNodeCategory.Leaf;
            NodeID = BehaviorNodeID.BlackboardCondition;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }
    }
}
