using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using GameCore.Tables.ID;
using GameCore.Enums;

namespace GameCore.Tables
{
    public class PlaceMapRow : BaseClassDataRow
    {
        [SerializeField]
        protected PlaceMapTableID table_id;
        public PlaceMapTableID TableID { get => table_id;}
        [SerializeField]
        protected string name;
        public string Name { get => name; } // 名前
        [SerializeField]
        protected bool use;
        public bool Use { get => use; } // 使用フラグ
        [SerializeField]
        protected Dictionary<GameCore.Enums.SeatPlaceID, GameCore.Classes.PlaceData> place_map = new Dictionary<GameCore.Enums.SeatPlaceID, GameCore.Classes.PlaceData>();
        public Dictionary<GameCore.Enums.SeatPlaceID, GameCore.Classes.PlaceData> Place_map { get => place_map; } // プレスデータ（辞書）

        public override void Read(int id,BinaryReader reader)
        {
            table_id = (PlaceMapTableID)id;
                    int len_name = reader.ReadInt32();
                    name = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(len_name));
            use = reader.ReadBoolean();
            place_map = new Dictionary<GameCore.Enums.SeatPlaceID, GameCore.Classes.PlaceData>();
            int place_map_count = reader.ReadInt32();
            for (int place_map_i = 0; place_map_i < place_map_count; place_map_i++) {
                GameCore.Enums.SeatPlaceID place_map_key;
                place_map_key = (GameCore.Enums.SeatPlaceID)Enum.ToObject(typeof(GameCore.Enums.SeatPlaceID), reader.ReadInt32());
                GameCore.Classes.PlaceData place_map_val;
                place_map_val = new GameCore.Classes.PlaceData();
                place_map_val.Read(reader);
                place_map[place_map_key] = place_map_val;
            }
        }
    }

}
