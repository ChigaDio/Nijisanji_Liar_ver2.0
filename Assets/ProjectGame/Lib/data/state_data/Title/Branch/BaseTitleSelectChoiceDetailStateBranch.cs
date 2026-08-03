using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseTitleSelectChoiceDetailStateBranch : BaseTitleDetailStateBranch<TitleSelectChoiceState>
    {
        public override abstract TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleSelectChoiceState state);
        public virtual bool TitleSelectChoice_to_InitialStart05(TitleStateManagerData manager_data, TitleSelectChoiceState state) { return false; }
        public virtual bool TitleSelectChoice_to_LoadGame06(TitleStateManagerData manager_data, TitleSelectChoiceState state) { return false; }
        public virtual bool TitleSelectChoice_to_ExitGame07(TitleStateManagerData manager_data, TitleSelectChoiceState state) { return false; }
    }
}
