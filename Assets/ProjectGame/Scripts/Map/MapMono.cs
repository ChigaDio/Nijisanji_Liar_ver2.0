using UnityEngine;
using System;
using GameCore.Tables.ID;

[Serializable]
public class MapMono : MonoBehaviour
{
    /// <summary>
    /// マップデータ
    /// </summary>
    [SerializeField]
    private  PlaceMapTableID map_id = PlaceMapTableID.None;
    public PlaceMapTableID GetMapID => map_id;


    /// <summary>
    /// センターポス
    /// </summary>
    [SerializeField]
    private  Transform center_place_trans;
    public Transform GetCenterPlaceTransForm => center_place_trans;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
