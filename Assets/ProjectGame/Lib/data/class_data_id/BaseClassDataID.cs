using System.IO;
    using System;
    using System.Collections.Generic;
    using GameCore.Enums;

    namespace GameCore.Tables
    {
        public abstract class BaseClassDataID<T,E> : BaseTable where T : Enum where E : BaseClassDataRow, new()
        {
            public static Dictionary<T,E> Table = new Dictionary<T,E>();

            // 各テーブルの静的コンストラクタで {Name}RowIndex と TableId がセットされる
            protected static BaseClassDataRowIndex<T> RowIndex;
            protected static TableID TableId;

            public override abstract void Read(BinaryReader reader);
            public override void Release()
            {
                Table.Clear();
            }

            /// <summary>
            /// 依存先プリロードのレジストリに自分自身を登録する。各テーブルの静的コンストラクタから呼ぶこと。
            /// </summary>
            protected static void RegisterReferenceLoader()
            {
                ClassDataReferenceLoader.Loaders[TableId] = (refId, header, reader, preloadReferences, forceReloadIndex, visited) =>
                {
                    T typedId = (T)Enum.ToObject(typeof(T), refId);
                    ReadOneInternal(typedId, header, reader, preloadReferences, forceReloadIndex, visited);
                };
            }

            /// <summary>
            /// 行インデックス（idごとのシーク位置）だけを読み込む。既に読み込み済みならスキップ（forceReload=trueで再読み込み）。
            /// </summary>
            protected static void EnsureRowIndexLoaded(BinaryReader reader, long tableBaseOffset, bool forceReload = false)
            {
                if (RowIndex.IsRead && !forceReload) return;

                reader.BaseStream.Seek(tableBaseOffset, SeekOrigin.Begin);
                int rowCount = reader.ReadInt32();
                int colCount = reader.ReadInt32();
                for (int i = 0; i < colCount; i++)
                {
                    int nameLen = reader.ReadInt32();
                    reader.ReadBytes(nameLen);
                    int typeLen = reader.ReadInt32();
                    reader.ReadBytes(typeLen);
                }
                // ここでreaderは行インデックスブロックの先頭に位置している
                RowIndex.Read(reader, forceReload);
            }

            /// <summary>
            /// 実際に1行読み込む内部処理。preloadReferences=trueの場合、この行が参照している他テーブルのidも
            /// (ネストして)連鎖的にロードする。visitedで循環参照を防ぐ。
            /// </summary>
            private static void ReadOneInternal(T id, ClassDataHeader header, BinaryReader reader, bool preloadReferences, bool forceReloadIndex, HashSet<(TableID, int)> visited)
            {
                var visitKey = (TableId, Convert.ToInt32(id));
                if (visited.Contains(visitKey)) return; // 循環参照ガード
                visited.Add(visitKey);

                if (!header.Entries.TryGetValue(TableId, out var tableEntry)) return;
                long tableBaseOffset = tableEntry.Offset;

                EnsureRowIndexLoaded(reader, tableBaseOffset, forceReloadIndex);
                if (!RowIndex.Entries.TryGetValue(id, out var entry)) return;

                reader.BaseStream.Seek(tableBaseOffset + entry.Offset, SeekOrigin.Begin);
                reader.ReadInt32(); // 行データ先頭のid(int)を読み飛ばす（idは引数側で分かっているため）
                E row = new E();
                row.Read(Convert.ToInt32(id), reader);
                Table[id] = row;

                if (preloadReferences)
                {
                    foreach (var reference in row.GetReferencedIds())
                    {
                        if (ClassDataReferenceLoader.Loaders.TryGetValue(reference.TableId, out var loader))
                        {
                            loader(reference.RefId, header, reader, true, forceReloadIndex, visited);
                        }
                    }
                }
            }

            /// <summary>
            /// 指定した1つのid(行)だけをロードする。テーブル全体はロードしない。
            /// TableId(マスターのTableID)は各テーブルの静的コンストラクタで既に設定済みのため、呼び出し側は意識しなくてよい。
            /// preloadReferences=trueで、この行が参照している他テーブルのidも(ネストして)連鎖的にロードする。
            /// </summary>
            public static void ReadOne(T id, ClassDataHeader header, BinaryReader reader, bool preloadReferences = false, bool forceReloadIndex = false)
            {
                ReadOneInternal(id, header, reader, preloadReferences, forceReloadIndex, new HashSet<(TableID, int)>());
            }

            /// <summary>
            /// 指定した複数のid(行)だけをロードする。テーブル全体はロードしない。
            /// preloadReferences=trueの場合、バッチ全体で1つのvisitedセットを共有するため、同じ参照先の二重ロードを避けられる。
            /// </summary>
            public static void ReadMany(IEnumerable<T> ids, ClassDataHeader header, BinaryReader reader, bool preloadReferences = false, bool forceReloadIndex = false)
            {
                var visited = new HashSet<(TableID, int)>();
                foreach (var id in ids)
                {
                    ReadOneInternal(id, header, reader, preloadReferences, forceReloadIndex, visited);
                }
            }

            /// <summary>
            /// テーブル全体をロードする（既存のRead(reader)を利用、高速な連続読みはそのまま）。
            /// preloadReferences=trueで、全行が参照している他テーブルのidも(ネストして)連鎖的にロードする。
            /// </summary>
            public virtual void ReadAll(ClassDataHeader header, BinaryReader reader, bool preloadReferences = false)
            {
                Read(reader);
                if (!preloadReferences) return;

                var visited = new HashSet<(TableID, int)>();
                foreach (var id in Table.Keys) visited.Add((TableId, Convert.ToInt32(id)));

                foreach (var row in Table.Values)
                {
                    foreach (var reference in row.GetReferencedIds())
                    {
                        if (ClassDataReferenceLoader.Loaders.TryGetValue(reference.TableId, out var loader))
                        {
                            loader(reference.RefId, header, reader, true, false, visited);
                        }
                    }
                }
            }

            /// <summary>指定した1つのidだけをアンロードする（テーブル全体は消さない）</summary>
            public static void UnloadOne(T id)
            {
                Table.Remove(id);
            }

            /// <summary>指定した複数のidだけをアンロードする（テーブル全体は消さない）</summary>
            public static void UnloadMany(IEnumerable<T> ids)
            {
                foreach (var id in ids) Table.Remove(id);
            }

            /// <summary>条件(predicate)に合致するidを一括アンロードする</summary>
            public static void UnloadWhere(Func<T, E, bool> predicate)
            {
                var keysToRemove = new List<T>();
                foreach (var kv in Table)
                {
                    if (predicate(kv.Key, kv.Value)) keysToRemove.Add(kv.Key);
                }
                foreach (var key in keysToRemove) Table.Remove(key);
            }
        }
    }
