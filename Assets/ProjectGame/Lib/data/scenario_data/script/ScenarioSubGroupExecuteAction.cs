
using Cysharp.Threading.Tasks;
using GameCore.Scenario;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

public class ScenarioSubGroupExecuteAction
{
    private List<ScenarioExecuteAction> scenarioActionList = new List<ScenarioExecuteAction>();
    public int SubGroupID { get; private set; }

    public void SetUp(BinaryReader reader)
    {
        SubGroupID = reader.ReadInt32(); // サブイベントID
        int actionCount = reader.ReadInt32(); // アクション（ロール）数
        for (int i = 0; i < actionCount; i++)
        {
            var addAction = new ScenarioExecuteAction();
            var id = (ScenarioRoleID)reader.ReadInt32(); // ロールID
            addAction.SetUp(id, reader);
            scenarioActionList.Add(addAction);
        }
    }

    public async UniTask OnInitializeAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
    {
        var tasks = scenarioActionList.Select(action => action.OnInitializeAsync(executeData,ct));
       
        await UniTask.WhenAll(tasks).AttachExternalCancellation(ct.Token);
    }

    public async UniTask OnExecuteAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
    {
        var oneTasks = scenarioActionList
                    .Where(action => !action.IsOneCompleted)
                    .Select(action => action.OnOneExecuteAsync(executeData,ct))
                    .ToArray();
        await UniTask.WhenAll(oneTasks).AttachExternalCancellation(ct.Token).AttachExternalCancellation(ct.Token);
        while (scenarioActionList.Any(action => !action.IsCompleted))
        {
            var tasks = scenarioActionList
                .Where(action => !action.IsCompleted)
                .Select(action => action.OnExecuteAsync(executeData,ct))
                .ToArray();
            await UniTask.WhenAll(tasks).AttachExternalCancellation(ct.Token);
            await UniTask.Yield(ct.Token);
        }
    }

    public async UniTask OnFinalizeAsync(ScenarioExecuteData executeData, CancellationTokenSource ct)
    {
        var tasks = scenarioActionList.Select(action => action.OnFinalizeAsync(executeData,ct)).ToArray();
        await UniTask.WhenAll(tasks).AttachExternalCancellation(ct.Token);
    }
}
