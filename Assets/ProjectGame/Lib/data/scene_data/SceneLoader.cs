using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using GameCore.Enums;

public class SceneLoader
{
    private static readonly HashSet<GameSceneID> loadedScenes = new HashSet<GameSceneID>();
    private static readonly Dictionary<GameSceneID, UniTask> loadingTasks = new Dictionary<GameSceneID, UniTask>();

    #region Load / Unload

    public static async UniTask LoadSceneAsync(GameSceneID scene, bool additive = false, Action action = null)
    {
        if (loadedScenes.Contains(scene))
        {
            DebugLog($"Scene '{scene}' is already loaded.");
            return;
        }

        if (loadingTasks.TryGetValue(scene, out UniTask existingTask))
        {
            DebugLog($"Scene '{scene}' is already loading, waiting...");
            await existingTask;
            return;
        }

        var task = InternalLoadSceneAsync(scene, additive, action);
        loadingTasks.Add(scene, task);

        try
        {
            await task;
        }
        finally
        {
            loadingTasks.Remove(scene);
        }
    }

    private static async UniTask InternalLoadSceneAsync(GameSceneID scene, bool additive, Action action = null)
    {
        if (!SceneList.sceneNames.TryGetValue(scene, out string sceneName))
        {
            Debug.LogError($"Scene enum '{scene}' is not mapped to a scene name.");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' does not exist in build settings.");
            return;
        }

        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        asyncOp.allowSceneActivation = true;

        while (!asyncOp.isDone)
            await UniTask.Yield();

        loadedScenes.Add(scene);
        action?.Invoke();
        DebugLog($"Scene '{scene}' loaded successfully.");
    }

    public static async UniTask UnloadSceneAsync(GameSceneID scene, Action action = null)
    {
        if (!loadedScenes.Contains(scene))
        {
            DebugLog($"Scene '{scene}' is not loaded, cannot unload.");
            return;
        }

        if (!SceneList.sceneNames.TryGetValue(scene, out string sceneName))
        {
            Debug.LogError($"Scene enum '{scene}' is not mapped to a scene name.");
            return;
        }

        AsyncOperation asyncOp = SceneManager.UnloadSceneAsync(sceneName);
        if (asyncOp == null)
        {
            Debug.LogError($"Failed to unload scene '{sceneName}'.");
            return;
        }

        while (!asyncOp.isDone)
            await UniTask.Yield();

        loadedScenes.Remove(scene);
        GC.Collect();
        await Resources.UnloadUnusedAssets();
        action?.Invoke();   
        DebugLog($"Scene '{scene}' unloaded successfully.");
    }

    /// <summary>
    /// 現在ロード済みのシーンをすべてアンロード
    /// </summary>
    /// <param name="keepScenes">残したいシーン</param>
    public static async UniTask UnloadAllScenesAsync(GameSceneID[] keepScenes, Action action = null)
    {
        var toKeep = new HashSet<GameSceneID>(keepScenes);
        var toUnload = new List<GameSceneID>();

        foreach (var scene in loadedScenes)
            if (!toKeep.Contains(scene))
                toUnload.Add(scene);

        foreach (var scene in toUnload)
            await UnloadSceneAsync(scene);

        action?.Invoke();
    }

    #endregion

    #region Instantiate in Scene

    /// <summary>
    /// 指定シーンに GameObject を生成
    /// </summary>
    public static GameObject InstantiateInScene(GameObject prefab, GameSceneID scene)
    {
        if (!TryGetLoadedScene(scene, out Scene targetScene))
            return null;

        GameObject obj = GameObject.Instantiate(prefab);
        SceneManager.MoveGameObjectToScene(obj, targetScene); // 安全に所属させる
        return obj;
    }

    #endregion

    #region GetComponent In Scene

    /// <summary>
    /// 指定シーンのルートオブジェクト（およびその子）から、型Tのコンポーネントを1つ取得する。
    /// 見つからない場合は null を返す。
    /// </summary>
    public static T GetComponentInScene<T>(GameSceneID scene, bool includeInactive = true) where T : Component
    {
        if (!TryGetLoadedScene(scene, out Scene targetScene))
            return null;

        foreach (GameObject root in targetScene.GetRootGameObjects())
        {
            T found = root.GetComponentInChildren<T>(includeInactive);
            if (found != null)
                return found;
        }
        return null;
    }

    /// <summary>
    /// 指定シーンのルートオブジェクト（およびその子）から、型Tのコンポーネントを全て取得する。
    /// </summary>
    public static List<T> GetComponentsInScene<T>(GameSceneID scene, bool includeInactive = true) where T : Component
    {
        var results = new List<T>();
        if (!TryGetLoadedScene(scene, out Scene targetScene))
            return results;

        foreach (GameObject root in targetScene.GetRootGameObjects())
        {
            results.AddRange(root.GetComponentsInChildren<T>(includeInactive));
        }
        return results;
    }

    private static bool TryGetLoadedScene(GameSceneID scene, out Scene targetScene)
    {
        if (!SceneList.sceneNames.TryGetValue(scene, out string sceneName))
        {
            Debug.LogError($"Scene enum '{scene}' is not mapped to a scene name.");
            targetScene = default;
            return false;
        }

        targetScene = SceneManager.GetSceneByName(sceneName);
        if (!targetScene.isLoaded)
        {
            Debug.LogError($"Scene '{sceneName}' is not loaded.");
            return false;
        }
        return true;
    }

    #endregion

    #region Utility

    public static IReadOnlyCollection<GameSceneID> GetLoadedScenes() => loadedScenes;

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private static void DebugLog(string message) => Debug.Log("[SceneLoader] " + message);

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterActiveScene()
    {
        var active = SceneManager.GetActiveScene();
        foreach (var kv in SceneList.sceneNames)
        {
            if (kv.Value == active.name)
            {
                loadedScenes.Add(kv.Key);
                break;
            }
        }
    }

    #endregion
}
