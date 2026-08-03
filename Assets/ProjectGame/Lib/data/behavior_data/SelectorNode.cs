
using System;

namespace GameCore.Behavior
{
    public class SelectorNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public SelectorNode(TEnum customNodeID,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            NodeCategory = BehaviorNodeCategory.Composite;
            NodeID = BehaviorNodeID.Selector;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            foreach (var child in Children)
            {
                CheckResetExecute(child,blackboard);
                var result = child.OnTick(blackboard);
                if (result == BehaviorResultStatus.Success)
                    return BehaviorResultStatus.Success;
            }
            return BehaviorResultStatus.Failure;
        }

        public override void OnReset(TBlackboard blackboard)
        {
        }
    }
}
