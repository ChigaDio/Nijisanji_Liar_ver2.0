using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using GameCore.Tables.ID;
using GameCore.Enums;

namespace GameCore.Tables
{
    public class GuestCharacterRow : BaseClassDataRow
    {
        [SerializeField]
        protected GuestCharacterTableID table_id;
        public GuestCharacterTableID TableID { get => table_id;}
        [SerializeField]
        protected bool use;
        public bool Use { get => use; } // 使用フラグ
        [SerializeField]
        protected string name;
        public string Name { get => name; } // 名前
        [SerializeField]
        protected GameCore.Classes.CharacterStats characterStats;
        public GameCore.Classes.CharacterStats Characterstats { get => characterStats; } // ステータス
        [SerializeField]
        protected UnityEngine.Color image_color = new UnityEngine.Color(1f, 1f, 1f, 1f);
        public UnityEngine.Color Image_color { get => image_color; } // キャラのイメージカラー
        [SerializeField]
        protected GameCore.Enums.GameObject_Character_PrefabID prefab_id;
        public GameCore.Enums.GameObject_Character_PrefabID Prefab_id { get => prefab_id; } // プレファブID

        public override void Read(int id,BinaryReader reader)
        {
            table_id = (GuestCharacterTableID)id;
            use = reader.ReadBoolean();
                    int len_name = reader.ReadInt32();
                    name = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(len_name));
            characterStats = new GameCore.Classes.CharacterStats();
            characterStats.Read(reader);
                image_color = new UnityEngine.Color(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            prefab_id = (GameCore.Enums.GameObject_Character_PrefabID)Enum.ToObject(typeof(GameCore.Enums.GameObject_Character_PrefabID), reader.ReadInt32());
        }
    }

}
