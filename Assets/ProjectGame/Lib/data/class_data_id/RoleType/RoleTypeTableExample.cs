
using System;
using UnityEngine;
using GameCore.Tables;
using GameCore.Tables.ID;
using System.Collections.Generic;
namespace GameCore.Tables
{
    public static class RoleTypeIDExtensions
    {
        public static RoleTypeRow GetRow(this RoleTypeTableID id)
        {
            if (RoleTypeTable.Table.TryGetValue(id, out var row))
            {
                return row;
            }
            else
            {
                return null; // または throw new KeyNotFoundException()
            }
        }
        public static int ToInt(this RoleTypeTableID id)
        {
            return (int)id;
        }
        
        public static int ToIndex(this RoleTypeTableID id)
        {
            return (int)id - 1;
        }
        public static RoleTypeTableID ToRoleTypeTableID(this int id)
        {
            return (RoleTypeTableID)id;
        }
        public static void ForID(Action<RoleTypeTableID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<RoleTypeTableID> id = RoleTypeTableID.Villager; id < RoleTypeTableID.Max; id++)
            {
                action(id);
            }
        }
        public static List<RoleTypeTableID> FindAll(Func<RoleTypeTableID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var results = new List<RoleTypeTableID>();
            for (EnumIDIter<RoleTypeTableID> id = RoleTypeTableID.Villager; id < RoleTypeTableID.Max; id++)
            {
                RoleTypeTableID value = id;
                if (!Enum.IsDefined(typeof(RoleTypeTableID), value))continue; // 無効な値はスキップ
                if (predicate(value))results.Add(value);
            }
            
            return results;
        }
        
        public static RoleTypeTableID Find(Func<RoleTypeTableID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            for (EnumIDIter<RoleTypeTableID> id = RoleTypeTableID.Villager; id < RoleTypeTableID.Max; id++)
            {
                RoleTypeTableID value = id;
                if (!Enum.IsDefined(typeof(RoleTypeTableID), value))continue; // 無効な値はスキップ
                if (predicate(value))return value;
            }
            
            return RoleTypeTableID.None; // デフォルト値（必要に応じて変更）
        }
    }
}
            