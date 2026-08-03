using System.IO;
    using System;
    using System.Collections.Generic;

    namespace GameCore.Tables
    {
        public abstract class BaseTable
        {

            public abstract void Read(BinaryReader reader);
            public abstract void Release();

        }
    }
