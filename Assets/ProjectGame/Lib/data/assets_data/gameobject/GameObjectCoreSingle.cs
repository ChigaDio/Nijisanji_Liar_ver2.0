// 自動生成ファイルです。手動編集しても generate 実行時に上書きされます。
using System;
using GameCore.Enums;
using Cysharp.Threading.Tasks;

namespace GameCore.Gameobject
{
    public partial class GameObjectCore
    {
        public static readonly GameObjectID[] _GameObject_Character_PrefabToGameObjectID = new GameObjectID[]
        {
            GameObjectID.None, // GameObject_Character_Prefab.None
            GameObjectID.Character_Ange, // GameObject_Character_Prefab.Ange
            GameObjectID.Character_Ryushen, // GameObject_Character_Prefab.Ryushen
            GameObjectID.Character_Belmond, // GameObject_Character_Prefab.Belmond
            GameObjectID.Character_Himawari, // GameObject_Character_Prefab.Himawari
            GameObjectID.Character_Mashiro, // GameObject_Character_Prefab.Mashiro
            GameObjectID.Character_Kuzuha, // GameObject_Character_Prefab.Kuzuha
        };

        public void LoadSingle(GameObject_Character_PrefabID id, AddressableSystem.GroupCategory groupCategory, Action onCompleted = null)
            => LoadSingle(GameObjectGroup.Character, _GameObject_Character_PrefabToGameObjectID[(int)id], groupCategory, onCompleted);

        public async UniTask LoadSingleAsync(GameObject_Character_PrefabID id, AddressableSystem.GroupCategory groupCategory, Action onCompleted = null)
            => await LoadSingleAsync(GameObjectGroup.Character, _GameObject_Character_PrefabToGameObjectID[(int)id], groupCategory, onCompleted);

        public void UnloadSingle(GameObject_Character_PrefabID id, Action onCompleted = null)
            => UnloadSingle(GameObjectGroup.Character, _GameObject_Character_PrefabToGameObjectID[(int)id], onCompleted);

       public async UniTask UnloadSingleAsync(GameObject_Character_PrefabID id, Action onCompleted = null)
            => await UnloadSingleAsync(GameObjectGroup.Character, _GameObject_Character_PrefabToGameObjectID[(int)id], onCompleted);

       public UnityEngine.GameObject GetGameObject(GameObject_Character_PrefabID id)
            => GetGameObject(GameObjectGroup.Character, _GameObject_Character_PrefabToGameObjectID[(int)id]);

    }
}