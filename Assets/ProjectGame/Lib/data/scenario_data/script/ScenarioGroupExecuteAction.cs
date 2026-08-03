
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class ScenarioGroupExecuteAction
{
    private List<ScenarioSubGroupExecuteAction> scenarioActionList = new List<ScenarioSubGroupExecuteAction>();
    public int ScenarioActionListCount() => scenarioActionList.Count;
    public int GroupID { get; private set; }

    public List<ScenarioSubGroupExecuteAction> FindSubGroupActionList(int subGroupID)
    {
        return scenarioActionList.FindAll(data => data.SubGroupID == subGroupID);
    }

    public void SetUp(BinaryReader reader)
    {
        GroupID = reader.ReadInt32(); // グループイベントID
        int subEventCount = reader.ReadInt32(); // サブイベント数
        for (int i = 0; i < subEventCount; i++)
        {
            var addAction = new ScenarioSubGroupExecuteAction();
            addAction.SetUp(reader);
            scenarioActionList.Add(addAction);
        }
    }

    public async UniTask OnInitializeAsync(int subGroupID, ScenarioExecuteData executeData, CancellationTokenSource ct)
    {
        var find = FindSubGroupActionList(subGroupID);
        var tasks = find.Select(action => action.OnInitializeAsync(executeData,ct));
        await UniTask.WhenAll(tasks).AttachExternalCancellation(ct.Token);
    }

    public async UniTask OnExecuteAsync(int subGroupID, ScenarioExecuteData executeData, CancellationTokenSource ct)
    {
        var find = FindSubGroupActionList(subGroupID);
        var tasks = find.Select(action => action.OnExecuteAsync(executeData,ct));
        await UniTask.WhenAll(tasks).AttachExternalCancellation(ct.Token);
    }

    public async UniTask OnFinalizeAsync(int subGroupID, ScenarioExecuteData executeData, CancellationTokenSource ct)
    {
        var find = FindSubGroupActionList(subGroupID);
        var tasks = find.Select(action => action.OnFinalizeAsync(executeData,ct));
        await UniTask.WhenAll(tasks).AttachExternalCancellation(ct.Token);
    }
}
