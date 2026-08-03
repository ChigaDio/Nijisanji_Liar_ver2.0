
using System;
using UnityEngine;
using GameCore.Tables;
using GameCore.Tables.ID;
using System.Collections.Generic;
namespace GameCore.Tables
{
    public static class PlaceMapIDExtensions
    {
        public static PlaceMapRow GetRow(this PlaceMapTableID id)
        {
            if (PlaceMapTable.Table.TryGetValue(id, out var row))
            {
                return row;
            }
            else
            {
                return null; // または throw new KeyNotFoundException()
            }
        }
        public static int ToInt(this PlaceMapTableID id)
        {
            return (int)id;
        }
        
        public static int ToIndex(this PlaceMapTableID id)
        {
            return (int)id - 1;
        }
        public static PlaceMapTableID ToPlaceMapTableID(this int id)
        {
            return (PlaceMapTableID)id;
        }
        public static void ForID(Action<PlaceMapTableID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<PlaceMapTableID> id = PlaceMapTableID.PlaceMap_CafeMap; id < PlaceMapTableID.Max; id++)
            {
                action(id);
            }
        }
        public static List<PlaceMapTableID> FindAll(Func<PlaceMapTableID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var results = new List<PlaceMapTableID>();
            for (EnumIDIter<PlaceMapTableID> id = PlaceMapTableID.PlaceMap_CafeMap; id < PlaceMapTableID.Max; id++)
            {
                PlaceMapTableID value = id;
                if (!Enum.IsDefined(typeof(PlaceMapTableID), value))continue; // 無効な値はスキップ
                if (predicate(value))results.Add(value);
            }
            
            return results;
        }
        
        public static PlaceMapTableID Find(Func<PlaceMapTableID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            for (EnumIDIter<PlaceMapTableID> id = PlaceMapTableID.PlaceMap_CafeMap; id < PlaceMapTableID.Max; id++)
            {
                PlaceMapTableID value = id;
                if (!Enum.IsDefined(typeof(PlaceMapTableID), value))continue; // 無効な値はスキップ
                if (predicate(value))return value;
            }
            
            return PlaceMapTableID.None; // デフォルト値（必要に応じて変更）
        }
    }
}
            