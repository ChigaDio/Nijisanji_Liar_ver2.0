using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameCore.Classes
{
    [Serializable]
    public class BasePlaceData : BaseCustomClassData
    {
        [SerializeField]
        protected Vector3 position;
        public Vector3 Position { get => position; } // 座標

        public BasePlaceData() : base() { }
        public override void Read(BinaryReader reader)        {
            position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }
    }
}
