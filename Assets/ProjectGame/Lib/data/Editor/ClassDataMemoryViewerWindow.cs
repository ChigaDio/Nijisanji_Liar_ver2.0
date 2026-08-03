using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameCore.Tables.Editor
{
    /// <summary>
    /// class_data_id / class_data_matrix_id の各テーブルについて、
    /// ロード済みid数と概算メモリサイズ(バイナリ上のバイト数の合計)を一覧表示するエディタウィンドウ。
    /// </summary>
    public class ClassDataMemoryViewerWindow : EditorWindow
    {
        private class TableInfo
        {
            public string Name;
            public int LoadedCount;
            public int TotalCount;
            public long MemoryBytes;
        }

        private static readonly Color HeaderColor = new Color(0.14f, 0.16f, 0.21f);
        private static readonly Color AccentColor = new Color(0.30f, 0.62f, 0.98f);
        private static readonly Color EmptyBarColor = new Color(0f, 0f, 0f, 0.25f);
        private static readonly Color RowColorA = new Color(1f, 1f, 1f, 0.02f);
        private static readonly Color RowColorB = new Color(1f, 1f, 1f, 0.06f);

        private Vector2 scroll;
        private string search = "";
        private bool showId = true;
        private bool showMatrix = true;
        private bool autoRefresh = true;
        private double lastRefreshTime;

        private List<TableInfo> idTables = new List<TableInfo>();
        private List<TableInfo> matrixTables = new List<TableInfo>();

        [MenuItem("GameCore/Class Data Memory Viewer")]
        public static void Open()
        {
            var window = GetWindow<ClassDataMemoryViewerWindow>("Class Data Memory");
            window.minSize = new Vector2(440, 340);
            window.Refresh();
        }

        private void OnEnable() => Refresh();

        private void OnGUI()
        {
            DrawToolbar();
            EditorGUILayout.Space(4);
            DrawSummary();
            EditorGUILayout.Space(4);

            scroll = EditorGUILayout.BeginScrollView(scroll);
            showId = DrawSection("ID テーブル", showId, idTables);
            EditorGUILayout.Space(8);
            showMatrix = DrawSection("Matrix テーブル", showMatrix, matrixTables);
            EditorGUILayout.EndScrollView();

            if (autoRefresh && EditorApplication.timeSinceStartup - lastRefreshTime > 1.0)
            {
                Refresh();
            }
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            search = EditorGUILayout.TextField(search, EditorStyles.toolbarSearchField, GUILayout.MinWidth(140));
            if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(64))) Refresh();
            autoRefresh = GUILayout.Toggle(autoRefresh, "Auto", EditorStyles.toolbarButton, GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSummary()
        {
            long idBytes = idTables.Sum(t => t.MemoryBytes);
            long matrixBytes = matrixTables.Sum(t => t.MemoryBytes);

            var rect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(rect, HeaderColor);
            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);
            var boldStyle = new GUIStyle(EditorStyles.boldLabel) { normal = { textColor = Color.white } };
            GUILayout.Label("ID: " + FormatBytes(idBytes), boldStyle);
            GUILayout.Space(12);
            GUILayout.Label("Matrix: " + FormatBytes(matrixBytes), boldStyle);
            GUILayout.FlexibleSpace();
            var totalStyle = new GUIStyle(boldStyle) { normal = { textColor = AccentColor } };
            GUILayout.Label("Total: " + FormatBytes(idBytes + matrixBytes), totalStyle);
            GUILayout.Space(8);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);
            EditorGUILayout.EndVertical();
        }

        private bool DrawSection(string title, bool expanded, List<TableInfo> tables)
        {
            expanded = EditorGUILayout.Foldout(expanded, title + " (" + tables.Count + ")", true);
            if (!expanded) return expanded;

            int i = 0;
            foreach (var info in tables)
            {
                if (!string.IsNullOrEmpty(search) && info.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) < 0)
                {
                    continue;
                }

                var rowRect = EditorGUILayout.BeginHorizontal(GUILayout.Height(20));
                EditorGUI.DrawRect(rowRect, (i % 2 == 0) ? RowColorA : RowColorB);

                GUILayout.Label(info.Name, GUILayout.Width(180));

                float ratio = info.TotalCount > 0 ? (float)info.LoadedCount / info.TotalCount : 0f;
                var barRect = GUILayoutUtility.GetRect(80, 16, GUILayout.ExpandWidth(true));
                DrawProgressBar(barRect, ratio, info.LoadedCount + " / " + info.TotalCount);

                GUILayout.Label(FormatBytes(info.MemoryBytes), GUILayout.Width(80));

                EditorGUILayout.EndHorizontal();
                i++;
            }

            if (tables.Count == 0)
            {
                EditorGUILayout.HelpBox("テーブルが見つかりませんでした。", MessageType.Info);
            }

            return expanded;
        }

        private void DrawProgressBar(Rect rect, float value, string label)
        {
            EditorGUI.DrawRect(rect, EmptyBarColor);
            var fillRect = new Rect(rect.x, rect.y, rect.width * Mathf.Clamp01(value), rect.height);
            Color fillColor = value <= 0f ? new Color(0.4f, 0.4f, 0.4f) : Color.Lerp(new Color(0.85f, 0.35f, 0.35f), AccentColor, value);
            EditorGUI.DrawRect(fillRect, fillColor);

            var style = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
            style.normal.textColor = Color.white;
            GUI.Label(rect, label, style);
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024) return bytes + " B";
            double kb = bytes / 1024.0;
            if (kb < 1024) return kb.ToString("0.0") + " KB";
            double mb = kb / 1024.0;
            return mb.ToString("0.00") + " MB";
        }

        private void Refresh()
        {
            lastRefreshTime = EditorApplication.timeSinceStartup;
            idTables.Clear();
            matrixTables.Clear();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = asm.GetTypes(); }
                catch { continue; }

                foreach (var type in types)
                {
                    if (type.IsAbstract || type.BaseType == null || !type.BaseType.IsGenericType) continue;

                    string baseName = type.BaseType.GetGenericTypeDefinition().Name;
                    if (baseName.StartsWith("BaseClassDataID"))
                    {
                        var info = BuildIdTableInfo(type);
                        if (info != null) idTables.Add(info);
                    }
                    else if (baseName.StartsWith("BaseClassDataMatrixID"))
                    {
                        var info = BuildMatrixTableInfo(type);
                        if (info != null) matrixTables.Add(info);
                    }
                }
            }

            idTables = idTables.OrderByDescending(t => t.MemoryBytes).ToList();
            matrixTables = matrixTables.OrderByDescending(t => t.MemoryBytes).ToList();
            Repaint();
        }

        private static long GetTupleItem2(object tuple)
        {
            if (tuple == null) return 0;
            var field = tuple.GetType().GetField("Item2");
            return field != null ? Convert.ToInt64(field.GetValue(tuple)) : 0;
        }

        private TableInfo BuildIdTableInfo(Type type)
        {
            var tableField = type.BaseType.GetField("Table", BindingFlags.Public | BindingFlags.Static);
            var rowIndexField = type.BaseType.GetField("RowIndex", BindingFlags.NonPublic | BindingFlags.Static);
            if (tableField == null) return null;

            var table = tableField.GetValue(null) as System.Collections.IDictionary;
            if (table == null) return null;

            int totalCount = 0;
            long memory = 0;
            var rowIndexObj = rowIndexField?.GetValue(null);
            var entries = GetEntriesDictionary(rowIndexObj);
            if (entries != null)
            {
                totalCount = entries.Count;
                foreach (var key in table.Keys)
                {
                    if (entries.Contains(key)) memory += GetTupleItem2(entries[key]);
                }
            }

            return new TableInfo { Name = type.Name, LoadedCount = table.Count, TotalCount = totalCount, MemoryBytes = memory };
        }

        private TableInfo BuildMatrixTableInfo(Type type)
        {
            var tableField = type.BaseType.GetField("Table", BindingFlags.Public | BindingFlags.Static);
            var rowIndexField = type.BaseType.GetField("RowIndex", BindingFlags.NonPublic | BindingFlags.Static);
            var cellIndexCacheField = type.BaseType.GetField("s_cellIndexCache", BindingFlags.NonPublic | BindingFlags.Static);
            if (tableField == null) return null;

            var table = tableField.GetValue(null) as System.Collections.IDictionary;
            if (table == null) return null;

            var cellIndexCache = cellIndexCacheField?.GetValue(null) as System.Collections.IDictionary;
            long memory = 0;

            foreach (var rowKey in table.Keys)
            {
                var rowDict = table[rowKey] as System.Collections.IDictionary;
                if (rowDict == null) continue;

                System.Collections.IDictionary cellIndex = null;
                if (cellIndexCache != null && cellIndexCache.Contains(rowKey))
                {
                    cellIndex = cellIndexCache[rowKey] as System.Collections.IDictionary;
                }

                foreach (var colKey in rowDict.Keys)
                {
                    if (cellIndex != null && cellIndex.Contains(colKey))
                    {
                        memory += GetTupleItem2(cellIndex[colKey]);
                    }
                }
            }

            int totalRowCount = 0;
            var rowIndexObj = rowIndexField?.GetValue(null);
            var entries = GetEntriesDictionary(rowIndexObj);
            if (entries != null) totalRowCount = entries.Count;

            return new TableInfo { Name = type.Name, LoadedCount = table.Count, TotalCount = totalRowCount, MemoryBytes = memory };
        }

        private static System.Collections.IDictionary GetEntriesDictionary(object rowIndexObj)
        {
            if (rowIndexObj == null) return null;
            var entriesField = rowIndexObj.GetType().GetField("Entries", BindingFlags.Public | BindingFlags.Instance);
            return entriesField?.GetValue(rowIndexObj) as System.Collections.IDictionary;
        }
    }
}
