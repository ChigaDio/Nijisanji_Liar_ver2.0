#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace AddressableSystem.EditorTools
{
    /// <summary>
    /// 現在ロード中のAddressable（Playモード時）と、プロジェクト内のAddressableグループ構成を
    /// 一覧できるエディタウィンドウ。
    /// Tools/Addressable/Addressable Manager から開く。
    /// </summary>
    public class AddressableManagerWindow : EditorWindow
    {
        private enum Tab { Runtime, Project }

        private Tab currentTab = Tab.Runtime;
        private Vector2 scrollPos;
        private string searchText = string.Empty;
        private bool autoRefresh = true;
        private double lastRefreshTime;

        private readonly Dictionary<GroupCategory, bool> groupFoldouts = new Dictionary<GroupCategory, bool>();
        private readonly Dictionary<string, bool> projectGroupFoldouts = new Dictionary<string, bool>();

        private static readonly Color HeaderColor = new Color(0.14f, 0.15f, 0.18f);
        private static readonly Color AccentColor = new Color(0.32f, 0.63f, 1.00f);
        private static readonly Color LoadedColor = new Color(0.35f, 0.80f, 0.45f);
        private static readonly Color PendingColor = new Color(0.95f, 0.65f, 0.25f);
        private static readonly Color MutedColor = new Color(0.62f, 0.62f, 0.66f);

        private GUIStyle _titleStyle;
        private GUIStyle _subtitleStyle;
        private GUIStyle _cardStyle;
        private GUIStyle _statValueStyle;
        private GUIStyle _pathStyle;
        private GUIStyle _tinyBoldStyle;
        private bool _stylesReady;

        [MenuItem("Tools/Addressable/Addressable Manager")]
        public static void Open()
        {
            var window = GetWindow<AddressableManagerWindow>();
            window.titleContent = new GUIContent("Addressable Manager");
            window.minSize = new Vector2(500, 420);
            window.Show();
        }

        private void OnEnable()
        {
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            if (!autoRefresh || !Application.isPlaying) return;
            if (EditorApplication.timeSinceStartup - lastRefreshTime < 0.5d) return;
            lastRefreshTime = EditorApplication.timeSinceStartup;
            Repaint();
        }

        private void EnsureStyles()
        {
            if (_stylesReady) return;
            _stylesReady = true;

            _titleStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 15 };
            _titleStyle.normal.textColor = Color.white;

            _subtitleStyle = new GUIStyle(EditorStyles.miniLabel);
            _subtitleStyle.normal.textColor = MutedColor;

            _cardStyle = new GUIStyle(EditorStyles.helpBox);
            _cardStyle.padding = new RectOffset(10, 10, 8, 8);
            _cardStyle.margin = new RectOffset(2, 2, 4, 4);

            _statValueStyle = new GUIStyle(EditorStyles.boldLabel) { fontSize = 20 };

            _pathStyle = new GUIStyle(EditorStyles.label);
            _pathStyle.wordWrap = false;
            _pathStyle.fontSize = 11;

            _tinyBoldStyle = new GUIStyle(EditorStyles.miniBoldLabel);
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawHeader();
            DrawToolbar();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.Space(6);

            if (currentTab == Tab.Runtime)
            {
                DrawRuntimeTab();
            }
            else
            {
                DrawProjectTab();
            }

            EditorGUILayout.Space(12);
            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            Rect rect = GUILayoutUtility.GetRect(position.width, 44);
            EditorGUI.DrawRect(rect, HeaderColor);

            var accentRect = new Rect(rect.x, rect.y + rect.height - 3, rect.width, 3);
            EditorGUI.DrawRect(accentRect, AccentColor);

            var titleRect = new Rect(rect.x + 12, rect.y + 4, rect.width - 24, 22);
            EditorGUI.LabelField(titleRect, "Addressable Manager", _titleStyle);

            string subtitle = Application.isPlaying
                ? "Play Mode - runtime load state"
                : "Edit Mode - project group configuration";
            var subRect = new Rect(rect.x + 12, rect.y + 24, rect.width - 24, 16);
            EditorGUI.LabelField(subRect, subtitle, _subtitleStyle);
        }

        private void DrawToolbar()
        {
            EditorGUILayout.Space(4);
            currentTab = (Tab)GUILayout.Toolbar((int)currentTab, new[] { "Runtime", "Project Groups" }, GUILayout.Height(22));

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Search", GUILayout.Width(46));
            searchText = EditorGUILayout.TextField(searchText);
            if (GUILayout.Button("Refresh", GUILayout.Width(64)))
            {
                Repaint();
            }
            autoRefresh = GUILayout.Toggle(autoRefresh, "Auto", "Button", GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
        }

        // ------------------------------------------------------------
        // Runtime tab
        // ------------------------------------------------------------
        private void DrawRuntimeTab()
        {
            if (!Application.isPlaying)
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.HelpBox(
                    "Playモード中に AddressableDataCore が管理しているロード状況をここに表示します。\n" +
                    "Playを開始すると自動的に更新されます。",
                    MessageType.Info);
                return;
            }

            AddressableDataCore core = AddressableDataCore.Instance;
            Dictionary<GroupCategory, Dictionary<AssetCategory, List<BaseAddressableData>>> entries = core.GetAllEntries();

            if (!string.IsNullOrEmpty(searchText))
            {
                entries = entries.ToDictionary(
                    g => g.Key,
                    g => g.Value.ToDictionary(
                        c => c.Key,
                        c => c.Value.Where(e => e.path != null && e.path.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0).ToList()
                    )
                );
            }

            int total = entries.Count;
            int loaded = entries.Sum(g => g.Value.Sum(c => c.Value.Count(e => e.IsLoadedAndSetup)));
            DrawSummaryCard(total, loaded);

            EditorGUILayout.Space(6);

            List<GroupCategory> groups = entries.Keys.OrderBy(g => g.ToString()).ToList();

            if (groups.Count == 0)
            {
                EditorGUILayout.HelpBox("現在ロード中のAddressableはありません。", MessageType.Info);
                return;
            }

            foreach (GroupCategory group in groups)
            {
                List<BaseAddressableData> groupEntries = entries[group].SelectMany(c => c.Value).ToList();
                DrawGroupCard(group, groupEntries, core);
            }
        }

        private void DrawSummaryCard(int total, int loaded)
        {
            EditorGUILayout.BeginVertical(_cardStyle);
            EditorGUILayout.BeginHorizontal();
            DrawStat("Total", total.ToString(), AccentColor);
            DrawStat("Loaded", loaded.ToString(), LoadedColor);
            DrawStat("Pending", (total - loaded).ToString(), PendingColor);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawStat(string label, string value, Color color)
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(100));
            GUIStyle style = new GUIStyle(_statValueStyle);
            style.normal.textColor = color;
            GUILayout.Label(value, style);
            GUILayout.Label(label, _subtitleStyle);
            EditorGUILayout.EndVertical();
        }

        private void DrawGroupCard(GroupCategory group, List<BaseAddressableData> entries, AddressableDataCore core)
        {
            if (!groupFoldouts.ContainsKey(group))
            {
                groupFoldouts[group] = true;
            }

            EditorGUILayout.BeginVertical(_cardStyle);

            EditorGUILayout.BeginHorizontal();
            groupFoldouts[group] = EditorGUILayout.Foldout(groupFoldouts[group], group + "  (" + entries.Count + ")", true);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Release", GUILayout.Width(70)))
            {
                if (EditorUtility.DisplayDialog("Release Group", group + " グループを全て解放しますか？", "Release", "Cancel"))
                {
                    core.ReleaseGroup(group);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (groupFoldouts[group])
            {
                IEnumerable<AssetCategory> categories = entries.Select(e => e.assetCategory).Distinct().OrderBy(c => c.ToString());
                foreach (AssetCategory category in categories)
                {
                    List<BaseAddressableData> categoryEntries = entries.Where(e => e.assetCategory == category).ToList();
                    DrawCategorySection(group, category, categoryEntries, core);
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawCategorySection(GroupCategory group, AssetCategory category, List<BaseAddressableData> entries, AddressableDataCore core)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("- " + category, _tinyBoldStyle, GUILayout.Width(140));
            int loadedCount = entries.Count(e => e.IsLoadedAndSetup);
            GUILayout.Label(loadedCount + "/" + entries.Count + " loaded", _subtitleStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Release", GUILayout.Width(70)))
            {
                if (EditorUtility.DisplayDialog("Release Category", group + " / " + category + " を解放しますか？", "Release", "Cancel"))
                {
                    core.ReleaseCategory(group, category);
                }
            }
            EditorGUILayout.EndHorizontal();

            foreach (BaseAddressableData entry in entries)
            {
                DrawEntryRow(entry);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(2);
        }

        private void DrawEntryRow(BaseAddressableData entry)
        {
            EditorGUILayout.BeginHorizontal();

            Color statusColor = entry.IsLoadedAndSetup ? LoadedColor : PendingColor;
            Color prevColor = GUI.color;
            GUI.color = statusColor;
            GUILayout.Label("*", GUILayout.Width(14));
            GUI.color = prevColor;

            string label = string.IsNullOrEmpty(entry.path) ? "(no path)" : entry.path;
            GUILayout.Label(label, _pathStyle);

            GUILayout.FlexibleSpace();

            if (entry.IsArray)
            {
                GUILayout.Label("x" + entry.GetArrayCount(), _subtitleStyle, GUILayout.Width(36));
            }
            if (entry.IsAutoRelease)
            {
                GUILayout.Label("auto", _subtitleStyle, GUILayout.Width(34));
            }

            EditorGUILayout.EndHorizontal();
        }

        // ------------------------------------------------------------
        // Project tab（Editモードでも見られる、プロジェクト内のAddressableグループ構成）
        // ------------------------------------------------------------
        private void DrawProjectTab()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings == null)
            {
                EditorGUILayout.HelpBox(
                    "Addressable Asset Settings が見つかりません。\n" +
                    "Window > Asset Management > Addressables > Groups からセットアップしてください。",
                    MessageType.Warning);
                return;
            }

            List<AddressableAssetGroup> groups = settings.groups.Where(g => g != null).ToList();

            EditorGUILayout.BeginVertical(_cardStyle);
            EditorGUILayout.BeginHorizontal();
            DrawStat("Groups", groups.Count.ToString(), AccentColor);
            DrawStat("Entries", groups.Sum(g => g.entries.Count).ToString(), LoadedColor);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(6);

            foreach (AddressableAssetGroup group in groups)
            {
                DrawProjectGroupCard(group, group == settings.DefaultGroup);
            }
        }

        private void DrawProjectGroupCard(AddressableAssetGroup group, bool isDefault)
        {
            if (!projectGroupFoldouts.ContainsKey(group.Name))
            {
                projectGroupFoldouts[group.Name] = false;
            }

            List<AddressableAssetEntry> entries = group.entries.ToList();
            if (!string.IsNullOrEmpty(searchText))
            {
                entries = entries
                    .Where(e => e.address != null && e.address.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                if (entries.Count == 0) return;
            }

            EditorGUILayout.BeginVertical(_cardStyle);
            EditorGUILayout.BeginHorizontal();
            projectGroupFoldouts[group.Name] = EditorGUILayout.Foldout(
                projectGroupFoldouts[group.Name], group.Name + "  (" + entries.Count + ")", true);
            GUILayout.FlexibleSpace();
            if (isDefault)
            {
                GUILayout.Label("Default", _subtitleStyle, GUILayout.Width(50));
            }
            if (GUILayout.Button("Select", GUILayout.Width(60)))
            {
                Selection.activeObject = group;
                EditorGUIUtility.PingObject(group);
            }
            EditorGUILayout.EndHorizontal();

            if (projectGroupFoldouts[group.Name])
            {
                EditorGUI.indentLevel++;
                foreach (AddressableAssetEntry entry in entries)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label(entry.address, _pathStyle);
                    GUILayout.FlexibleSpace();
                    string typeName = entry.MainAssetType != null ? entry.MainAssetType.Name : "?";
                    GUILayout.Label(typeName, _subtitleStyle, GUILayout.Width(90));
                    if (GUILayout.Button("Ping", GUILayout.Width(44)))
                    {
                        UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(entry.AssetPath);
                        if (asset != null)
                        {
                            Selection.activeObject = asset;
                            EditorGUIUtility.PingObject(asset);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();
        }
    }
}
#endif