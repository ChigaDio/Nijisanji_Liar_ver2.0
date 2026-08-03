using System;
using UnityEngine;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseDetailStateBranch<TStateId, TManagerData, TState>
        where TStateId : Enum
        where TManagerData : BaseStateManagerData<TStateId>
        where TState : BaseState<TStateId, TManagerData>
    {
        public abstract TStateId ConditionsBranch(TManagerData manager_data, TState state);
    }
}
