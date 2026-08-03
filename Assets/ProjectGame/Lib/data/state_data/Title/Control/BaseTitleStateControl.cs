using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;
using GameCore.States;

namespace GameCore.States.Control
{
    public abstract class BaseTitleStateControl
        : BaseStateControl<TitleStateID, TitleStateManagerData, BaseTitleState>
    {
        protected override StateGroupID GroupID => StateGroupID.Title;

        protected override TitleStateID GetInitStartID()
        {
            return TitleStateID.Loading01;
        }

        public override void BranchState()
        {
            if (state.IsActive) return;

            isTransitioning = true;
            try
            {
            var id = state_manager_data.PopStateID();
            if(id == TitleStateID.None) id = state_manager_data.GetNowStateID();
            switch (id)
            {
                case TitleStateID.Loading:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.FadeOut:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.StartAnim:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.SelectChoice:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.InitialStart:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.LoadGame:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.ExitGame:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.FadeIn:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.Loading01:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    var next_id = TitleStateID.FadeOut02;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.FadeOut02:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    var next_id = TitleStateID.StartAnim03;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.StartAnim03:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    var next_id = TitleStateID.SelectChoice04;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.SelectChoice04:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.InitialStart05:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.LoadGame06:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.ExitGame07:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case TitleStateID.FadeIn08:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    is_finish = true;
                    return;
                }
            }
            }
            finally
            {
                isTransitioning = false;
            }
        }

        public override async UniTask BranchStateAsync(CancellationToken life_time_token)
        {
            if (state.IsActive) return;

            isTransitioning = true;
            try
            {
            var id = state_manager_data.PopStateID();
            if(id == TitleStateID.None) id = state_manager_data.GetNowStateID();
            switch (id)
            {
                case TitleStateID.Loading:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.FadeOut:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.StartAnim:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.SelectChoice:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.InitialStart:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.LoadGame:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.ExitGame:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.FadeIn:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.Loading01:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    var next_id = TitleStateID.FadeOut02;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.FadeOut02:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    var next_id = TitleStateID.StartAnim03;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.StartAnim03:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    var next_id = TitleStateID.SelectChoice04;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.SelectChoice04:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.InitialStart05:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.LoadGame06:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.ExitGame07:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case TitleStateID.FadeIn08:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    is_finish = true;
                    return;
                }
            }
            }
            finally
            {
                isTransitioning = false;
            }
        }

        public override void BranchStateCombined()
        {
            isTransitioning = true;
            try
            {
            var id = state_manager_data.PopStateID();
            if(id == TitleStateID.None) id = state_manager_data.GetNowStateID();
            switch (id)
            {
                case TitleStateID.Loading:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.FadeOut:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.StartAnim:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.SelectChoice:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.InitialStart:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.LoadGame:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.ExitGame:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.FadeIn:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == TitleStateID.None) id = state_manager_data.SaveStateID;
                    if(id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = TitleStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.Loading01:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    var next_id = TitleStateID.FadeOut02;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.FadeOut02:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    var next_id = TitleStateID.StartAnim03;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.StartAnim03:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    var next_id = TitleStateID.SelectChoice04;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.SelectChoice04:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.InitialStart05:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.LoadGame06:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.ExitGame07:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == TitleStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(TitleStateID.None);
                        state_manager_data.SaveStateID = TitleStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case TitleStateID.FadeIn08:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    is_finish = true;
                    return;
                }
            }
            }
            finally
            {
                isTransitioning = false;
            }
        }

        public override BaseTitleState FactoryState(TitleStateID state_id)
        {
            switch (state_id)
            {
                case TitleStateID.Loading: return new TitleLoadingState();
                case TitleStateID.FadeOut: return new TitleFadeOutState();
                case TitleStateID.StartAnim: return new TitleStartAnimState();
                case TitleStateID.SelectChoice: return new TitleSelectChoiceState();
                case TitleStateID.InitialStart: return new TitleInitialStartState();
                case TitleStateID.LoadGame: return new TitleLoadGameState();
                case TitleStateID.ExitGame: return new TitleExitGameState();
                case TitleStateID.FadeIn: return new TitleFadeInState();
                case TitleStateID.Loading01: return new TitleLoadingState();
                case TitleStateID.FadeOut02: return new TitleFadeOutState();
                case TitleStateID.StartAnim03: return new TitleStartAnimState();
                case TitleStateID.SelectChoice04: return new TitleSelectChoiceState();
                case TitleStateID.InitialStart05: return new TitleInitialStartState();
                case TitleStateID.LoadGame06: return new TitleLoadGameState();
                case TitleStateID.ExitGame07: return new TitleExitGameState();
                case TitleStateID.FadeIn08: return new TitleFadeInState();
                default: return null;
            }
        }
    }
}
