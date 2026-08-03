
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class GameObject_Character_PrefabIDExtensions
    {
        public static int ToInt(this GameObject_Character_PrefabID id)
        {
            return (int)id;
        }
        public static GameObject_Character_PrefabID ToGameObject_Character_PrefabID(this int id)
        {
            return (GameObject_Character_PrefabID)id;
        }
        public static int ToIndex(this GameObject_Character_PrefabID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<GameObject_Character_PrefabID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<GameObject_Character_PrefabID> id = GameObject_Character_PrefabID.Ange; id < GameObject_Character_PrefabID.Max; id++)
            {
                action(id);
            }
        }
        public static List<GameObject_Character_PrefabID> FindAll(Func<GameObject_Character_PrefabID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<GameObject_Character_PrefabID>();
            for (EnumIDIter<GameObject_Character_PrefabID> id = GameObject_Character_PrefabID.Ange; id < GameObject_Character_PrefabID.Max; id++)
            {
                GameObject_Character_PrefabID value = id;
                if (!Enum.IsDefined(typeof(GameObject_Character_PrefabID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static GameObject_Character_PrefabID Find(Func<GameObject_Character_PrefabID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<GameObject_Character_PrefabID> id = GameObject_Character_PrefabID.Ange; id < GameObject_Character_PrefabID.Max; id++)
            {
                GameObject_Character_PrefabID value = id;
                if (!Enum.IsDefined(typeof(GameObject_Character_PrefabID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return GameObject_Character_PrefabID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        