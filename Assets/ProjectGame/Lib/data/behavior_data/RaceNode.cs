
using System;

namespace GameCore.Behavior
{
    public class RaceNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        private bool _finished = false;

        public RaceNode(TEnum customNodeID,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            NodeCategory = BehaviorNodeCategory.Composite;
            NodeID = BehaviorNodeID.Race;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            if (_finished) return BehaviorResultStatus.Success;

            foreach (var child in Children)
            {
                CheckResetExecute(child,blackboard);
                var result = child.OnTick(blackboard);
                if (result == BehaviorResultStatus.Success || result == BehaviorResultStatus.Failure)
                {
                    _finished = true;
                    return result;
                }
            }
            return BehaviorResultStatus.InProgress;
        }

        public override void OnReset(TBlackboard blackboard)
        {
            _finished = false;
        }
    }
}
