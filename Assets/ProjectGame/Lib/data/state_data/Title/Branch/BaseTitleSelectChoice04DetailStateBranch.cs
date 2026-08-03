using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseTitleSelectChoice04DetailStateBranch : BaseTitleSelectChoiceDetailStateBranch
    {
        public override TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleSelectChoiceState state)
        {
            if (TitleSelectChoice_to_InitialStart05(manager_data, state))
                return TitleStateID.InitialStart05;
            if (TitleSelectChoice_to_LoadGame06(manager_data, state))
                return TitleStateID.LoadGame06;
            if (TitleSelectChoice_to_ExitGame07(manager_data, state))
                return TitleStateID.ExitGame07;
            return TitleStateID.None;
        }
    }
}
