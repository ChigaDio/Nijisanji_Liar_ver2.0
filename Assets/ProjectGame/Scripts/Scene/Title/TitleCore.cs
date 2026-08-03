using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCore;
using GameCore.Sound;
using GameCore.States.Control;
using GameCore.Tables;
using GameCore.Tables.ID;
using UnityEngine;

public class TitleCore : BaseSingleton<TitleCore>
{
    /// <summary>
    /// マップのセンターオブジェクト
    /// </summary>
    [SerializeField]
    private GameObject place_map_center;

    private Dictionary<GuestCharacterTableID,GuestController> dict_guest_title_character = new Dictionary<GuestCharacterTableID, GuestController>();
    public Dictionary<GuestCharacterTableID,GuestController> GuestTitleCharacter => dict_guest_title_character;

    private TitleStateControl title_state_control = new TitleStateControl();
    // Update is called once per frame
    void Update()
    {
        title_state_control.UpdateStateCombined();
    }

    private void Start()
    {
        title_state_control.Setup(gameObject.GetCancellationTokenOnDestroy());
        title_state_control.StartStateCombined();
    }


    public async UniTask SetCharacter()
    {
        var mapMono = SceneLoader.GetComponentInScene<MapMono>(GameCore.Enums.GameSceneID.MorningRoom);
        if(mapMono != null) this.place_map_center = mapMono.GetCenterPlaceTransForm.gameObject;
        //USEフラグがOnで6人をランダムで取得
        var guest_character_ids = GameCore.Tables.GuestCharacterIDExtensions.FindAll(x => x.GetRow().Use == true);
        // シャッフル
        for (int i = guest_character_ids.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (guest_character_ids[i], guest_character_ids[j]) = (guest_character_ids[j], guest_character_ids[i]);
        }
        var random_guest_character_ids = guest_character_ids.GetRange(0, Mathf.Min(6, guest_character_ids.Count));
        
        ///カフェマップ
        var place_id =  GameCore.Tables.ID.PlaceMapTableID.PlaceMap_CafeMap;
        var data_row = place_id.GetRow();
        for(int i = 0; i < random_guest_character_ids.Count; i++)
        {
            var random_guest_data_row = random_guest_character_ids[i].GetRow();
            var state_place_id = (GameCore.Enums.SeatPlaceID)(i + 1);
            if(data_row.Place_map.ContainsKey(state_place_id) == false) continue;
            var place_position = data_row.Place_map[state_place_id].Position;

            var prefab_id = random_guest_data_row.Prefab_id;
            //ロード
            await GameCore.Gameobject.GameObjectCore.Instance.LoadSingleAsync(prefab_id,AddressableSystem.GroupCategory.Game);

            var find_prefab = GameCore.Gameobject.GameObjectCore.Instance.GetGameObject(random_guest_data_row.Prefab_id);
            if(find_prefab == null) continue;

            var handle = GameObject.InstantiateAsync(find_prefab,this.place_map_center.transform);
            await handle;

            GuestController result = handle.Result[0].GetComponent<GuestController>();
            //マテリアル初期設定
            result._material.Awake();
            //座標をセット
            result.transform.localPosition = place_position;
            //イメージカラーに設定
            result._material.SetColorEfficiently(random_guest_data_row.Image_color);
            result._material.SetSwitchEfficiently(1.0f);

            dict_guest_title_character.Add(random_guest_data_row.TableID,result);
            await UniTask.Yield();
            
        }

    }

    public void Onestroy()
    {
        dict_guest_title_character.Clear();
        //Addressableで該当するものを削除(BGMは削除)
        SoundCore.Instance.UnloadGroup(SoundGroup.Title,AddressableSystem.GroupCategory.Title);
        //マップの削除
        SceneLoader.UnloadSceneAsync(GameCore.Enums.GameSceneID.MorningRoom).Forget();
    }
}
