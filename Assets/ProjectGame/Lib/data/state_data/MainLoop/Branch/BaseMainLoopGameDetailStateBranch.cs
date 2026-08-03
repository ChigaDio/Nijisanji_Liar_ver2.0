using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public abstract class BaseMainLoopGameDetailStateBranch : BaseMainLoopDetailStateBranch<MainLoopGameState>
    {
        public override abstract MainLoopStateID ConditionsBranch(MainLoopStateManagerData manager_data, MainLoopGameState state);
        public virtual bool MainLoopGame_to_Unload04(MainLoopStateManagerData manager_data, MainLoopGameState state) { return false; }
        public virtual bool MainLoopGame_to_Title02(MainLoopStateManagerData manager_data, MainLoopGameState state) { return false; }
    }
}
