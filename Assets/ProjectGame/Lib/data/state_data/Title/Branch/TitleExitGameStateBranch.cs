using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public class TitleExitGameStateBranch : BaseTitleStateBranch<TitleExitGameState, BaseTitleExitGameDetailStateBranch>
    {
        public override TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleExitGameState state)
        {
            var id = manager_data.GetNowStateID();
            var branch = Factory(id);
            return branch != null ? branch.ConditionsBranch(manager_data, state) : TitleStateID.None;
        }

        public override BaseTitleExitGameDetailStateBranch Factory(TitleStateID id)
        {
            switch (id)
            {
                case TitleStateID.ExitGame07:
                    return new TitleExitGame07DetailStateBranch();
                default:
                    return null;
            }
        }
    }
}
