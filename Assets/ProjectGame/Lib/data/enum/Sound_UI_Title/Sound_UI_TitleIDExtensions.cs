
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class Sound_UI_TitleIDExtensions
    {
        public static int ToInt(this Sound_UI_TitleID id)
        {
            return (int)id;
        }
        public static Sound_UI_TitleID ToSound_UI_TitleID(this int id)
        {
            return (Sound_UI_TitleID)id;
        }
        public static int ToIndex(this Sound_UI_TitleID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<Sound_UI_TitleID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<Sound_UI_TitleID> id = Sound_UI_TitleID.SelectMove; id < Sound_UI_TitleID.Max; id++)
            {
                action(id);
            }
        }
        public static List<Sound_UI_TitleID> FindAll(Func<Sound_UI_TitleID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<Sound_UI_TitleID>();
            for (EnumIDIter<Sound_UI_TitleID> id = Sound_UI_TitleID.SelectMove; id < Sound_UI_TitleID.Max; id++)
            {
                Sound_UI_TitleID value = id;
                if (!Enum.IsDefined(typeof(Sound_UI_TitleID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static Sound_UI_TitleID Find(Func<Sound_UI_TitleID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<Sound_UI_TitleID> id = Sound_UI_TitleID.SelectMove; id < Sound_UI_TitleID.Max; id++)
            {
                Sound_UI_TitleID value = id;
                if (!Enum.IsDefined(typeof(Sound_UI_TitleID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return Sound_UI_TitleID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        