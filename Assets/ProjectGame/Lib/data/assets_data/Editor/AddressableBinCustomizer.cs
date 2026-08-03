
        using UnityEngine;
using System.Collections.Generic;
using GameCore;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using System.IO;

[InitializeOnLoad]   // ← 必須
public class AddressableBinCustomizer
{
    private static bool _isProcessing = false;

    static AddressableBinCustomizer()
    {
        AddressableAssetSettings.OnModificationGlobal += OnAddressableModification;
        Debug.Log("[Addressable Bin] カスタムAddress自動設定スクリプトが起動しました");
    }

    private static void OnAddressableModification(AddressableAssetSettings settings,
        AddressableAssetSettings.ModificationEvent e, object obj)
    {
        if (_isProcessing) return;

        // EntryCreated か EntryModified のときだけ処理
        if (e != AddressableAssetSettings.ModificationEvent.EntryCreated &&
            e != AddressableAssetSettings.ModificationEvent.EntryModified &&
            e != AddressableAssetSettings.ModificationEvent.EntryAdded)
            return;

        // ★★★ ここを修正：objが配列の場合も単体のEntryの場合も両方対応 ★★★
        ProcessEntries(settings, obj);
    }

    private static void ProcessEntries(AddressableAssetSettings settings, object data)
    {
        // 1. 単体のEntryの場合
        if (data is AddressableAssetEntry singleEntry)
        {
            ProcessSingleEntry(settings, singleEntry);
            return;
        }

        // 2. 配列（object[]）の場合 ← これがあなたの環境で起きているやつ
        if (data is object[] entryArray)
        {
            foreach (var item in entryArray)
            {
                if (item is AddressableAssetEntry entry)
                    ProcessSingleEntry(settings, entry);
            }
            return;
        }

        // 3. List<AddressableAssetEntry> の場合（念のため）
        if (data is IList<AddressableAssetEntry> entryList)
        {
            foreach (var entry in entryList)
                ProcessSingleEntry(settings, entry);
        }
    }

    private static void ProcessSingleEntry(AddressableAssetSettings settings, AddressableAssetEntry entry)
    {
        if (entry == null) return;

        string assetPath = AssetDatabase.GUIDToAssetPath(entry.guid);
        if (string.IsNullOrEmpty(assetPath) || !assetPath.EndsWith(".bytes", System.StringComparison.OrdinalIgnoreCase))
            return;

        string newAddress = GetCustomAddressForBin(assetPath);

        if (entry.address != newAddress)
        {
            _isProcessing = true;

            entry.address = newAddress;
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryModified, entry, true);

            Debug.Log($"[Addressable Bin] .bytes のAddressを自動設定 → {newAddress}  ({assetPath})");

            _isProcessing = false;
        }
    }

    private static string GetCustomAddressForBin(string assetPath)
    {
        string fileNameWithExt = Path.GetFileName(assetPath);

        List<string> fileDataPath = new List<string>
        {
            SupportFiles.ID_BIN_FILE,
            SupportFiles.MATRIX_ID_BIN_FILE,
            SupportFiles.ALL_GAMEOBJECT_BIN_FILE,
            SupportFiles.ALL_TEXTURE_BIN_FILE,
            SupportFiles.ALL_SOUND_BIN_FILE,
            SupportFiles.ALL_SCENARIO_EVENT_BIN_FILE
        };

        var findData = fileDataPath.Find(x => x.Equals(fileNameWithExt));

        // リストに一致したらファイル名だけ、それ以外は元のフルパス（デフォルト）のまま
        return findData != null ? fileNameWithExt : assetPath;
    }
}
#endif
        