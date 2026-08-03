
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class GameObject_CharacterIDExtensions
    {
        public static int ToInt(this GameObject_CharacterID id)
        {
            return (int)id;
        }
        public static GameObject_CharacterID ToGameObject_CharacterID(this int id)
        {
            return (GameObject_CharacterID)id;
        }
        public static int ToIndex(this GameObject_CharacterID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<GameObject_CharacterID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<GameObject_CharacterID> id = GameObject_CharacterID.Prefab; id < GameObject_CharacterID.Max; id++)
            {
                action(id);
            }
        }
        public static List<GameObject_CharacterID> FindAll(Func<GameObject_CharacterID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<GameObject_CharacterID>();
            for (EnumIDIter<GameObject_CharacterID> id = GameObject_CharacterID.Prefab; id < GameObject_CharacterID.Max; id++)
            {
                GameObject_CharacterID value = id;
                if (!Enum.IsDefined(typeof(GameObject_CharacterID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static GameObject_CharacterID Find(Func<GameObject_CharacterID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<GameObject_CharacterID> id = GameObject_CharacterID.Prefab; id < GameObject_CharacterID.Max; id++)
            {
                GameObject_CharacterID value = id;
                if (!Enum.IsDefined(typeof(GameObject_CharacterID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return GameObject_CharacterID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        