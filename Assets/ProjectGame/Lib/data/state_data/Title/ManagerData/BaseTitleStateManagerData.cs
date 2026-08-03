using System.Collections.Generic;
using UnityEngine;
using GameCore.States.ID;

namespace GameCore.States.Managers
{
    public class BaseTitleStateManagerData : BaseStateManagerData<GameCore.States.ID.TitleStateID>
    {
        protected TitleStateBaseID now_state_base_id = TitleStateBaseID.None;
        protected TitleStateBaseID old_state_base_id = TitleStateBaseID.None;

        public TitleStateBaseID GetNowStateBaseID() => now_state_base_id;
        public TitleStateBaseID GetOldStateBaseID() => old_state_base_id;

        public override void ChangeStateNowID(TitleStateID new_state_id)
        {
            base.ChangeStateNowID(new_state_id);
            old_state_base_id = now_state_base_id;
            switch (new_state_id)
            {
                case TitleStateID.Loading: now_state_base_id = TitleStateBaseID.Loading; break;
                case TitleStateID.FadeOut: now_state_base_id = TitleStateBaseID.FadeOut; break;
                case TitleStateID.StartAnim: now_state_base_id = TitleStateBaseID.StartAnim; break;
                case TitleStateID.SelectChoice: now_state_base_id = TitleStateBaseID.SelectChoice; break;
                case TitleStateID.InitialStart: now_state_base_id = TitleStateBaseID.InitialStart; break;
                case TitleStateID.LoadGame: now_state_base_id = TitleStateBaseID.LoadGame; break;
                case TitleStateID.ExitGame: now_state_base_id = TitleStateBaseID.ExitGame; break;
                case TitleStateID.FadeIn: now_state_base_id = TitleStateBaseID.FadeIn; break;
                case TitleStateID.Loading01: now_state_base_id = TitleStateBaseID.Loading; break;
                case TitleStateID.FadeOut02: now_state_base_id = TitleStateBaseID.FadeOut; break;
                case TitleStateID.StartAnim03: now_state_base_id = TitleStateBaseID.StartAnim; break;
                case TitleStateID.SelectChoice04: now_state_base_id = TitleStateBaseID.SelectChoice; break;
                case TitleStateID.InitialStart05: now_state_base_id = TitleStateBaseID.InitialStart; break;
                case TitleStateID.LoadGame06: now_state_base_id = TitleStateBaseID.LoadGame; break;
                case TitleStateID.ExitGame07: now_state_base_id = TitleStateBaseID.ExitGame; break;
                case TitleStateID.FadeIn08: now_state_base_id = TitleStateBaseID.FadeIn; break;
                default: now_state_base_id = TitleStateBaseID.None; break;
            }
        }
   }
}
