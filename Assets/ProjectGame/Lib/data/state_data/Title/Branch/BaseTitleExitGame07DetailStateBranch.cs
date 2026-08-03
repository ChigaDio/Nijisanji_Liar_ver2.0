using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseTitleExitGame07DetailStateBranch : BaseTitleExitGameDetailStateBranch
    {
        public override TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleExitGameState state)
        {
            if (TitleExitGame_to_FadeIn08(manager_data, state))
                return TitleStateID.FadeIn08;
            if (TitleExitGame_to_SelectChoice04(manager_data, state))
                return TitleStateID.SelectChoice04;
            return TitleStateID.None;
        }
    }
}
