
// File: CooldownNode.cs
using System;

namespace GameCore.Behavior
{
    public class ActionNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {


        public ActionNode(TEnum customNodeID,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            NodeCategory = BehaviorNodeCategory.Leaf;
            NodeID = BehaviorNodeID.Action;   
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {

            return BehaviorResultStatus.Success;
        }

        public override void OnReset(TBlackboard blackboard)
        {
        }
        
    }
}
        