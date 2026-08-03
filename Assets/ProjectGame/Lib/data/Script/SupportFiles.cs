using UnityEngine;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameCore
{
    /// <summary>
    /// サポートデータ内の代表的なファイルを一箇所で定義して取得できるヘルパー。
    /// ALL_SOUND_BIN など、呼び出し側はその名前だけ参照すればフルパスが返る。
    /// </summary>
    public static class SupportFiles
    {
        public const string SUPPORT_ROOT_NAME = "SupportChigadio";
        public const string SUPPORT_DATA_NAME = "data";

        // data直下のフォルダ
        public const string ASSETS_FOLDER = "assets-data";
        public const string SOUND_FOLDER = "sound";
        public const string TEXTURE_FOLDER = "texture";
        public const string GAMEOBJECT_FOLDER = "gameobject";
        public const string MATERIAL_FOLDER = "material";


        //dataID
        public const string ID_FOLDER = "class_data_id";
        public const string ID_BIN_FILE = "all_class_data.bytes";

        //matrixID
        public const string MATRIX_DATA_ID_FOLDER = "class_data_matrix_id";
        public const string MATRIX_ID_BIN_FILE = "all_class_data_matrix.bytes";

        // ファイル名（ここだけ定義すればOK）
        public const string ALL_SOUND_BIN_FILE = "sound_data.bytes";
        public const string ALL_TEXTURE_BIN_FILE = "texture_data.bytes";
        public const string ALL_GAMEOBJECT_BIN_FILE = "gameobject_data.bytes";
        public const string ALL_MATERIAL_BIN_FILE = "material_data.bytes";

        //Scenario
        public const string SCENARIO_FOLDER = "scenario_data";
        public const string SCENARIO_EVEMT_FOLDER = "scenario_event_data";
        public const string ALL_SCENARIO_EVENT_BIN_FILE = "all_events.bytes";
        
        //CustomClassDataID
        public const string CUSTOM_CLASS_DATA_FOLDER = "custom_class_data_id";
        public const string CUSTOM_CLASS_DATA_ID_BIN_FILE = "all_custom_class_data_id.bytes";

        // キャッシュ（最初に解決したパスを保持）
        public static string s_cachedSupportDataPath = null;

        /// <summary>
        /// SupportChigadio/data のフルパスを取得（キャッシュあり／EditorではAssetDatabaseを試行）
        /// </summary>
        private static string SupportDataPath
        {
            get
            {
                if (!string.IsNullOrEmpty(s_cachedSupportDataPath)) return s_cachedSupportDataPath;

#if UNITY_EDITOR
                // EditorならAssetDatabaseでまず探す（ただしメインスレッドでないと例外になる可能性があるので try/catch）
                try
                {
                    string assetsRelative = FindFolderPathByAssetDatabase(SUPPORT_ROOT_NAME); // "Assets/..."
                    if (!string.IsNullOrEmpty(assetsRelative))
                    {
                        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                        string absoluteSupportRoot = Path.GetFullPath(Path.Combine(projectRoot, assetsRelative)); // -> .../Project/Assets/.../SupportChigadio
                        string dataPath = Path.Combine(absoluteSupportRoot, "..", SUPPORT_DATA_NAME);
                        s_cachedSupportDataPath = Path.GetFullPath(dataPath).Replace("\\", "/");
                        return s_cachedSupportDataPath;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"AssetDatabase lookup failed (maybe called from background thread): {e.Message}. Falling back to filesystem.");
                }
#endif
                // ファイルシステム上での候補（projectRoot/SupportChigadio/data）
                string projectRootFs = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
                string candidate = Path.Combine(projectRootFs, SUPPORT_ROOT_NAME, SUPPORT_DATA_NAME);
                if (Directory.Exists(candidate))
                {
                    s_cachedSupportDataPath = Path.GetFullPath(candidate).Replace("\\", "/");
                    return s_cachedSupportDataPath;
                }

                // それでも見つからなければプロジェクト内を検索（重い可能性あり）
                try
                {
                    var dirs = Directory.GetDirectories(projectRootFs, SUPPORT_ROOT_NAME, SearchOption.AllDirectories);
                    if (dirs != null && dirs.Length > 0)
                    {
                        string found = dirs[0];
                        string dataPath = Path.Combine(found, SUPPORT_DATA_NAME);
                        s_cachedSupportDataPath = Path.GetFullPath(dataPath).Replace("\\", "/");
                        return s_cachedSupportDataPath;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Fallback search failed: {ex.Message}");
                }

                // 最後の最終手段：Project直下の GameData を使う
                string fallback = Path.Combine(projectRootFs, "GameData");
                s_cachedSupportDataPath = Path.GetFullPath(fallback).Replace("\\", "/");
                return s_cachedSupportDataPath;
            }
        }

        /// <summary>
        /// これだけ参照すれば all_sound.bytes のフルパスが得られる（呼び出し側はこれだけ見れば良い）
        /// </summary>
        public static string ALL_SOUND_BIN => Path.GetFullPath(Path.Combine(SupportDataPath, ASSETS_FOLDER, SOUND_FOLDER, ALL_SOUND_BIN_FILE)).Replace("\\", "/");
        public static string ALL_TEXTURE_BIN => Path.GetFullPath(Path.Combine(SupportDataPath, ASSETS_FOLDER, TEXTURE_FOLDER, ALL_TEXTURE_BIN_FILE)).Replace("\\", "/");
        public static string ALL_GAMEOBJECT_BIN => Path.GetFullPath(Path.Combine(SupportDataPath, ASSETS_FOLDER, GAMEOBJECT_FOLDER, ALL_GAMEOBJECT_BIN_FILE)).Replace("\\", "/");
        public static string ALL_MATERIAL_BIN => Path.GetFullPath(Path.Combine(SupportDataPath, ASSETS_FOLDER, MATERIAL_FOLDER, ALL_MATERIAL_BIN_FILE)).Replace("\\\\", "/");

        public static string ALL_MATRIX_ID_BIN => Path.GetFullPath(Path.Combine(SupportDataPath, MATRIX_DATA_ID_FOLDER, MATRIX_ID_BIN_FILE)).Replace("\\", "/");
        public static string ALL_ID_BIN => Path.GetFullPath(Path.Combine(SupportDataPath, ID_FOLDER, ID_BIN_FILE)).Replace("\\", "/");
        public static string ALL_CUSTOM_CLASS_DATA_ID_BIN => Path.GetFullPath(Path.Combine(SupportDataPath, CUSTOM_CLASS_DATA_FOLDER, CUSTOM_CLASS_DATA_ID_BIN_FILE)).Replace("\\", "/");
        public static string ALL_SCENARIO_EVENTS_BIN => Path.GetFullPath(Path.Combine(SupportDataPath, SCENARIO_FOLDER, SCENARIO_EVEMT_FOLDER, ALL_SCENARIO_EVENT_BIN_FILE)).Replace("\\", "/");

#if UNITY_EDITOR
        // Editor専用：AssetDatabaseで探して "Assets/..." を返す（失敗すれば null）
        private static string FindFolderPathByAssetDatabase(string folderName)
        {
            string[] guids = AssetDatabase.FindAssets("t:folder " + folderName, new[] { "Assets" });
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid); // "Assets/...."
                if (AssetDatabase.IsValidFolder(path) && Path.GetFileName(path) == folderName)
                    return path;
            }
            return null;
        }
#endif

        /// <summary>
        /// 補助：絶対パスがプロジェクト内（Projectルート）に含まれるなら "Assets/..." 相対パスを返す。AssetDatabase系APIに渡したいときに使える。
        /// </summary>
        public static string GetAssetRelativePath(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath)) return null;
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..")).Replace("\\", "/");
            absolutePath = Path.GetFullPath(absolutePath).Replace("\\", "/");
            if (absolutePath.StartsWith(projectRoot, StringComparison.OrdinalIgnoreCase))
            {
                string rel = absolutePath.Substring(projectRoot.Length).TrimStart('/', '\\');
                return rel;
            }
            return null;
        }

        /// <summary>
        /// 存在確認のショートカット
        /// </summary>
        public static bool ALL_SOUND_BIN_Exists => File.Exists(ALL_SOUND_BIN);
        
                /// <summary>
        /// Addressableのチェック
        /// </summary>
        public static bool ADDRESSABLE_CHECK = true;
    }
}
