using System.IO;

    namespace GameCore.Classes
    {
        [System.Serializable]
        public abstract class BaseCustomClassData
        {
            public abstract void Read(BinaryReader reader);
        }
    }
