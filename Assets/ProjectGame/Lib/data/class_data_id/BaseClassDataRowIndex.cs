using System.IO;
    using System;
    using System.Collections.Generic;

    namespace GameCore.Tables
    {
        // テーブル内の各行(id)ごとの[Offset(テーブル先頭からの相対位置), Size]を保持する。
        // 各テーブルの{Name}RowIndexはこのクラスを継承して生成される。
        public abstract class BaseClassDataRowIndex<T> where T : Enum
        {
            public Dictionary<T, (long Offset, int Size)> Entries = new Dictionary<T, (long, int)>();
            public bool IsRead { get; private set; }

            // reader は「行インデックスブロックの先頭」に位置している前提。
            // forceReload=true でデバッグ用に読み直しできる。
            public void Read(BinaryReader reader, bool forceReload = false)
            {
                if (IsRead && !forceReload) return;
                Entries.Clear();
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    int idVal = reader.ReadInt32();
                    T id = (T)Enum.ToObject(typeof(T), idVal);
                    long offset = reader.ReadInt64();
                    int size = reader.ReadInt32();
                    Entries[id] = (offset, size);
                }
                IsRead = true;
            }
        }
    }
