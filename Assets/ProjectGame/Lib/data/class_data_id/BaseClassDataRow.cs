using System.IO;
    using System.Collections.Generic;
    using GameCore.Enums;

    namespace GameCore.Tables
    {
        public abstract class BaseClassDataRow
        {
            public abstract void Read(int id,BinaryReader reader);

            /// <summary>
            /// この行が参照している他のclass_data_idの(TableID, 参照先id)一覧。
            /// 参照フィールドを持つテーブルでは自動生成コード側でoverrideされる。デフォルトは空。
            /// </summary>
            public virtual List<(TableID TableId, int RefId)> GetReferencedIds()
            {
                return new List<(TableID, int)>();
            }
        }
    }
