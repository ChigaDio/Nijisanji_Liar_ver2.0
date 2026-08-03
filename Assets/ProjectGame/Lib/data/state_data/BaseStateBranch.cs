using System;
using UnityEngine;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseStateBranch<TStateId, TManagerData, TState, TDetailState>
        where TStateId : Enum
        where TManagerData : BaseStateManagerData<TStateId>
        where TState : BaseState<TStateId, TManagerData>
        where TDetailState : BaseDetailStateBranch<TStateId, TManagerData, TState>
    {
        public abstract TStateId ConditionsBranch(TManagerData manager_data, TState state);
        public abstract TDetailState Factory(TStateId id);
    }
}
