using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public class TitleLoadGame06DetailStateBranch : BaseTitleLoadGame06DetailStateBranch
    {
        public override bool TitleLoadGame_to_FadeIn08(TitleStateManagerData manager_data, TitleLoadGameState state)
        {
            return false;
        }

        public override bool TitleLoadGame_to_SelectChoice04(TitleStateManagerData manager_data, TitleLoadGameState state)
        {
            return false;
        }

    }
}
