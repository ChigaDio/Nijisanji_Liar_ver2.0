using System;
using UnityEngine;

[Serializable]
public class MapTitle : MonoBehaviour
{
    /// <summary>
    /// 回転スピード
    /// </summary>
    [SerializeField]
    private float speed = 1.0f;
    [SerializeField]
    private Transform map;

    private void Update() 
    {
        if(map == null) return;
        map.Rotate(0f,speed * Time.deltaTime,0f);
    }
}
