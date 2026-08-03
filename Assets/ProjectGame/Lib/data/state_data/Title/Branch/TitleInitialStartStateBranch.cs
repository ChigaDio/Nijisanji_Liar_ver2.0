using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public class TitleInitialStartStateBranch : BaseTitleStateBranch<TitleInitialStartState, BaseTitleInitialStartDetailStateBranch>
    {
        public override TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleInitialStartState state)
        {
            var id = manager_data.GetNowStateID();
            var branch = Factory(id);
            return branch != null ? branch.ConditionsBranch(manager_data, state) : TitleStateID.None;
        }

        public override BaseTitleInitialStartDetailStateBranch Factory(TitleStateID id)
        {
            switch (id)
            {
                case TitleStateID.InitialStart05:
                    return new TitleInitialStart05DetailStateBranch();
                default:
                    return null;
            }
        }
    }
}
