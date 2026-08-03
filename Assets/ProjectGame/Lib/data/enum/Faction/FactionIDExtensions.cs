
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class FactionIDExtensions
    {
        public static int ToInt(this FactionID id)
        {
            return (int)id;
        }
        public static FactionID ToFactionID(this int id)
        {
            return (FactionID)id;
        }
        public static int ToIndex(this FactionID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<FactionID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<FactionID> id = FactionID.Human; id < FactionID.Max; id++)
            {
                action(id);
            }
        }
        public static List<FactionID> FindAll(Func<FactionID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<FactionID>();
            for (EnumIDIter<FactionID> id = FactionID.Human; id < FactionID.Max; id++)
            {
                FactionID value = id;
                if (!Enum.IsDefined(typeof(FactionID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static FactionID Find(Func<FactionID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<FactionID> id = FactionID.Human; id < FactionID.Max; id++)
            {
                FactionID value = id;
                if (!Enum.IsDefined(typeof(FactionID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return FactionID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        