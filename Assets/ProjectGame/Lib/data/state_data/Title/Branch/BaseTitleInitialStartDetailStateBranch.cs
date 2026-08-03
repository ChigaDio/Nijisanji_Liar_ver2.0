using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseTitleInitialStartDetailStateBranch : BaseTitleDetailStateBranch<TitleInitialStartState>
    {
        public override abstract TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleInitialStartState state);
        public virtual bool TitleInitialStart_to_FadeIn08(TitleStateManagerData manager_data, TitleInitialStartState state) { return false; }
        public virtual bool TitleInitialStart_to_SelectChoice04(TitleStateManagerData manager_data, TitleInitialStartState state) { return false; }
    }
}
