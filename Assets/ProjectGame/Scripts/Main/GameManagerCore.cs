using System.Collections.Generic;
using GameCore;
using GameCore.Tables.ID;
using GameCore.Classes;
using UnityEngine;
using GameCore.Tables;

public class GameManagerCore : BaseSingleton<GameManagerCore>
{
    [System.Serializable]
    public class GameManagerData
    {
        /// <summary>
        /// 各エージェントデータ
        /// </summary>
        [SerializeField]
        public Dictionary<GuestCharacterTableID,Agent> agent_tables = new Dictionary<GuestCharacterTableID, Agent>();

        /// <summary>
        /// プレイヤーデータ
        /// </summary>
        public Agent player_data = new Agent();
    }
    /// <summary>
    /// ゲームマネージャーデータ
    /// </summary>
    public GameManagerData game_manager_data = new GameManagerData();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RuleSetting(GameManagerData value_data = null)
    {
        //まずマネージャーデータを確認
        //エージェントデータがなければ新規で作成
        if(value_data != null) 
        {
            game_manager_data = value_data;
            return;
        }

        //新規で作成
        //まずはランダムで6人を選ぶ
        //USEフラグがOnで6人をランダムで取得
        var guest_character_ids = GameCore.Tables.GuestCharacterIDExtensions.FindAll(x => x.GetRow().Use == true);
        // シャッフル
        for (int i = guest_character_ids.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (guest_character_ids[i], guest_character_ids[j]) = (guest_character_ids[j], guest_character_ids[i]);
        }
        var random_guest_character_ids = guest_character_ids.GetRange(0, Mathf.Min(6, guest_character_ids.Count));
        for(int i = 0; i < random_guest_character_ids.Count; i++)
        {
            List<GuestCharacterTableID> list_ids = new List<GuestCharacterTableID>();
            list_ids.Add(GuestCharacterTableID.None); //プレイヤーを初期に入れる
            
            for(int j = 0; j < random_guest_character_ids.Count; j++)
            {
                if(i == j) continue;
                list_ids.Add(random_guest_character_ids[j]);
            }
            //仮の役職決定
            RoleTypeTableID role_id = RoleTypeTableID.Villager;
            if(i == 0)
            {
                role_id = RoleTypeTableID.Werewolf;
            }
            var add_agent = new Agent();
            add_agent.Initialize(random_guest_character_ids[i],list_ids,role_id);
            //ここでゲームマネージャーデータに保存
            game_manager_data.agent_tables.Add(random_guest_character_ids[i],add_agent);
        }
    }
}
