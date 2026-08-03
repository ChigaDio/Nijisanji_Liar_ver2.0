
using System;
using UnityEngine;
using GameCore.Tables;
using GameCore.Tables.ID;
using System.Collections.Generic;
namespace GameCore.Tables
{
    public static class GuestCharacterIDExtensions
    {
        public static GuestCharacterRow GetRow(this GuestCharacterTableID id)
        {
            if (GuestCharacterTable.Table.TryGetValue(id, out var row))
            {
                return row;
            }
            else
            {
                return null; // または throw new KeyNotFoundException()
            }
        }
        public static int ToInt(this GuestCharacterTableID id)
        {
            return (int)id;
        }
        
        public static int ToIndex(this GuestCharacterTableID id)
        {
            return (int)id - 1;
        }
        public static GuestCharacterTableID ToGuestCharacterTableID(this int id)
        {
            return (GuestCharacterTableID)id;
        }
        public static void ForID(Action<GuestCharacterTableID> action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            for (EnumIDIter<GuestCharacterTableID> id = GuestCharacterTableID.Kuzuha; id < GuestCharacterTableID.Max; id++)
            {
                action(id);
            }
        }
        public static List<GuestCharacterTableID> FindAll(Func<GuestCharacterTableID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            var results = new List<GuestCharacterTableID>();
            for (EnumIDIter<GuestCharacterTableID> id = GuestCharacterTableID.Kuzuha; id < GuestCharacterTableID.Max; id++)
            {
                GuestCharacterTableID value = id;
                if (!Enum.IsDefined(typeof(GuestCharacterTableID), value))continue; // 無効な値はスキップ
                if (predicate(value))results.Add(value);
            }
            
            return results;
        }
        
        public static GuestCharacterTableID Find(Func<GuestCharacterTableID, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            for (EnumIDIter<GuestCharacterTableID> id = GuestCharacterTableID.Kuzuha; id < GuestCharacterTableID.Max; id++)
            {
                GuestCharacterTableID value = id;
                if (!Enum.IsDefined(typeof(GuestCharacterTableID), value))continue; // 無効な値はスキップ
                if (predicate(value))return value;
            }
            
            return GuestCharacterTableID.None; // デフォルト値（必要に応じて変更）
        }
    }
}
            