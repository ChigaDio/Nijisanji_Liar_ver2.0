using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using GameCore.Tables.ID;
using GameCore.Enums;

namespace GameCore.Tables
{
    public class RoleTypeRow : BaseClassDataRow
    {
        [SerializeField]
        protected RoleTypeTableID table_id;
        public RoleTypeTableID TableID { get => table_id;}
        [SerializeField]
        protected string name;
        public string Name { get => name; } // 役職名
        [SerializeField]
        protected GameCore.Enums.FactionID faction_id;
        public GameCore.Enums.FactionID Faction_id { get => faction_id; } // 所属ID
        [SerializeField]
        protected bool use;
        public bool Use { get => use; } // 使用フラグ

        public override void Read(int id,BinaryReader reader)
        {
            table_id = (RoleTypeTableID)id;
                    int len_name = reader.ReadInt32();
                    name = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(len_name));
            faction_id = (GameCore.Enums.FactionID)Enum.ToObject(typeof(GameCore.Enums.FactionID), reader.ReadInt32());
            use = reader.ReadBoolean();
        }
    }

}
