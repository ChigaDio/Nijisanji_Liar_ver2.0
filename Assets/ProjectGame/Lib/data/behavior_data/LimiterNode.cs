
using System;

namespace GameCore.Behavior
{
    public class LimiterNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public int Max { get; set; } = 3;
        private int _current = 0;

        public LimiterNode(TEnum customNodeID, int valueMax,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            Max = valueMax;
            NodeCategory = BehaviorNodeCategory.Decorator;
            NodeID = BehaviorNodeID.Limiter;
        }

        public override void OnInit(TBlackboard blackboard)
        {

            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            if (Children.Count == 0 || _current >= Max) return BehaviorResultStatus.Failure;
            _current++;
            CheckResetExecute(Children[0],blackboard);
            return Children[0].OnTick(blackboard);
        }

        public override void OnReset(TBlackboard blackboard)
        {
            _current = 0;
        }
    }
}
