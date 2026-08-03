using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public class TitleSelectChoiceStateBranch : BaseTitleStateBranch<TitleSelectChoiceState, BaseTitleSelectChoiceDetailStateBranch>
    {
        public override TitleStateID ConditionsBranch(TitleStateManagerData manager_data, TitleSelectChoiceState state)
        {
            var id = manager_data.GetNowStateID();
            var branch = Factory(id);
            return branch != null ? branch.ConditionsBranch(manager_data, state) : TitleStateID.None;
        }

        public override BaseTitleSelectChoiceDetailStateBranch Factory(TitleStateID id)
        {
            switch (id)
            {
                case TitleStateID.SelectChoice04:
                    return new TitleSelectChoice04DetailStateBranch();
                default:
                    return null;
            }
        }
    }
}
