using System.IO;
    using System;
    using System.Collections.Generic;
    using GameCore.Enums;

    namespace GameCore.Tables
    {
        public abstract class BaseClassDataMatrixID<TRow, TCol, E> : BaseTableMatrix where TRow : Enum where TCol : Enum where E : BaseClassDataMatrixRow, new()
        {
            public static Dictionary<TRow, Dictionary<TCol, E>> Table = new Dictionary<TRow, Dictionary<TCol, E>>();

            // 各テーブルの静的コンストラクタで {Name}MatrixRowIndex と TableId がセットされる（rowKeyごとのシーク位置）
            protected static BaseClassDataRowIndex<TRow> RowIndex;
            protected static MatrixTableID TableId;
            // 列キー一覧（行インデックス読み込み時にキャッシュされる）
            protected static List<TCol> s_colKeys;
            // rowKeyごとの「セル単位のシーク位置」キャッシュ（row_blockの先頭からの相対offset）
            protected static Dictionary<TRow, Dictionary<TCol, (long Offset, int Size)>> s_cellIndexCache = new Dictionary<TRow, Dictionary<TCol, (long, int)>>();

            public override abstract void Read(BinaryReader reader);
            public override void Release()
            {
                Table.Clear();
                s_cellIndexCache.Clear();
            }

            /// <summary>
            /// 行インデックス（rowKeyごとのシーク位置）と列キー一覧だけを読み込む。
            /// 既に読み込み済みならスキップ（forceReload=trueで再読み込み）。
            /// </summary>
            protected static void EnsureRowIndexLoaded(BinaryReader reader, long tableBaseOffset, bool forceReload = false)
            {
                if (RowIndex.IsRead && !forceReload) return;

                reader.BaseStream.Seek(tableBaseOffset, SeekOrigin.Begin);
                int rowCount = reader.ReadInt32();
                for (int i = 0; i < rowCount; i++) reader.ReadInt32(); // rowKeyのid列（行インデックスに再度現れるのでここでは読み捨て）
                int colCount = reader.ReadInt32();
                s_colKeys = new List<TCol>(colCount);
                for (int i = 0; i < colCount; i++)
                {
                    s_colKeys.Add((TCol)Enum.ToObject(typeof(TCol), reader.ReadInt32()));
                }
                // ここでreaderは行インデックスブロックの先頭に位置している
                RowIndex.Read(reader, forceReload);

                if (forceReload) s_cellIndexCache.Clear();
            }

            /// <summary>
            /// 指定したrowKeyの「セル単位のシーク位置(row_block先頭からの相対offset)」を読み込む。
            /// 既に読み込み済みならキャッシュを返す（forceReload=trueで再読み込み）。
            /// </summary>
            protected static Dictionary<TCol, (long Offset, int Size)> EnsureCellIndexLoaded(TRow rowId, BinaryReader reader, long tableBaseOffset, bool forceReload = false)
            {
                if (!forceReload && s_cellIndexCache.TryGetValue(rowId, out var cached)) return cached;
                if (!RowIndex.Entries.TryGetValue(rowId, out var rowEntry)) return null;

                reader.BaseStream.Seek(tableBaseOffset + rowEntry.Offset, SeekOrigin.Begin);
                int cellIndexCount = reader.ReadInt32();
                var cellIndex = new Dictionary<TCol, (long, int)>();
                for (int i = 0; i < cellIndexCount; i++)
                {
                    int colIdVal = reader.ReadInt32();
                    TCol colId = (TCol)Enum.ToObject(typeof(TCol), colIdVal);
                    long offset = reader.ReadInt64();
                    int size = reader.ReadInt32();
                    cellIndex[colId] = (offset, size);
                }
                s_cellIndexCache[rowId] = cellIndex;
                return cellIndex;
            }

            /// <summary>
            /// セルが参照している他のclass_data_idを(ネストして)連鎖的にプリロードする。
            /// idHeader/idReaderはID側(all_class_data.bytes)のヘッダーとreader。呼び出し側で別途開いたものを渡す。
            /// </summary>
            private static void PreloadCellReferences(E cell, ClassDataHeader idHeader, BinaryReader idReader, HashSet<(TableID, int)> visited)
            {
                if (cell == null || idHeader == null || idReader == null) return;
                foreach (var reference in cell.GetReferencedIds())
                {
                    if (ClassDataReferenceLoader.Loaders.TryGetValue(reference.TableId, out var loader))
                    {
                        loader(reference.RefId, idHeader, idReader, true, false, visited);
                    }
                }
            }

            private static void ReadCellInternal(TRow rowId, TCol colId, Dictionary<TCol, (long Offset, int Size)> cellIndex, BinaryReader reader, long tableBaseOffset, long rowOffset,
                bool preloadReferences, ClassDataHeader idHeader, BinaryReader idReader, HashSet<(TableID, int)> visited)
            {
                if (cellIndex == null || !cellIndex.TryGetValue(colId, out var cellEntry)) return;
                reader.BaseStream.Seek(tableBaseOffset + rowOffset + cellEntry.Offset, SeekOrigin.Begin);
                var cell = new E();
                cell.Read(reader);
                if (!Table.TryGetValue(rowId, out var rowDict))
                {
                    rowDict = new Dictionary<TCol, E>();
                    Table[rowId] = rowDict;
                }
                rowDict[colId] = cell;

                if (preloadReferences) PreloadCellReferences(cell, idHeader, idReader, visited);
            }

            // ========================= 行単位 =========================

            /// <summary>
            /// 指定した1つのrowKey(行、全列)だけをロードする。テーブル全体はロードしない。
            /// preloadReferences=trueの場合、idHeader/idReader(ID側の別途開いたヘッダーとreader)経由で参照先も(ネストして)連鎖的にロードする。
            /// </summary>
            public static void ReadOneRow(TRow rowId, ClassDataMatrixHeader header, BinaryReader reader, bool preloadReferences = false, ClassDataHeader idHeader = null, BinaryReader idReader = null, bool forceReloadIndex = false)
            {
                if (!header.Entries.TryGetValue(TableId, out var tableEntry)) return;
                long tableBaseOffset = tableEntry.Offset;

                EnsureRowIndexLoaded(reader, tableBaseOffset, forceReloadIndex);
                if (!RowIndex.Entries.TryGetValue(rowId, out var rowEntry)) return;
                var cellIndex = EnsureCellIndexLoaded(rowId, reader, tableBaseOffset, forceReloadIndex);

                var visited = new HashSet<(TableID, int)>();
                foreach (var ck in s_colKeys)
                {
                    ReadCellInternal(rowId, ck, cellIndex, reader, tableBaseOffset, rowEntry.Offset, preloadReferences, idHeader, idReader, visited);
                }
            }

            /// <summary>指定した複数のrowKey(行)だけをロードする。テーブル全体はロードしない。</summary>
            public static void ReadManyRows(IEnumerable<TRow> rowIds, ClassDataMatrixHeader header, BinaryReader reader, bool preloadReferences = false, ClassDataHeader idHeader = null, BinaryReader idReader = null, bool forceReloadIndex = false)
            {
                foreach (var rowId in rowIds) ReadOneRow(rowId, header, reader, preloadReferences, idHeader, idReader, forceReloadIndex);
            }

            /// <summary>指定した1つのrowKey(行全体)だけをアンロードする（テーブル全体は消さない）</summary>
            public static void UnloadOneRow(TRow rowId)
            {
                Table.Remove(rowId);
                s_cellIndexCache.Remove(rowId);
            }

            /// <summary>指定した複数のrowKeyだけをアンロードする（テーブル全体は消さない）</summary>
            public static void UnloadManyRows(IEnumerable<TRow> rowIds)
            {
                foreach (var rowId in rowIds) UnloadOneRow(rowId);
            }

            /// <summary>条件(predicate)に合致するrowKey(行)を一括アンロードする</summary>
            public static void UnloadRowsWhere(Func<TRow, Dictionary<TCol, E>, bool> predicate)
            {
                var keysToRemove = new List<TRow>();
                foreach (var kv in Table)
                {
                    if (predicate(kv.Key, kv.Value)) keysToRemove.Add(kv.Key);
                }
                foreach (var key in keysToRemove) UnloadOneRow(key);
            }

            // ========================= 列単位 =========================

            /// <summary>
            /// 指定した1つのcolKey(列、全行)だけをロードする。テーブル全体はロードしない。
            /// </summary>
            public static void ReadOneColumn(TCol colId, ClassDataMatrixHeader header, BinaryReader reader, bool preloadReferences = false, ClassDataHeader idHeader = null, BinaryReader idReader = null, bool forceReloadIndex = false)
            {
                if (!header.Entries.TryGetValue(TableId, out var tableEntry)) return;
                long tableBaseOffset = tableEntry.Offset;

                EnsureRowIndexLoaded(reader, tableBaseOffset, forceReloadIndex);
                var visited = new HashSet<(TableID, int)>();
                foreach (var rowId in RowIndex.Entries.Keys)
                {
                    if (!RowIndex.Entries.TryGetValue(rowId, out var rowEntry)) continue;
                    var cellIndex = EnsureCellIndexLoaded(rowId, reader, tableBaseOffset, forceReloadIndex);
                    ReadCellInternal(rowId, colId, cellIndex, reader, tableBaseOffset, rowEntry.Offset, preloadReferences, idHeader, idReader, visited);
                }
            }

            /// <summary>指定した複数のcolKey(列)だけをロードする。テーブル全体はロードしない。</summary>
            public static void ReadManyColumns(IEnumerable<TCol> colIds, ClassDataMatrixHeader header, BinaryReader reader, bool preloadReferences = false, ClassDataHeader idHeader = null, BinaryReader idReader = null, bool forceReloadIndex = false)
            {
                foreach (var colId in colIds) ReadOneColumn(colId, header, reader, preloadReferences, idHeader, idReader, forceReloadIndex);
            }

            /// <summary>指定した1つのcolKey(列全体)だけをアンロードする（テーブル全体は消さない）</summary>
            public static void UnloadOneColumn(TCol colId)
            {
                foreach (var rowDict in Table.Values) rowDict.Remove(colId);
            }

            /// <summary>指定した複数のcolKeyだけをアンロードする（テーブル全体は消さない）</summary>
            public static void UnloadManyColumns(IEnumerable<TCol> colIds)
            {
                foreach (var colId in colIds) UnloadOneColumn(colId);
            }

            /// <summary>条件(predicate)に合致するcolKey(列)を一括アンロードする</summary>
            public static void UnloadColumnsWhere(Func<TCol, bool> predicate)
            {
                if (s_colKeys == null) return;
                var colsToRemove = new List<TCol>();
                foreach (var ck in s_colKeys)
                {
                    if (predicate(ck)) colsToRemove.Add(ck);
                }
                foreach (var ck in colsToRemove) UnloadOneColumn(ck);
            }

            // ========================= セル単位 =========================

            /// <summary>指定した1つのセル(rowKey×colKey)だけをロードする。</summary>
            public static void ReadOneCell(TRow rowId, TCol colId, ClassDataMatrixHeader header, BinaryReader reader, bool preloadReferences = false, ClassDataHeader idHeader = null, BinaryReader idReader = null, bool forceReloadIndex = false)
            {
                if (!header.Entries.TryGetValue(TableId, out var tableEntry)) return;
                long tableBaseOffset = tableEntry.Offset;

                EnsureRowIndexLoaded(reader, tableBaseOffset, forceReloadIndex);
                if (!RowIndex.Entries.TryGetValue(rowId, out var rowEntry)) return;
                var cellIndex = EnsureCellIndexLoaded(rowId, reader, tableBaseOffset, forceReloadIndex);
                var visited = new HashSet<(TableID, int)>();
                ReadCellInternal(rowId, colId, cellIndex, reader, tableBaseOffset, rowEntry.Offset, preloadReferences, idHeader, idReader, visited);
            }

            /// <summary>指定した複数のセル(rowKey×colKeyの組)だけをロードする。</summary>
            public static void ReadManyCells(IEnumerable<(TRow Row, TCol Col)> cells, ClassDataMatrixHeader header, BinaryReader reader, bool preloadReferences = false, ClassDataHeader idHeader = null, BinaryReader idReader = null, bool forceReloadIndex = false)
            {
                foreach (var cell in cells) ReadOneCell(cell.Row, cell.Col, header, reader, preloadReferences, idHeader, idReader, forceReloadIndex);
            }

            /// <summary>指定した1つのセルだけをアンロードする</summary>
            public static void UnloadOneCell(TRow rowId, TCol colId)
            {
                if (Table.TryGetValue(rowId, out var rowDict)) rowDict.Remove(colId);
            }

            /// <summary>指定した複数のセルだけをアンロードする</summary>
            public static void UnloadManyCells(IEnumerable<(TRow Row, TCol Col)> cells)
            {
                foreach (var cell in cells) UnloadOneCell(cell.Row, cell.Col);
            }

            /// <summary>条件(predicate)に合致するセルを一括アンロードする</summary>
            public static void UnloadCellsWhere(Func<TRow, TCol, E, bool> predicate)
            {
                var toRemove = new List<(TRow, TCol)>();
                foreach (var rowKv in Table)
                {
                    foreach (var colKv in rowKv.Value)
                    {
                        if (predicate(rowKv.Key, colKv.Key, colKv.Value)) toRemove.Add((rowKv.Key, colKv.Key));
                    }
                }
                foreach (var pair in toRemove) UnloadOneCell(pair.Item1, pair.Item2);
            }

            /// <summary>
            /// テーブル全体をロードする（既存のRead(reader)を利用、高速な連続読みはそのまま）。
            /// preloadReferences=trueの場合、idHeader/idReader経由で全セルの参照先も(ネストして)連鎖的にロードする。
            /// </summary>
            public virtual void ReadAll(ClassDataMatrixHeader header, BinaryReader reader, bool preloadReferences = false, ClassDataHeader idHeader = null, BinaryReader idReader = null)
            {
                Read(reader);
                if (!preloadReferences || idHeader == null || idReader == null) return;

                var visited = new HashSet<(TableID, int)>();
                foreach (var rowDict in Table.Values)
                {
                    foreach (var cell in rowDict.Values)
                    {
                        PreloadCellReferences(cell, idHeader, idReader, visited);
                    }
                }
            }
        }
    }
