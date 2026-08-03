using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseMainLoopDetailStateBranch<TState> : BaseDetailStateBranch<MainLoopStateID, MainLoopStateManagerData, TState>
        where TState : GameCore.States.BaseMainLoopState
    {
        public override abstract MainLoopStateID ConditionsBranch(MainLoopStateManagerData manager_data, TState state);
    }
}
