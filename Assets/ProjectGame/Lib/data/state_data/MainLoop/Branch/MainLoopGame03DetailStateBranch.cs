using System;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;

namespace GameCore.States.Branch
{
    public class MainLoopGame03DetailStateBranch : BaseMainLoopGame03DetailStateBranch
    {
        public override bool MainLoopGame_to_Unload04(MainLoopStateManagerData manager_data, MainLoopGameState state)
        {
            return false;
        }

        public override bool MainLoopGame_to_Title02(MainLoopStateManagerData manager_data, MainLoopGameState state)
        {
            return false;
        }

    }
}
