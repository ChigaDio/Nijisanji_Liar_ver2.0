using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameCore.Classes
{
    [Serializable]
    public class BaseCharacterImpression : BaseCustomClassData
    {
        [SerializeField]
        protected float suspicion;
        public float Suspicion { get => suspicion; } // 疑惑
        [SerializeField]
        protected float favorability;
        public float Favorability { get => favorability; } // 友好度

        public BaseCharacterImpression() : base() { }
        public override void Read(BinaryReader reader)        {
            suspicion = reader.ReadSingle();
            favorability = reader.ReadSingle();
        }
    }
}
