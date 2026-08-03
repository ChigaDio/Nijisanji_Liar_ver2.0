using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameCore.Classes
{
    [Serializable]
    public class BaseCharacterStats : BaseCustomClassData
    {
        [SerializeField]
        protected float charisma;
        public float Charisma { get => charisma; } // カリスマ
        [SerializeField]
        protected float intuition;
        public float Intuition { get => intuition; } // 直感
        [SerializeField]
        protected float reasoning;
        public float Reasoning { get => reasoning; } // ロジック
        [SerializeField]
        protected float appeal;
        public float Appeal { get => appeal; } // 可愛さ
        [SerializeField]
        protected float deception;
        public float Deception { get => deception; } // 演技力
        [SerializeField]
        protected float stealth;
        public float Stealth { get => stealth; } // ステルス

        public BaseCharacterStats() : base() { }
        public override void Read(BinaryReader reader)        {
            charisma = reader.ReadSingle();
            intuition = reader.ReadSingle();
            reasoning = reader.ReadSingle();
            appeal = reader.ReadSingle();
            deception = reader.ReadSingle();
            stealth = reader.ReadSingle();
        }
    }
}
