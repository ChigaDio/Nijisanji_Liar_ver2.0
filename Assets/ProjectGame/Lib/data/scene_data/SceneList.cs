using System;
using System.Collections.Generic;
using GameCore.Enums;

public class SceneList
{
    // シーン名管理
    public static readonly Dictionary<GameSceneID, string> sceneNames = new Dictionary<GameSceneID, string>
    {
        { GameSceneID.MorningRoom, "MorningRoom" }
    };
}
