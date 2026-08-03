
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class Sound_UIIDExtensions
    {
        public static int ToInt(this Sound_UIID id)
        {
            return (int)id;
        }
        public static Sound_UIID ToSound_UIID(this int id)
        {
            return (Sound_UIID)id;
        }
        public static int ToIndex(this Sound_UIID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<Sound_UIID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<Sound_UIID> id = Sound_UIID.Title; id < Sound_UIID.Max; id++)
            {
                action(id);
            }
        }
        public static List<Sound_UIID> FindAll(Func<Sound_UIID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<Sound_UIID>();
            for (EnumIDIter<Sound_UIID> id = Sound_UIID.Title; id < Sound_UIID.Max; id++)
            {
                Sound_UIID value = id;
                if (!Enum.IsDefined(typeof(Sound_UIID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static Sound_UIID Find(Func<Sound_UIID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<Sound_UIID> id = Sound_UIID.Title; id < Sound_UIID.Max; id++)
            {
                Sound_UIID value = id;
                if (!Enum.IsDefined(typeof(Sound_UIID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return Sound_UIID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        