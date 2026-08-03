
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;

public class EditorCommunication : EditorWindow
{
    private static TcpListener listener;
    private static Thread listenerThread;
    private static volatile bool pendingCommand;
    private static string pendingCommandName;
    private static CommData pendingCommandData;
    private static string commandResult;

    // ウィンドウを表示するためのメニュー項目を追加
    [MenuItem("Window/Communication Server")]
    public static void ShowWindow()
    {
        GetWindow<EditorCommunication>("Comm Server");
    }

    // JsonUtility は List を直列化できないのでラッパーが必要
    [System.Serializable]
    private class Wrapper<T>
    {
        public List<T> items;
    }

    [MenuItem("Tools/通信サーバー開始")]
    public static void StartServer()
    {
        if (listener != null) return;
        listener = new TcpListener(IPAddress.Loopback, 12345);
        listener.Start();
        listenerThread = new Thread(new ThreadStart(ListenForClients));
        listenerThread.IsBackground = true;
        listenerThread.Start();
        EditorApplication.update += ProcessPendingCommand;
        Debug.Log("通信サーバーを開始しました。");
    }

    [MenuItem("Tools/通信サーバー停止")]
    public static void StopServer()
    {
        listener?.Stop();
        listener = null;
        EditorApplication.update -= ProcessPendingCommand;
        Debug.Log("通信サーバーを停止しました。");
    }

    // EditorウィンドウのGUIを描画
    private void OnGUI()
    {
        GUILayout.Label("Communication Server Status", EditorStyles.boldLabel);

        // サーバー状態に応じてインジケーターの色を設定
        Color indicatorColor = listener != null ? Color.green : Color.red;
        string statusText = listener != null ? "Running" : "Stopped";

        // インジケーターの描画
        Rect indicatorRect = GUILayoutUtility.GetRect(20, 20);
        EditorGUI.DrawRect(indicatorRect, indicatorColor);

        // ステータステキストの表示
        GUILayout.Label($"Status: {statusText}", EditorStyles.label);

        // サーバー開始/停止ボタン
        if (listener == null)
        {
            if (GUILayout.Button("Start Server"))
            {
                StartServer();
            }
        }
        else
        {
            if (GUILayout.Button("Stop Server"))
            {
                StopServer();
            }
        }
    }

    private static void ListenForClients()
    {
        while (true)
        {
            try
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] lenBytes = new byte[4];
                    stream.Read(lenBytes, 0, 4);
                    int len = BitConverter.ToInt32(lenBytes, 0);
                    byte[] msgBytes = new byte[len];
                    stream.Read(msgBytes, 0, len);
                    string msg = System.Text.Encoding.UTF8.GetString(msgBytes);
                    var json = JsonUtility.FromJson<CommMessage>(msg);

                    pendingCommandName = json.command;
                    pendingCommandData = json.data;
                    pendingCommand = true;

                    while (pendingCommand)
                    {
                        Thread.Sleep(10);
                    }

                    var response = new CommMessage { result = commandResult };
                    byte[] respBytes = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(response));
                    stream.Write(BitConverter.GetBytes(respBytes.Length), 0, 4);
                    stream.Write(respBytes, 0, respBytes.Length);
                }
            }
            catch (Exception e)
            {
                if (listener == null) break;
                Debug.LogError(e);
            }
        }
    }

    private static void ProcessPendingCommand()
    {
        if (!pendingCommand) return;
        commandResult = HandleCommand(pendingCommandName, pendingCommandData);
        pendingCommand = false;
    }

    private static string HandleCommand(string command, CommData data)
    {
        if (command == "get_project_path")
        {
            return Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        }
        else if (command == "get_addressable_path")
        {
            string assetPath = NormalizeAssetPath(data.file_path);
            Debug.Log($"[get_addressable_path] assetPath: {assetPath}");
            return ResolveAddressableAddress(assetPath);
        }
        else if (command == "get_sprite_info")
        {
            string filePath = data.file_path;
            string assetPath = filePath.Replace(@"\", "/");
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..")).Replace(@"\", "/").TrimEnd('/');
            if (assetPath.StartsWith(projectRoot, StringComparison.OrdinalIgnoreCase))
            {
                assetPath = assetPath.Substring(projectRoot.Length).TrimStart('/');
            }
            if (!assetPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                assetPath = "Assets/" + assetPath.TrimStart('/');
            }

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null || importer.spriteImportMode != SpriteImportMode.Multiple)
            {
                return "[]";
            }

            var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            var sprites = new List<Sprite>();  // Sprite型でリストにする
            foreach (var obj in assets)
            {
                if (obj is Sprite sprite)
                {
                    sprites.Add(sprite);
                }
            }

            // 名前から数値部分を抽出してソート（例: "sprite_10" の "10" をintに変換）
            sprites.Sort((a, b) =>
            {
                int numA = int.Parse(a.name.Split('_').Last());  // 名前が "sprite_数字" 形式の場合
                int numB = int.Parse(b.name.Split('_').Last());
                return numA.CompareTo(numB);
            });

            // 名前リストにする場合
            var spriteNames = sprites.Select(s => s.name).ToList();

            return JsonUtility.ToJson(new Wrapper<string> { items = spriteNames });
        }
        else if (command == "get_animator_controller_info")
        {
            string filePath = data.file_path;
            string assetPath = NormalizeAssetPath(filePath);

            var controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(assetPath);
            if (controller == null) return "[]";
            
            var info = new AnimatorFullInfo
            {
                parameters = controller.parameters.Select(p => new ParamInfo
                {
                    name = p.name,
                    type = p.type.ToString(),
                    defaultFloat = p.defaultFloat,
                    defaultInt = p.defaultInt,
                    defaultBool = p.defaultBool
                }).ToList(),


                layers = controller.layers.Select((l,i) => new LayerFullInfo
                {
                    name = l.name ?? "BaseLayer",
                    index = i,
                    states = GetAllStatesInLayer(l.stateMachine).ToList()
                }).ToList()
            };

            return JsonUtility.ToJson(info);
        }
        else if (command == "get_material_properties")
        {
            string filePath = data.file_path;
            string assetPath = NormalizeAssetPath(filePath);

            Shader shader = null;
            string ext = Path.GetExtension(assetPath).ToLowerInvariant();
            if (ext == ".mat")
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (mat != null) shader = mat.shader;
            }
            else if (ext == ".shader" || ext == ".shadergraph")
            {
                // .shadergraph はインポート後に通常のShaderアセットとして
                // AssetDatabaseから読み込める（ShaderGraphImporterを直接使う必要はない）
                shader = AssetDatabase.LoadAssetAtPath<Shader>(assetPath);
            }

            if (shader == null) return "{}";

            // 【重要】UnityEditor.ShaderUtil（旧型式・非推奨寄りのエディタ専用API）は使わない。
            // 代わりにUnityEngine.Shaderのインスタンスメソッド（Runtime/Editor共通、
            // ShaderGraph生成のShaderにも同じように使える）でプロパティを列挙する。
            var props = new List<ShaderPropertyInfo>();
            int propCount = shader.GetPropertyCount();
            for (int i = 0; i < propCount; i++)
            {
                var flags = shader.GetPropertyFlags(i);
                if ((flags & UnityEngine.Rendering.ShaderPropertyFlags.HideInInspector) != 0) continue;

                props.Add(new ShaderPropertyInfo
                {
                    name = shader.GetPropertyName(i),
                    type = shader.GetPropertyType(i).ToString()
                });
            }

            var materialResult = new MaterialPropertiesResult
            {
                items = props
            };

            return JsonUtility.ToJson(materialResult);
        }
        return null;
    }

    // assetPath（"Assets/..."形式、正規化済み）からAddressableのアドレスを取得する。
    // Addressableでなければ assetPath をそのまま返す。
    // get_addressable_path / get_material_properties など、Addressableパスが必要な
    // すべてのコマンドはこのメソッドだけを経由する（duplicate実装の禁止・唯一の実装元）。
    private static string ResolveAddressableAddress(string assetPath)
    {
        if (string.IsNullOrEmpty(assetPath)) return assetPath;

        string guid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(guid) || guid == "00000000000000000000000000000000")
        {
            Debug.LogWarning($"No valid GUID found for assetPath: {assetPath}");
            return assetPath;
        }

        AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null)
        {
            Debug.LogWarning("AddressableAssetSettings is not initialized.");
            return assetPath;
        }

        var entry = settings.FindAssetEntry(guid);
        if (entry == null)
        {
            Debug.LogWarning($"Asset not Addressable: {assetPath}. Returning relative path.");
            return assetPath;
        }
        return entry.address;
    }

    [System.Serializable]
    private class ShaderPropertyInfo
    {
        public string name;
        public string type;
    }

    [System.Serializable]
    private class MaterialPropertiesResult
    {
        public List<ShaderPropertyInfo> items;
    }

    [System.Serializable]
    private class CommMessage
    {
        public string command;
        public CommData data;
        public string result;
    }

    [System.Serializable]
    private class CommData
    {
        public string file_path;
    }

    private static string NormalizeAssetPath(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return "";

        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."))
            .Replace(@"\", "/").TrimEnd('/');

        string normalized = filePath.Replace(@"\", "/").Trim();

        if (normalized.StartsWith(projectRoot, StringComparison.OrdinalIgnoreCase))
        {
            normalized = normalized.Substring(projectRoot.Length).TrimStart('/');
        }

        if (!normalized.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
        {
            normalized = "Assets/" + normalized.TrimStart('/');
        }

        return normalized;
    }

    private static IEnumerable<StateFullInfo> GetAllStatesInLayer(AnimatorStateMachine sm)
    {
        foreach (var child in sm.stateMachines)
            foreach (var s in GetAllStatesInLayer(child.stateMachine))
                yield return s;

        foreach (var childState in sm.states)
        {
            var state = childState.state;
            var stateInfo = new StateFullInfo
            {
                name = state.name,
                isBlendTree = state.motion is UnityEditor.Animations.BlendTree,
                blendTree = state.motion is UnityEditor.Animations.BlendTree bt ? GetBlendTreeInfo(bt) : null,
                motions = state.motion is UnityEditor.Animations.BlendTree ? null : new List<string> { state.motion ? state.motion.name : "None" }
            };
            yield return stateInfo;
        }
    }

    private static BlendTreeInfo GetBlendTreeInfo(UnityEditor.Animations.BlendTree bt)
    {
        return new BlendTreeInfo
        {
            blendType = bt.blendType.ToString(),
            blendParameter = bt.blendParameter,
            blendParameterY = bt.blendParameterY,
            children = bt.children.Select(c => new BlendTreeChildInfo
            {
                motionName = c.motion ? c.motion.name : "None",
                threshold = c.threshold,
                timeScale = c.timeScale,
                directBlendParameter = c.directBlendParameter
            }).ToList()
        };
    }

    // シリアライズ用クラス
    [Serializable]
    private class AnimatorFullInfo
    {
        public List<ParamInfo> parameters = new List<ParamInfo>();
        public List<LayerFullInfo> layers = new List<LayerFullInfo>();
    }
    [Serializable]
    private class ParamInfo
    {
        public string name;
        public string type;         // "Float", "Int", "Bool", "Trigger"
        public float defaultFloat;
        public int defaultInt;
        public bool defaultBool;
    }
    [Serializable] private class LayerFullInfo { public string name; public int index; public List<StateFullInfo> states; }
    [Serializable] private class StateFullInfo { public string name; public bool isBlendTree; public List<string> motions; public BlendTreeInfo blendTree; }
    [Serializable] private class BlendTreeInfo { public string blendType; public string blendParameter; public string blendParameterY; public List<BlendTreeChildInfo> children; }
    [Serializable] private class BlendTreeChildInfo { public string motionName; public float threshold; public float timeScale; public string directBlendParameter; }
}
