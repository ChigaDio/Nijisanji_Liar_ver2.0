using System.IO;
    using System.Collections.Generic;
    using GameCore.Enums;

    namespace GameCore.Tables
    {
        [System.Serializable]
        public abstract class BaseClassDataMatrixRow
        {
            public abstract void Read(BinaryReader reader);

            /// <summary>
            /// このセルが参照している他のclass_data_idの(TableID, 参照先id)一覧。
            /// 参照フィールドを持つセルでは自動生成コード側でoverrideされる。デフォルトは空。
            /// </summary>
            public virtual List<(TableID TableId, int RefId)> GetReferencedIds()
            {
                return new List<(TableID, int)>();
            }
        }
    }
