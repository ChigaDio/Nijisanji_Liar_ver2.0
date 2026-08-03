using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using GameCore.Tables.ID;
using GameCore.Tables;
namespace GameCore.Classes
{
    [Serializable]
    public class Agent : BaseAgent
    {
        /// <summary>
        /// キャラクターごとの好感度、疑惑度
        /// </summary>
        [SerializeField]
        private Dictionary<GameCore.Tables.ID.GuestCharacterTableID, CharacterImpression> character_impressions = new Dictionary<GameCore.Tables.ID.GuestCharacterTableID, CharacterImpression>();

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(GuestCharacterTableID id,List<GameCore.Tables.ID.GuestCharacterTableID> character_ids,RoleTypeTableID value_role_type)
        {
            //今回参加するキャラ
            foreach (var character_id in character_ids)
            {
                if (!character_impressions.ContainsKey(character_id))
                {
                    character_impressions.Add(character_id, new CharacterImpression());
                }
            }
            var data  = id.GetRow();
            if(data != null) character_stats = data.Characterstats;
            //役職
            role_type = value_role_type;
        }

        /// <summary>
        /// 一番疑惑度が高いキャラを返す
        /// </summary>
        /// <returns></returns>
        public GameCore.Tables.ID.GuestCharacterTableID GetMostSuspiciousCharacter()
        {
            GameCore.Tables.ID.GuestCharacterTableID most_suspicious_character_id = GameCore.Tables.ID.GuestCharacterTableID.None;
            float max_suspicion = float.MinValue;

            foreach (var kvp in character_impressions)
            {
                var character_id = kvp.Key;
                var impression = kvp.Value;

                if (impression.Suspicion > max_suspicion)
                {
                    max_suspicion = impression.Suspicion;
                    most_suspicious_character_id = character_id;
                }
            }

            return most_suspicious_character_id;
        }

        /// <summary>
        /// 一番好感度が高いキャラを返す
        /// </summary>
        /// <returns></returns>
        public GameCore.Tables.ID.GuestCharacterTableID GetMostLikedCharacter()
        {
            GameCore.Tables.ID.GuestCharacterTableID most_liked_character_id = GameCore.Tables.ID.GuestCharacterTableID.None;
            float max_like = float.MinValue;

            foreach (var kvp in character_impressions)
            {
                var character_id = kvp.Key;
                var impression = kvp.Value;

                if (impression.Favorability > max_like)
                {
                    max_like = impression.Favorability;
                    most_liked_character_id = character_id;
                }
            }

            return most_liked_character_id;
        }

        /// <summary>
        /// 指定したキャラクターの好感度を変動させる
        /// </summary>
        /// <param name="character_id"></param>
        /// <param name="change_value"></param>
        public void ChangeFavorability(GameCore.Tables.ID.GuestCharacterTableID character_id, float change_value)
        {
            if (character_impressions.ContainsKey(character_id))
            {
                character_impressions[character_id].ChangeFavorability(change_value);
            }
        }

        /// <summary>
        /// 指定したキャラクターの疑惑度を変動させる
        /// </summary>
        /// <param name="character_id"></param>
        /// <param name="change_value"></param>
        public void ChangeSuspicion(GameCore.Tables.ID.GuestCharacterTableID character_id, float change_value)
        {
            if (character_impressions.ContainsKey(character_id))
            {
                character_impressions[character_id].ChangeSuspicion(change_value);
            }
        }
    }
}
