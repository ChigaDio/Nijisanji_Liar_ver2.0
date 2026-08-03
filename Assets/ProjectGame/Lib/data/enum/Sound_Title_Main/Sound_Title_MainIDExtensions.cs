
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class Sound_Title_MainIDExtensions
    {
        public static int ToInt(this Sound_Title_MainID id)
        {
            return (int)id;
        }
        public static Sound_Title_MainID ToSound_Title_MainID(this int id)
        {
            return (Sound_Title_MainID)id;
        }
        public static int ToIndex(this Sound_Title_MainID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<Sound_Title_MainID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<Sound_Title_MainID> id = Sound_Title_MainID.Main_BGM; id < Sound_Title_MainID.Max; id++)
            {
                action(id);
            }
        }
        public static List<Sound_Title_MainID> FindAll(Func<Sound_Title_MainID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<Sound_Title_MainID>();
            for (EnumIDIter<Sound_Title_MainID> id = Sound_Title_MainID.Main_BGM; id < Sound_Title_MainID.Max; id++)
            {
                Sound_Title_MainID value = id;
                if (!Enum.IsDefined(typeof(Sound_Title_MainID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static Sound_Title_MainID Find(Func<Sound_Title_MainID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<Sound_Title_MainID> id = Sound_Title_MainID.Main_BGM; id < Sound_Title_MainID.Max; id++)
            {
                Sound_Title_MainID value = id;
                if (!Enum.IsDefined(typeof(Sound_Title_MainID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return Sound_Title_MainID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        