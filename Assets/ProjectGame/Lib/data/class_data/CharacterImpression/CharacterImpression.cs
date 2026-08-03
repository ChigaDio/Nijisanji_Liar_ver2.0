using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
namespace GameCore.Classes
{
    [Serializable]
    public class CharacterImpression : BaseCharacterImpression
    {

        //好感度を変動する
        public void ChangeFavorability(float value)
        {
            //-100 ~ 100の範囲で変動するようにする
            this.favorability = Mathf.Clamp(this.favorability + value, -100f, 100f);
        }

        //疑惑度を変動する
        public void ChangeSuspicion(float value)
        {
            //疑惑度が0未満にならないようにする
            this.suspicion = Mathf.Max(0, this.suspicion + value);
        }
        
    }

}
