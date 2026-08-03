using System;
using UnityEngine;
using GameCore.States.Managers;

using GameCore.States.ID;
namespace GameCore.States.Branch
{
    public abstract class BaseTitleStateBranch<TState, TDetailState> : BaseStateBranch<TitleStateID, TitleStateManagerData, TState, TDetailState>
        where TState : GameCore.States.BaseTitleState
        where TDetailState : BaseTitleDetailStateBranch<TState>
    {
        public override abstract TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TState state);
        public override abstract TDetailState Factory(TitleStateID id);
    }
}
