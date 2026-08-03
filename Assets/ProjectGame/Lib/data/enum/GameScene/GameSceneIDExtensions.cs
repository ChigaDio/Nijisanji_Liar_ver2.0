
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class GameSceneIDExtensions
    {
        public static int ToInt(this GameSceneID id)
        {
            return (int)id;
        }
        public static GameSceneID ToGameSceneID(this int id)
        {
            return (GameSceneID)id;
        }
        public static int ToIndex(this GameSceneID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<GameSceneID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<GameSceneID> id = GameSceneID.MorningRoom; id < GameSceneID.Max; id++)
            {
                action(id);
            }
        }
        public static List<GameSceneID> FindAll(Func<GameSceneID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<GameSceneID>();
            for (EnumIDIter<GameSceneID> id = GameSceneID.MorningRoom; id < GameSceneID.Max; id++)
            {
                GameSceneID value = id;
                if (!Enum.IsDefined(typeof(GameSceneID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static GameSceneID Find(Func<GameSceneID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<GameSceneID> id = GameSceneID.MorningRoom; id < GameSceneID.Max; id++)
            {
                GameSceneID value = id;
                if (!Enum.IsDefined(typeof(GameSceneID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return GameSceneID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        