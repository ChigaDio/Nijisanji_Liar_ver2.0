
using System;
using System.Linq;

namespace GameCore.Behavior
{
    public enum ParallelPolicyID
    {
        ALL,
        ANY
    }

    public class ParallelNode<TBlackboard, TEnum> : BaseBehaviorNode<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public ParallelPolicyID SuccessPolicy { get; set; } = ParallelPolicyID.ALL;
        public ParallelPolicyID FailurePolicy { get; set; } = ParallelPolicyID.ANY;

        public ParallelNode(TEnum customNodeID, ParallelPolicyID valueSuccessPolicy, ParallelPolicyID valueFailurePolicy,BehaviorResetTypeID resetType) : base(customNodeID,resetType)
        {
            SuccessPolicy = valueSuccessPolicy;
            FailurePolicy = valueFailurePolicy;
            
            NodeCategory = BehaviorNodeCategory.Composite;
            NodeID = BehaviorNodeID.Parallel;
        }

        public override void OnInit(TBlackboard blackboard)
        {
            OnReset(blackboard);
        }

        public override BehaviorResultStatus OnTick(TBlackboard blackboard)
        {
            var results = Children.Select(c =>
            {
                CheckResetExecute(c,blackboard);
                var result = c.OnTick(blackboard);
                return result;
            }).ToList();

            bool allSuccess = results.All(r => r == BehaviorResultStatus.Success);
            bool anySuccess = results.Any(r => r == BehaviorResultStatus.Success);
            bool allFailure = results.All(r => r == BehaviorResultStatus.Failure);
            bool anyFailure = results.Any(r => r == BehaviorResultStatus.Failure);

            if (SuccessPolicy == ParallelPolicyID.ALL && allSuccess) return BehaviorResultStatus.Success;
            else if (SuccessPolicy == ParallelPolicyID.ANY && anySuccess) return BehaviorResultStatus.Success;
            if (FailurePolicy == ParallelPolicyID.ALL && allFailure) return BehaviorResultStatus.Failure;
            else if (FailurePolicy == ParallelPolicyID.ANY && anyFailure) return BehaviorResultStatus.Failure;

            return BehaviorResultStatus.InProgress;
        }

        public override void OnReset(TBlackboard blackboard)
        {
        }
    }
}
