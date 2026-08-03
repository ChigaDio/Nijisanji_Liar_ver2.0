
using Cysharp.Threading.Tasks;
using GameCore.Scenario;
using System.IO;
using System.Threading;
using UnityEngine;

public class ScenarioExecuteAction
{
    private BaseScenarioRoleData roleData;
    private BaseOrigintScenarioRoleAction action;


    public bool IsStartUp => action != null && action.IsStartUp;
    public bool IsRelease => action != null && action.IsRelease;
    public bool IsCompleted => action != null && action.IsCompleted && action.IsStartUp;
    public bool IsOneCompleted => action != null && action.IsOneExecute && action.IsStartUp;

    public void SetUp(ScenarioRoleID id, BinaryReader reader)
    {
        roleData = ScenarioRoleFactory.CreateRoleData(id);
        roleData.ReadBinary(reader);
        action = ScenarioRoleFactory.CreateRoleAction(roleData);
    }


    public async UniTask OnInitializeAsync(ScenarioExecuteData executeData,CancellationTokenSource ct)
    {
        if (IsStartUp)
        {
            await UniTask.Yield(ct.Token);
            return;
        }
        await action.OnInitializeAsync(executeData,ct);
        action.OnInitialize(executeData, ct);
        await UniTask.Yield(ct.Token);
    }


    public async UniTask OnOneExecuteAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
    {
        if (IsOneCompleted)
        {
            await UniTask.Yield(ct.Token);
            return;
        }
        await action.OnOneExecuteAsync(executeData,ct);
        action.OnOneExecute(executeData, ct);
        await UniTask.Yield(ct.Token);
    }


    public async UniTask OnExecuteAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
    {
        if (IsCompleted)
        {
            await UniTask.Yield(ct.Token);
            return;
        }
        await action.OnExecuteAsync(executeData,ct);
        action.OnExecute(executeData, ct);
        await UniTask.Yield(ct.Token);
    }

    public async UniTask OnFinalizeAsync(ScenarioExecuteData executeData,CancellationTokenSource ct)
    {
        if (IsRelease)
        {
            await UniTask.Yield(ct.Token);
            return;
        }
        await action.OnFinalizeAsync(executeData,ct);
        action.OnFinalize(executeData, ct);
        await UniTask.Yield(ct.Token);
    }
}
