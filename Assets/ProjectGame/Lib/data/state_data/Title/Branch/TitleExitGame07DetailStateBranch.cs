using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public class TitleExitGame07DetailStateBranch : BaseTitleExitGame07DetailStateBranch
    {
        public override bool TitleExitGame_to_FadeIn08(TitleStateManagerData manager_data, TitleExitGameState state)
        {
            return false;
        }

        public override bool TitleExitGame_to_SelectChoice04(TitleStateManagerData manager_data, TitleExitGameState state)
        {
            return false;
        }

    }
}
