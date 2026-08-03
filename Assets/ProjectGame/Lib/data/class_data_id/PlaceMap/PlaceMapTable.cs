using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using GameCore.Tables.ID;
using GameCore.Enums;

namespace GameCore.Tables
{
    public class PlaceMapTable : BaseClassDataID<PlaceMapTableID, PlaceMapRow>
    {
        static PlaceMapTable()
        {
            RowIndex = new PlaceMapRowIndex();
            TableId = TableID.PlaceMap;
            RegisterReferenceLoader(); // 依存先プリロード用に自分自身を登録
        }

        public override void Read(BinaryReader reader)
        {
            PlaceMapTable.Table.Clear();
            int rowCount = reader.ReadInt32();
            int colCount = reader.ReadInt32();
            var colNames = new string[colCount];
            var colTypes = new string[colCount];
            for(int i=0; i<colCount; i++) {
                int len = reader.ReadInt32();
                colNames[i] = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(len));
                len = reader.ReadInt32();
                colTypes[i] = System.Text.Encoding.UTF8.GetString(reader.ReadBytes(len));
            }
            RowIndex.Read(reader, true); // 行インデックスブロックを読み進めつつキャッシュしておく（高速な連続読みはそのまま維持）
            for(int r=0; r<rowCount; r++) {
                var enumVal = (PlaceMapTableID)Enum.ToObject(typeof(PlaceMapTableID), reader.ReadInt32());
                var row = new PlaceMapRow();
                row.Read(r + 1,reader);
                Table[enumVal] = row;
            }
        }
    }
}
