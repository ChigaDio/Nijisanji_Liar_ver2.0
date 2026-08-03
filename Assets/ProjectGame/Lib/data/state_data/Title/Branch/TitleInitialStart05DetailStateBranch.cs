using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public class TitleInitialStart05DetailStateBranch : BaseTitleInitialStart05DetailStateBranch
    {
        public override bool TitleInitialStart_to_FadeIn08(TitleStateManagerData manager_data, TitleInitialStartState state)
        {
            return false;
        }

        public override bool TitleInitialStart_to_SelectChoice04(TitleStateManagerData manager_data, TitleInitialStartState state)
        {
            return false;
        }

    }
}
