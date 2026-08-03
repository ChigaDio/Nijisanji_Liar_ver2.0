#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using GameCore;

public class SupportFilesPostprocessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 100;

    public void OnPostprocessBuild(BuildReport report)
    {
        string buildDir = Path.GetDirectoryName(report.summary.outputPath);
        if (string.IsNullOrEmpty(buildDir)) return;

        // コピー対象のファイルと、それぞれの SupportChigadio/data 以下の相対フォルダ
        var allFiles = new List<(string filePath, string targetSubFolder)>
        {
            (SupportFiles.ALL_SOUND_BIN, Path.Combine(SupportFiles.ASSETS_FOLDER, SupportFiles.SOUND_FOLDER)),
            (SupportFiles.ALL_TEXTURE_BIN, Path.Combine(SupportFiles.ASSETS_FOLDER, SupportFiles.TEXTURE_FOLDER)),
            (SupportFiles.ALL_GAMEOBJECT_BIN, Path.Combine(SupportFiles.ASSETS_FOLDER, SupportFiles.GAMEOBJECT_FOLDER)),
            (SupportFiles.ALL_MATRIX_ID_BIN, SupportFiles.MATRIX_DATA_ID_FOLDER),
            (SupportFiles.ALL_ID_BIN, SupportFiles.ID_FOLDER),
            (SupportFiles.ALL_MATERIAL_BIN, Path.Combine(SupportFiles.ASSETS_FOLDER, SupportFiles.MATERIAL_FOLDER)),
            (SupportFiles.ALL_SCENARIO_EVENTS_BIN,Path.Combine(SupportFiles.SCENARIO_FOLDER,SupportFiles.SCENARIO_EVEMT_FOLDER)),
            (SupportFiles.ALL_CUSTOM_CLASS_DATA_ID_BIN, SupportFiles.CUSTOM_CLASS_DATA_FOLDER)
        };

        foreach (var (filePath, targetFolder) in allFiles)
        {
            CopySupportFileToTargetFolder(filePath, buildDir, targetFolder);
        }
    }

    private void CopySupportFileToTargetFolder(string sourceFilePath, string buildRoot, string targetSubFolder)
    {
        if (!File.Exists(sourceFilePath))
        {
            Debug.LogWarning($"[SupportFilesPostprocessor] Source file not found: {sourceFilePath}");
            return;
        }

        string destPath = Path.Combine(buildRoot, SupportFiles.SUPPORT_ROOT_NAME, SupportFiles.SUPPORT_DATA_NAME, targetSubFolder, Path.GetFileName(sourceFilePath));

        // コピー先フォルダを作成
        string destDir = Path.GetDirectoryName(destPath);
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        // 上書きコピー
        File.Copy(sourceFilePath, destPath, true);
        Debug.Log($"[SupportFilesPostprocessor] Copied {sourceFilePath} -> {destPath}");
    }
}
#endif
