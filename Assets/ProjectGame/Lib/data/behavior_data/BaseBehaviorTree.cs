
using System;

namespace GameCore.Behavior
{
    public abstract class BaseBehaviorTree<TBlackboard, TEnum>
        where TBlackboard : BaseBehaviorBlackboard<TBlackboard,TEnum>, new()
        where TEnum : struct, Enum
    {
        public TBlackboard Blackboard { get; private set; } = new();
        public BaseBehaviorNode<TBlackboard, TEnum> Root { get; protected set; }

        public void SetRoot(BaseBehaviorNode<TBlackboard, TEnum> root) => Root = root;

        public abstract void OnInit(Action<TBlackboard> action = null,TBlackboard blackboard = null);
        public abstract BehaviorResultStatus Tick();
        public abstract void OnReset(Action<TBlackboard> action = null);
        public void SetBlackboard(TBlackboard blackboard) => Blackboard = blackboard;
    }
}
