using System.Collections.Generic;
using UnityEngine;
using GameCore.States.ID;

namespace GameCore.States.Managers
{
    public class BaseMainLoopStateManagerData : BaseStateManagerData<GameCore.States.ID.MainLoopStateID>
    {
        protected MainLoopStateBaseID now_state_base_id = MainLoopStateBaseID.None;
        protected MainLoopStateBaseID old_state_base_id = MainLoopStateBaseID.None;

        public MainLoopStateBaseID GetNowStateBaseID() => now_state_base_id;
        public MainLoopStateBaseID GetOldStateBaseID() => old_state_base_id;

        public override void ChangeStateNowID(MainLoopStateID new_state_id)
        {
            base.ChangeStateNowID(new_state_id);
            old_state_base_id = now_state_base_id;
            switch (new_state_id)
            {
                case MainLoopStateID.Loading: now_state_base_id = MainLoopStateBaseID.Loading; break;
                case MainLoopStateID.Title: now_state_base_id = MainLoopStateBaseID.Title; break;
                case MainLoopStateID.Game: now_state_base_id = MainLoopStateBaseID.Game; break;
                case MainLoopStateID.Unload: now_state_base_id = MainLoopStateBaseID.Unload; break;
                case MainLoopStateID.Exit: now_state_base_id = MainLoopStateBaseID.Exit; break;
                case MainLoopStateID.Loading01: now_state_base_id = MainLoopStateBaseID.Loading; break;
                case MainLoopStateID.Title02: now_state_base_id = MainLoopStateBaseID.Title; break;
                case MainLoopStateID.Game03: now_state_base_id = MainLoopStateBaseID.Game; break;
                case MainLoopStateID.Unload04: now_state_base_id = MainLoopStateBaseID.Unload; break;
                case MainLoopStateID.Exit05: now_state_base_id = MainLoopStateBaseID.Exit; break;
                default: now_state_base_id = MainLoopStateBaseID.None; break;
            }
        }
   }
}
