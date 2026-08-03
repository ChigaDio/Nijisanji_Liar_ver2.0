using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseTitleLoadGame06DetailStateBranch : BaseTitleLoadGameDetailStateBranch
    {
        public override TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleLoadGameState state)
        {
            if (TitleLoadGame_to_FadeIn08(manager_data, state))
                return TitleStateID.FadeIn08;
            if (TitleLoadGame_to_SelectChoice04(manager_data, state))
                return TitleStateID.SelectChoice04;
            return TitleStateID.None;
        }
    }
}
