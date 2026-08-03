
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class Sound_TitleIDExtensions
    {
        public static int ToInt(this Sound_TitleID id)
        {
            return (int)id;
        }
        public static Sound_TitleID ToSound_TitleID(this int id)
        {
            return (Sound_TitleID)id;
        }
        public static int ToIndex(this Sound_TitleID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<Sound_TitleID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<Sound_TitleID> id = Sound_TitleID.Main; id < Sound_TitleID.Max; id++)
            {
                action(id);
            }
        }
        public static List<Sound_TitleID> FindAll(Func<Sound_TitleID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<Sound_TitleID>();
            for (EnumIDIter<Sound_TitleID> id = Sound_TitleID.Main; id < Sound_TitleID.Max; id++)
            {
                Sound_TitleID value = id;
                if (!Enum.IsDefined(typeof(Sound_TitleID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static Sound_TitleID Find(Func<Sound_TitleID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<Sound_TitleID> id = Sound_TitleID.Main; id < Sound_TitleID.Max; id++)
            {
                Sound_TitleID value = id;
                if (!Enum.IsDefined(typeof(Sound_TitleID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return Sound_TitleID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        