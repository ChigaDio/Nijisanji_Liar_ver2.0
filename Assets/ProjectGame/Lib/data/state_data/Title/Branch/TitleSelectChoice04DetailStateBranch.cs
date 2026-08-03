using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public class TitleSelectChoice04DetailStateBranch : BaseTitleSelectChoice04DetailStateBranch
    {
        public override bool TitleSelectChoice_to_InitialStart05(TitleStateManagerData manager_data, TitleSelectChoiceState state)
        {
            return false;
        }

        public override bool TitleSelectChoice_to_LoadGame06(TitleStateManagerData manager_data, TitleSelectChoiceState state)
        {
            return false;
        }

        public override bool TitleSelectChoice_to_ExitGame07(TitleStateManagerData manager_data, TitleSelectChoiceState state)
        {
            return false;
        }

    }
}
