using System.IO;
    using System;
    using System.Collections.Generic;

    namespace GameCore.Tables
    {
        public abstract class BaseTableMatrix : BaseTable
        {

            public override abstract void Read(BinaryReader reader);
            public override abstract void Release();

        }
    }
