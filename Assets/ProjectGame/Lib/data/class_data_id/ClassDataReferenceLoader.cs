using System.IO;
    using System;
    using System.Collections.Generic;
    using GameCore.Enums;

    namespace GameCore.Tables
    {
        public static class ClassDataReferenceLoader
        {
            // (参照先id, header, reader, preloadReferences, forceReloadIndex, 循環参照防止用visited)
            public delegate void LoadOneDelegate(int refId, ClassDataHeader header, BinaryReader reader, bool preloadReferences, bool forceReloadIndex, HashSet<(TableID, int)> visited);

            public static readonly Dictionary<TableID, LoadOneDelegate> Loaders = new Dictionary<TableID, LoadOneDelegate>();
        }
    }
