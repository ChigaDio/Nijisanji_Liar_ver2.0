using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameCore.Classes
{
    [Serializable]
    public class BaseAgent : BaseCustomClassData
    {
        [SerializeField]
        protected GameCore.Tables.ID.GuestCharacterTableID guest_character_id;
        public GameCore.Tables.ID.GuestCharacterTableID Guest_character_id { get => guest_character_id; } // ゲストID(Noneならプレイヤー)
        [SerializeField]
        protected GameCore.Classes.CharacterStats character_stats;
        public GameCore.Classes.CharacterStats Character_stats { get => character_stats; } // キャラステータス
        [SerializeField]
        protected GameCore.Tables.ID.RoleTypeTableID role_type;
        public GameCore.Tables.ID.RoleTypeTableID Role_type { get => role_type; } // 役職ID

        public BaseAgent() : base() { }
        public override void Read(BinaryReader reader)        {
            guest_character_id = (GameCore.Tables.ID.GuestCharacterTableID)Enum.ToObject(typeof(GameCore.Tables.ID.GuestCharacterTableID), reader.ReadInt32());
            character_stats = new GameCore.Classes.CharacterStats();
            character_stats.Read(reader);
            role_type = (GameCore.Tables.ID.RoleTypeTableID)Enum.ToObject(typeof(GameCore.Tables.ID.RoleTypeTableID), reader.ReadInt32());
        }
    }
}
