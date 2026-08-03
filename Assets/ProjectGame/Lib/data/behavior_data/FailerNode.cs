
using System;

namespace GameCore.Behavior
{
    public class FailerNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public FailerNode(TEnum customNodeID,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            NodeCategory = BehaviorNodeCategory.Decorator;
            NodeID = BehaviorNodeID.Failer;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            if (Children.Count > 0)
            {
                CheckResetExecute(Children[0],blackboard);
                Children[0].OnTick(blackboard);
            }
            return BehaviorResultStatus.Failure;
        }

        public override void OnReset(TBlackboard blackboard)
        {
        }
    }
}
