using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseMainLoopGame03DetailStateBranch : BaseMainLoopGameDetailStateBranch
    {
        public override MainLoopStateID ConditionsBranch(MainLoopStateManagerData manager_data, MainLoopGameState state)
        {
            if (MainLoopGame_to_Unload04(manager_data, state))
                return MainLoopStateID.Unload04;
            if (MainLoopGame_to_Title02(manager_data, state))
                return MainLoopStateID.Title02;
            return MainLoopStateID.None;
        }
    }
}
