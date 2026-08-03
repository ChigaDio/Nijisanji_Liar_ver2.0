using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseTitleExitGameDetailStateBranch : BaseTitleDetailStateBranch<TitleExitGameState>
    {
        public override abstract TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleExitGameState state);
        public virtual bool TitleExitGame_to_FadeIn08(TitleStateManagerData manager_data, TitleExitGameState state) { return false; }
        public virtual bool TitleExitGame_to_SelectChoice04(TitleStateManagerData manager_data, TitleExitGameState state) { return false; }
    }
}
