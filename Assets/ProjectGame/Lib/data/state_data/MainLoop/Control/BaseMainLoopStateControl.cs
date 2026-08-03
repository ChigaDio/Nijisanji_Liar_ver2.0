using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GameCore.States.ID;
using GameCore.States.Managers;
using GameCore.States;

namespace GameCore.States.Control
{
    public abstract class BaseMainLoopStateControl
        : BaseStateControl<MainLoopStateID, MainLoopStateManagerData, BaseMainLoopState>
    {
        protected override StateGroupID GroupID => StateGroupID.MainLoop;

        protected override MainLoopStateID GetInitStartID()
        {
            return MainLoopStateID.Loading01;
        }

        public override void BranchState()
        {
            if (state.IsActive) return;

            isTransitioning = true;
            try
            {
            var id = state_manager_data.PopStateID();
            if(id == MainLoopStateID.None) id = state_manager_data.GetNowStateID();
            switch (id)
            {
                case MainLoopStateID.Loading:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case MainLoopStateID.Title:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case MainLoopStateID.Game:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case MainLoopStateID.Unload:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case MainLoopStateID.Exit:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case MainLoopStateID.Loading01:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    var next_id = MainLoopStateID.Title02;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case MainLoopStateID.Title02:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    var next_id = MainLoopStateID.Game03;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case MainLoopStateID.Game03:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case MainLoopStateID.Unload04:
                {
                    state.Exit(state_manager_data);
                    state_manager_data.PopUpStateID();
                    var next_id = MainLoopStateID.Exit05;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state.Enter(state_manager_data);
                    return;
                }
                case MainLoopStateID.Exit05:
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
            if(id == MainLoopStateID.None) id = state_manager_data.GetNowStateID();
            switch (id)
            {
                case MainLoopStateID.Loading:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case MainLoopStateID.Title:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case MainLoopStateID.Game:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case MainLoopStateID.Unload:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case MainLoopStateID.Exit:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case MainLoopStateID.Loading01:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    var next_id = MainLoopStateID.Title02;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case MainLoopStateID.Title02:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    var next_id = MainLoopStateID.Game03;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case MainLoopStateID.Game03:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case MainLoopStateID.Unload04:
                {
                    await state.ExitAsync(state_manager_data, stateCts.Token);
                    state_manager_data.PopUpStateID();
                    var next_id = MainLoopStateID.Exit05;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    var ct2 = RenewStateToken(life_time_token);
                    await state.EnterAsync(state_manager_data, ct2);
                    return;
                }
                case MainLoopStateID.Exit05:
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
            if(id == MainLoopStateID.None) id = state_manager_data.GetNowStateID();
            switch (id)
            {
                case MainLoopStateID.Loading:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case MainLoopStateID.Title:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case MainLoopStateID.Game:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case MainLoopStateID.Unload:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case MainLoopStateID.Exit:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    id = state_manager_data.PopStateID();
                    if(id == MainLoopStateID.None) id = state_manager_data.SaveStateID;
                    if(id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        is_finish = true;
                        return;
                    }
                    else
                    {
                        state_manager_data.ChangeStateNowID(id);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                    }
                    state = FactoryState(id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case MainLoopStateID.Loading01:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    var next_id = MainLoopStateID.Title02;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case MainLoopStateID.Title02:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    var next_id = MainLoopStateID.Game03;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case MainLoopStateID.Game03:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                   var next_id = state.BranchNextState(state_manager_data);
                    state_manager_data.ChangeStateNowID(next_id);
                    if (next_id == MainLoopStateID.None)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case MainLoopStateID.Unload04:
                {
                    if (state.UseExitSync) state.Exit(state_manager_data);
                    if (state.UseExitAsync) state.ExitAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    state_manager_data.PopUpStateID();
                    var next_id = MainLoopStateID.Exit05;
                    state_manager_data.ChangeStateNowID(next_id);
                    state = FactoryState(next_id);
                    if (state == null)
                    {
                        state_manager_data.ChangeStateNowID(MainLoopStateID.None);
                        state_manager_data.SaveStateID = MainLoopStateID.None;
                        is_finish = true;
                        return;
                    }
                    combinedAsyncUpdateStarted = false;
                    if (state.UseEnterSync) state.Enter(state_manager_data);
                    if (state.UseEnterAsync) state.EnterAsync(state_manager_data, combinedCts.Token).Forget(LogAsyncException);
                    return;
                }
                case MainLoopStateID.Exit05:
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

        public override BaseMainLoopState FactoryState(MainLoopStateID state_id)
        {
            switch (state_id)
            {
                case MainLoopStateID.Loading: return new MainLoopLoadingState();
                case MainLoopStateID.Title: return new MainLoopTitleState();
                case MainLoopStateID.Game: return new MainLoopGameState();
                case MainLoopStateID.Unload: return new MainLoopUnloadState();
                case MainLoopStateID.Exit: return new MainLoopExitState();
                case MainLoopStateID.Loading01: return new MainLoopLoadingState();
                case MainLoopStateID.Title02: return new MainLoopTitleState();
                case MainLoopStateID.Game03: return new MainLoopGameState();
                case MainLoopStateID.Unload04: return new MainLoopUnloadState();
                case MainLoopStateID.Exit05: return new MainLoopExitState();
                default: return null;
            }
        }
    }
}
