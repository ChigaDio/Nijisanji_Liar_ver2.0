

using System;
using System.Collections.Generic;

namespace GameCore.Behavior
{
    public abstract class BaseBehaviorNode<TBlackboard, TEnum> : OriginBehaviorNode
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public BehaviorNodeCategory NodeCategory { get; protected set; }

        public BehaviorNodeID NodeID { get; protected set; }
        public TEnum CustomNodeID { get; protected set; }

        public BehaviorResetTypeID ResetTypeID { get; protected set; }

        protected List<BaseBehaviorNode<TBlackboard, TEnum>> children = new();

        public IReadOnlyList<BaseBehaviorNode<TBlackboard, TEnum>> Children => children;

        public BaseBehaviorNode(TEnum valueCustomNodeID,BehaviorResetTypeID resetType)
        {
            CustomNodeID = valueCustomNodeID;
            ResetTypeID = resetType;
        }

        public abstract void OnInit(TBlackboard blackboard);

        public abstract BehaviorResultStatus OnTick(TBlackboard blackboard);

        public abstract void OnReset(TBlackboard blackboard);

        public void CheckResetExecute(BaseBehaviorNode<TBlackboard, TEnum> child,TBlackboard blackboard)
        {
            if (child == null) return;
            if (child.ResetTypeID == BehaviorResetTypeID.None) return;
            
            blackboard.XORFlag(child.CustomNodeID,true);
            var check = blackboard.IsFlagSet(child.CustomNodeID);
            if (check == false) return;
            

            if (child.ResetTypeID == BehaviorResetTypeID.THIS_RESET)
            {
                this.OnReset(blackboard);
            }
            else if (child.ResetTypeID == BehaviorResetTypeID.THIS_CHILD_RESET_ALL)
            {
                this.OnAllReset(blackboard);
            }
            else if (child.ResetTypeID == BehaviorResetTypeID.CHILD_FIRST_RESET)
            {
                child.OnReset(blackboard);
            }
            else if(child.ResetTypeID == BehaviorResetTypeID.CHILD_FIRST_RESET)
            {
                child.OnAllReset(blackboard);
            }

            
        }
        public void OnAllReset(TBlackboard blackboard)
        {
            OnReset(blackboard);
            foreach (var child in Children)
                child.OnAllReset(blackboard);
        }

        public void AddChild(BaseBehaviorNode<TBlackboard, TEnum> child)
        {
            if (child != null)
                children.Add(child);
        }

        public void AddChildren(List<BaseBehaviorNode<TBlackboard, TEnum>> valueChildren)
        {
            if (valueChildren != null)
                children.AddRange(valueChildren);
        }

        public void SetChildren(List<BaseBehaviorNode<TBlackboard, TEnum>> valueChildren)
        {
            children = valueChildren ?? new();
        }

        public void SetChild(BaseBehaviorNode<TBlackboard, TEnum> child)
        {
            children = child != null ? new List<BaseBehaviorNode<TBlackboard, TEnum>> { child } : new();
        }
    }
}



