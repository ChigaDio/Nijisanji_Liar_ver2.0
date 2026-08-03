
using Cysharp.Threading.Tasks;
using GameCore;
using GameCore.Scenario;
using System;
using System.IO;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ScenarioManagerCore : BaseSingleton<ScenarioManagerCore>
{
    public bool IsHeaderLoad { get; private set; } = false;

    private ScenarioMasterExecuteAction master = new ScenarioMasterExecuteAction();

    private string event_play_name = "";
    private string event_sub_name = "";
    private bool is_event_change = false;

    public override void AwakeSingleton()
    {
        base.AwakeSingleton();
        ScenarioEventBinaryHeader.ReadHeaderAsync(() =>
        {
            IsHeaderLoad = true;
        }, addressable: SupportFiles.ADDRESSABLE_CHECK).Forget();
    }

    public void SetExecuteGroupID(int value) => master?.SetExecuteGroupID(value);
    public void SetExecuteSubGroupID(int value) => master?.SetExecuteSubGroupID(value);

    public void SetEventName(string value_event_name, string value_event_sub_name)
    {
        event_play_name = value_event_name;
        event_sub_name = value_event_sub_name;
        is_event_change = true;
    }

    public void SetEventNameID(string value_event_name, string value_event_sub_name,
                               int value_group_id = 1, int value_sub_group_id = 1)
    {
        SetExecuteGroupID(value_group_id);
        SetExecuteSubGroupID(value_sub_group_id);
        SetEventName(value_event_name, value_event_sub_name);
    }

    /// <summary>
    /// シナリオを実行します。
    /// </summary>
    /// <param name="eventName">イベント名</param>
    /// <param name="eventSubName">サブイベント名</param>
    /// <param name="addressable">trueの場合 Addressable から読み込む（TextAsset）</param>
    /// <param name="action">完了時に実行するアクション</param>
    /// <param name="cts">外部キャンセルトークン</param>
    public async UniTask ScenarioExecuteUpdate(
        string eventName,
        string eventSubName,
        bool addressable = false,                    // ← 追加
        Action<ScenarioExecuteData> action = null,
        CancellationTokenSource cts = null)
    {
        using var localCts = new CancellationTokenSource();
        using var linkedCts = cts != null
            ? CancellationTokenSource.CreateLinkedTokenSource(localCts.Token, cts.Token, this.GetCancellationTokenOnDestroy())
            : CancellationTokenSource.CreateLinkedTokenSource(localCts.Token, this.GetCancellationTokenOnDestroy());


        event_play_name = eventName;
        event_sub_name = eventSubName;
        is_event_change = true;

        try
        {
            while (!master.IsExecuteFinish && !linkedCts.IsCancellationRequested && is_event_change)
            {
                master.AllRelease();
                is_event_change = false;

                var seekPos = ScenarioEventBinaryHeader.GetEventSeekPos(event_play_name, event_sub_name);

                if (addressable)
                {
                    await LoadAndExecuteWithAddressable(seekPos, linkedCts);
                }
                else
                {
                    await LoadAndExecuteWithFileStream(seekPos, linkedCts);
                }

                await UniTask.Yield(PlayerLoopTiming.Update, linkedCts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log($"ScenarioExecuteUpdate canceled for {eventName}/{eventSubName}");
            throw;
        }
        finally
        {
            action?.Invoke(master.ExecuteData);
            master.AllRelease();
            is_event_change = false;
            await UniTask.Yield(PlayerLoopTiming.Update, linkedCts.Token);
        }
    }

    // ====================== 非Addressable（従来通り） ======================
    private async UniTask LoadAndExecuteWithFileStream(long seekPos, CancellationTokenSource token)
    {
        using (var stream = new FileStream(SupportFiles.ALL_SCENARIO_EVENTS_BIN, FileMode.Open, FileAccess.Read))
        using (var reader = new BinaryReader(stream, Encoding.UTF8))
        {
            stream.Seek(seekPos, SeekOrigin.Begin);
            master.SetUp(reader);

            await ExecuteScenarioLoop(token);
        }
    }

    // ====================== Addressable版 ======================
    private async UniTask LoadAndExecuteWithAddressable(long seekPos, CancellationTokenSource token)
    {
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(SupportFiles.ALL_SCENARIO_EVENT_BIN_FILE);

        TextAsset textAsset = await handle.ToUniTask(cancellationToken: token.Token);

        if (textAsset == null)
        {
            Debug.LogError($"Failed to load Addressable scenario binary: {SupportFiles.ALL_SCENARIO_EVENT_BIN_FILE}");
            if (handle.IsValid()) Addressables.Release(handle);
            return;
        }

        try
        {
            using (var ms = new MemoryStream(textAsset.bytes))
            using (var reader = new BinaryReader(ms, Encoding.UTF8))
            {
                ms.Seek(seekPos, SeekOrigin.Begin);
                master.SetUp(reader);

                await ExecuteScenarioLoop(token);
            }
        }
        finally
        {
            if (handle.IsValid()) Addressables.Release(handle);
        }
    }

    // ====================== 共通の実行ループ ======================
    private async UniTask ExecuteScenarioLoop(CancellationTokenSource token)
    {
        while (!master.IsExecuteFinish && !token.IsCancellationRequested)
        {
            await master.OnInitializeAsync(token);   // CancellationToken対応を推奨
            await master.OnExecuteAsync(token);
            await master.OnFinalizeAsync(token);

            await UniTask.Yield(PlayerLoopTiming.Update, token.Token);
        }
    }
}


        