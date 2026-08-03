
using System;
using UnityEngine;
using System.Collections.Generic;
namespace GameCore.Enums
{
    public static class SeatPlaceIDExtensions
    {
        public static int ToInt(this SeatPlaceID id)
        {
            return (int)id;
        }
        public static SeatPlaceID ToSeatPlaceID(this int id)
        {
            return (SeatPlaceID)id;
        }
        public static int ToIndex(this SeatPlaceID id)
        {
            return (int)id - 1;
        }
        public static void ForID(Action<SeatPlaceID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<SeatPlaceID> id = SeatPlaceID.Place_01; id < SeatPlaceID.Max; id++)
            {
                action(id);
            }
        }
        public static List<SeatPlaceID> FindAll(Func<SeatPlaceID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var results = new List<SeatPlaceID>();
            for (EnumIDIter<SeatPlaceID> id = SeatPlaceID.Place_01; id < SeatPlaceID.Max; id++)
            {
                SeatPlaceID value = id;
                if (!Enum.IsDefined(typeof(SeatPlaceID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    results.Add(value);
            }

            return results;
        }

        public static SeatPlaceID Find(Func<SeatPlaceID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            for (EnumIDIter<SeatPlaceID> id = SeatPlaceID.Place_01; id < SeatPlaceID.Max; id++)
            {
                SeatPlaceID value = id;
                if (!Enum.IsDefined(typeof(SeatPlaceID), value))
                    continue; // 無効な値はスキップ
                if (predicate(value))
                    return value;
            }

            return SeatPlaceID.None; // デフォルト値（必要に応じて変更）
        }
        
        
        
    }
}
        