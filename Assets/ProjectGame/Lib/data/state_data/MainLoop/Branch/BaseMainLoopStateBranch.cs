using System;
using UnityEngine;
using GameCore.States.Managers;

using GameCore.States.ID;
namespace GameCore.States.Branch
{
    public abstract class BaseMainLoopStateBranch<TState, TDetailState> : BaseStateBranch<MainLoopStateID, MainLoopStateManagerData, TState, TDetailState>
        where TState : GameCore.States.BaseMainLoopState
        where TDetailState : BaseMainLoopDetailStateBranch<TState>
    {
        public override abstract MainLoopStateID ConditionsBranch(MainLoopStateManagerData manager_data, TState state);
        public override abstract TDetailState Factory(MainLoopStateID id);
    }
}
