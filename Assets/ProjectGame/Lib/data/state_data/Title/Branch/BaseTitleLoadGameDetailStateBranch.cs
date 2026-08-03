using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseTitleLoadGameDetailStateBranch : BaseTitleDetailStateBranch<TitleLoadGameState>
    {
        public override abstract TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleLoadGameState state);
        public virtual bool TitleLoadGame_to_FadeIn08(TitleStateManagerData manager_data, TitleLoadGameState state) { return false; }
        public virtual bool TitleLoadGame_to_SelectChoice04(TitleStateManagerData manager_data, TitleLoadGameState state) { return false; }
    }
}
