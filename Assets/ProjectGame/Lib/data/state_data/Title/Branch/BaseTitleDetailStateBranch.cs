using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseTitleDetailStateBranch<TState> : BaseDetailStateBranch<TitleStateID, TitleStateManagerData, TState>
        where TState : GameCore.States.BaseTitleState
    {
        public override abstract TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TState state);
    }
}
