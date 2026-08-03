
using System.Collections.Generic;
using GameCore.Enums;
namespace GameCore.Gameobject
{
    public class GameObjectDatabase
    {
        [System.Serializable]
        public class GameObjectData
        {
            private readonly string idName;
            private readonly string addressablePath;
            private readonly GameObjectID gameObjectID;
            private readonly int subGroupId;
            public GameObjectData(GameObjectID gameObjectID, string idName, string addressablePath, int subGroupId = 0)
            {
                this.idName = idName;
                this.addressablePath = addressablePath;
                this.gameObjectID = gameObjectID;
                this.subGroupId = subGroupId;
            }
            public string IdName => idName;
            public string AddressablePath => addressablePath;
            public GameObjectID GameObjectID => gameObjectID;
            // SubGroup ID（0 = SubGroupなし）。専用enum(例:GameObject_EnemyID)にキャストして使う
            public int SubGroupId => subGroupId;
        }

        [System.Serializable]
        public class GroupedGameObjects
        {
            private readonly GameObjectGroup group;
            private readonly List<GameObjectData> gameObjects;
            public GroupedGameObjects(GameObjectGroup group, List<GameObjectData> gameObjects)
            {
                this.group = group;
                this.gameObjects = gameObjects ?? new List<GameObjectData>();
            }
            public GameObjectGroup Group => group;
            public List<GameObjectData> GameObjects => gameObjects;
        }

        private readonly List<GroupedGameObjects> groupedGameObjects;
        public GameObjectDatabase()
        {
            groupedGameObjects = new List<GroupedGameObjects>();
        }
        public List<GroupedGameObjects> GroupedGameObjectsList => groupedGameObjects;
    }
}
