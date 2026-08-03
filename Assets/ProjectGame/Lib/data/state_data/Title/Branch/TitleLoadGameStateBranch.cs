using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public class TitleLoadGameStateBranch : BaseTitleStateBranch<TitleLoadGameState, BaseTitleLoadGameDetailStateBranch>
    {
        public override TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleLoadGameState state)
        {
            var id = manager_data.GetNowStateID();
            var branch = Factory(id);
            return branch != null ? branch.ConditionsBranch(manager_data, state) : TitleStateID.None;
        }

        public override BaseTitleLoadGameDetailStateBranch Factory(TitleStateID id)
        {
            switch (id)
            {
                case TitleStateID.LoadGame06:
                    return new TitleLoadGame06DetailStateBranch();
                default:
                    return null;
            }
        }
    }
}
