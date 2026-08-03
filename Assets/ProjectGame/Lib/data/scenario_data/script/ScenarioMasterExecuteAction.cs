
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

public class ScenarioMasterExecuteAction
{
    private List<ScenarioGroupExecuteAction> scenarioActionList = new List<ScenarioGroupExecuteAction>();
    public int executeGroupID { get; private set; } = 1;
    public int executeSubGroupID { get; private set; } = 1;
    public bool IsExecuteFinish { get; private set; }
    private ScenarioExecuteData executeData = new ScenarioExecuteData();
    public ScenarioExecuteData ExecuteData {  get { return executeData; } }
    public void SetExecuteGroupID(int value)
    {
        if (value <= 0 || value >= scenarioActionList.Count) return;
        executeGroupID = value;
    }
    public void SetExecuteSubGroupID(int value)
    {
        if(value <= 0 || value >= scenarioActionList.Find(id => id.GroupID == executeGroupID).ScenarioActionListCount()) return;
        executeSubGroupID = value;
    }
    
    private List<ScenarioGroupExecuteAction> FindGroupActionList(int groupID)
    {
        return scenarioActionList.FindAll(data => data.GroupID == groupID);
    }

    public bool IsMaxReached()
    {
        return IsExecuteFinish;
    }

    public void SetUp(BinaryReader reader)
    {
        IsExecuteFinish = false;
        int groupEventCount = reader.ReadInt32(); // グループイベント数
        for (int i = 0; i < groupEventCount; i++)
        {
            var addAction = new ScenarioGroupExecuteAction();
            addAction.SetUp(reader);
            scenarioActionList.Add(addAction);
        }
    }

    public async UniTask OnInitializeAsync(CancellationTokenSource ct)
    {
        if (IsMaxReached()) return;

        var find = FindGroupActionList(executeGroupID);
        var tasks = find.Select(action => action.OnInitializeAsync(executeSubGroupID, executeData,ct));
        await UniTask.WhenAll(tasks).AttachExternalCancellation(ct.Token);
    }

    public async UniTask OnExecuteAsync(CancellationTokenSource ct)
    {
        if (IsMaxReached()) return;

        var find = FindGroupActionList(executeGroupID).First();
        var subFind = find.FindSubGroupActionList(executeSubGroupID);
        var tasks = subFind.Select(action => action.OnExecuteAsync(executeData, ct)).ToArray();
        await UniTask.WhenAll(tasks).AttachExternalCancellation(ct.Token);
    }

    public async UniTask OnFinalizeAsync(CancellationTokenSource ct)
    {
        if (IsMaxReached()) return;

        var find = FindGroupActionList(executeGroupID).First();
        var subFind = find.FindSubGroupActionList(executeSubGroupID);
        var tasks = subFind.Select(action => action.OnFinalizeAsync(executeData, ct)).ToArray();
        await UniTask.WhenAll(tasks).AttachExternalCancellation(ct.Token);

        executeSubGroupID++;
        var currentGroup = scenarioActionList.Find(data => data.GroupID == executeGroupID);
        if (currentGroup != null)
        {
            var subGroupCount = currentGroup.FindSubGroupActionList(executeSubGroupID);
            if (subGroupCount == null || !subGroupCount.Any())
            {
                executeGroupID++;
                executeSubGroupID = 1;
                if(executeGroupID >= scenarioActionList.Count)
                {
                    IsExecuteFinish = true;
                }
            }
        }
        else
        {
            IsExecuteFinish = true;
        }
    }
    
    public void AllRelease()
    {
        executeGroupID = executeSubGroupID = 1;
        scenarioActionList.Clear();
    }
}
