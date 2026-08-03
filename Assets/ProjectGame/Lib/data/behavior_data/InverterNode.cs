
using System;

namespace GameCore.Behavior
{
    public class InverterNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public InverterNode(TEnum customNodeID,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            NodeCategory = BehaviorNodeCategory.Decorator;
            NodeID = BehaviorNodeID.Inverter; 
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
            return result == BehaviorResultStatus.Success ? BehaviorResultStatus.Failure :
                   result == BehaviorResultStatus.Failure ? BehaviorResultStatus.Success : result;
        }

        public override void OnReset(TBlackboard blackboard)
        {
        }
    }
}
