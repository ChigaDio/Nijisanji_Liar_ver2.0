
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class GameObjectIDExtensions
    {
        public static int ToInt(this GameObjectID id)
        {
            return (int)id;
        }
        public static GameObjectID ToGameObjectID(this int id)
        {
            return (GameObjectID)id;
        }
        public static int ToIndex(this GameObjectID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<GameObjectID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<GameObjectID> id = GameObjectID.Character_Ange; id < GameObjectID.Max; id++)
            {
                action(id);
            }
        }
        public static List<GameObjectID> FindAll(Func<GameObjectID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<GameObjectID>();
            for (EnumIDIter<GameObjectID> id = GameObjectID.Character_Ange; id < GameObjectID.Max; id++)
            {
                GameObjectID value = id;
                if (!Enum.IsDefined(typeof(GameObjectID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static GameObjectID Find(Func<GameObjectID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<GameObjectID> id = GameObjectID.Character_Ange; id < GameObjectID.Max; id++)
            {
                GameObjectID value = id;
                if (!Enum.IsDefined(typeof(GameObjectID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return GameObjectID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        