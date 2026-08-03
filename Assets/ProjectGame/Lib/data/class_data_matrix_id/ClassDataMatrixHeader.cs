

using System;
using System.IO;
using System.Collections.Generic;
using GameCore.Enums;

namespace GameCore.Tables
{
    public class ClassDataMatrixHeader
    {
        public Dictionary<MatrixTableID, (string Name, long Offset, int Size)> Entries = new Dictionary<MatrixTableID, (string, long, int)>();

        public ClassDataMatrixHeader(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for(int i = 0; i < count; i++)
            {
                int id = reader.ReadInt32();
                MatrixTableID tableId = (MatrixTableID)Enum.ToObject(typeof(MatrixTableID), id);
                int nameLen = reader.ReadInt32();
                string name = new string(reader.ReadChars(nameLen));
                long offset = reader.ReadInt64();
                int size = reader.ReadInt32();
                Entries[tableId] = (name, offset, size);
            }
        }

        public TTable GetData<TTable>(MatrixTableID id, BinaryReader reader) where TTable : BaseTableMatrix, new()
        {
            if (!Entries.TryGetValue(id, out var entry)) return null;
            reader.BaseStream.Seek(entry.Offset, SeekOrigin.Begin);
            TTable data = new TTable();
            data.Read(reader);
            return data;
        }
    }
}

