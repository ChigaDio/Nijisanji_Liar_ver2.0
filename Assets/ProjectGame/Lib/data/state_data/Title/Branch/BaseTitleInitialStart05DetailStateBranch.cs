using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseTitleInitialStart05DetailStateBranch : BaseTitleInitialStartDetailStateBranch
    {
        public override TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleInitialStartState state)
        {
            if (TitleInitialStart_to_FadeIn08(manager_data, state))
                return TitleStateID.FadeIn08;
            if (TitleInitialStart_to_SelectChoice04(manager_data, state))
                return TitleStateID.SelectChoice04;
            return TitleStateID.None;
        }
    }
}
