using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SceneBuild : EditorWindow
{
    [MenuItem("Tools/Build/Build All")]
    public static void BuildAll()
    {
        string path = EditorUtility.OpenFolderPanel("Select Build Folder", Application.dataPath, "");
        if (string.IsNullOrEmpty(path)) return;

        // Build Client
        string clientPath = Path.Combine(path, "Client");
        if (!Directory.Exists(clientPath)) Directory.CreateDirectory(clientPath);

        BuildPlayerOptions clientOptions = new BuildPlayerOptions();
        clientOptions.scenes = new string[] {
            "D:/DoujinGameUnity/Nijisanji_Liar_Ver2.0/Assets/ProjectGame/MyAssets/Scene/Map/MorningRoom.unity"
        };
        clientOptions.locationPathName = Path.Combine(clientPath, "Client.exe");
        clientOptions.target = BuildTarget.StandaloneWindows64;
        
        var clientReport = BuildPipeline.BuildPlayer(clientOptions);
        if (clientReport.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Client Build Succeeded: " + clientOptions.locationPathName);
        }
        else
        {
            Debug.LogError("Client Build Failed");
        }

        // Build Server
        string serverPath = Path.Combine(path, "Server");
        if (!Directory.Exists(serverPath)) Directory.CreateDirectory(serverPath);

        BuildPlayerOptions serverOptions = new BuildPlayerOptions();
        serverOptions.scenes = new string[] {
            
        };
        serverOptions.locationPathName = Path.Combine(serverPath, "Server.x86_64");
        serverOptions.target = BuildTarget.StandaloneLinux64;
        serverOptions.subtarget = (int)StandaloneBuildSubtarget.Server; // Headless mode
        
        var serverReport = BuildPipeline.BuildPlayer(serverOptions);
        if (serverReport.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Server Build Succeeded: " + serverOptions.locationPathName);
        }
        else
        {
            Debug.LogError("Server Build Failed");
        }
    }
}
