using Cysharp.Threading.Tasks;
using GameCore;
using GameCore.Tables;
using System.IO;
using System.Threading;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;                    // ← 追加
using UnityEngine.ResourceManagement.AsyncOperations;   // ← 追加

public class ClassDataIDCore : BaseSingleton<ClassDataIDCore>
{
    private ClassDataHeader m_classDataTables;
    private CancellationToken cts;
    private bool isLoaded;
    public bool IsLoaded => isLoaded;

    public override void AwakeSingleton()
    {
        base.AwakeSingleton();
        instance = this;
        if (cts == null) cts = this.GetCancellationTokenOnDestroy();
        isLoaded = false;
        
        DontDestroyOnLoad(instance);
    }
    

    private void OnDestroy()
    {
    }

    /// <summary>
    /// ALL_ID_BIN を読み込み（Addressable対応追加）
    /// </summary>
    public async UniTask LoadClassDataAsync(Func<BinaryReader, ClassDataHeader, UniTask> onLoaded, bool addressable = false)
    {
        if (cts == null) cts = this.GetCancellationTokenOnDestroy();
        if (isLoaded) return;

        string path = addressable == true ?  SupportFiles.ID_BIN_FILE  :  SupportFiles.ALL_ID_BIN;

        try
        {
            if (!addressable)
            {
                // 従来の同期ファイル読み込み
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    if (m_classDataTables == null) m_classDataTables = new ClassDataHeader(reader);
                    if (onLoaded != null)
                    {
                        await ExecuteOnThreadPoolAndReturn(onLoaded, reader, m_classDataTables, cts);
                    }
                    isLoaded = true;
                }
            }
            else
            {
                // ====================== Addressableの場合 ======================
                AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(path);

                TextAsset textAsset = await handle.ToUniTask(cancellationToken: cts);

                if (textAsset == null)
                {
                    Debug.LogError($"Failed to load Addressable binary: {path}");
                    if (handle.IsValid()) Addressables.Release(handle);
                    return;
                }

                using (MemoryStream ms = new MemoryStream(textAsset.bytes))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    if (m_classDataTables == null) m_classDataTables = new ClassDataHeader(reader);
                    if (onLoaded != null)
                    {
                        await ExecuteOnThreadPoolAndReturn(onLoaded, reader, m_classDataTables, cts);
                    }
                    isLoaded = true;
                }

                if (handle.IsValid()) Addressables.Release(handle);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.LogWarning("TableIDCoreの読み込みがキャンセルされました。");
        }
        catch (Exception ex)
        {
            Debug.LogError($"読み込み中にエラーが発生: {ex}");
        }
    }

    private async UniTask ExecuteOnThreadPoolAndReturn(
        Func<BinaryReader, ClassDataHeader, UniTask> action,
        BinaryReader reader,
        ClassDataHeader classDataHeader,
        CancellationToken token)
    {
#if UNITY_WEBGL
        await action(reader, classDataHeader).AttachExternalCancellation(token);
#else
        await UniTask.SwitchToThreadPool();
        await action(reader, classDataHeader).AttachExternalCancellation(token);
        await UniTask.SwitchToMainThread();
#endif
    }
}
