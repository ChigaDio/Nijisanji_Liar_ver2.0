


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using AddressableSystem;
using GameCore.Enums;
namespace GameCore.Gameobject
{
    public partial class GameObjectCore : BaseSingleton<GameObjectCore>
    {
        private GameObjectDatabase database;
        private Dictionary<GameObjectGroup, Dictionary<GameObjectID, AddressableData<UnityEngine.GameObject>>> loadedGameObjects =
            new Dictionary<GameObjectGroup, Dictionary<GameObjectID, AddressableData<UnityEngine.GameObject>>>();
        private bool isLoadDatabase = false;
        public bool IsLoadDatabase => isLoadDatabase;
        private CancellationToken destroyToken;

        public override void AwakeSingleton()
        {
            base.AwakeSingleton();
            instance = this;
            DontDestroyOnLoad(gameObject);
            destroyToken = this.GetCancellationTokenOnDestroy();
            LoadDatabaseAsync().Forget();
        }

        private async UniTask LoadDatabaseAsync()
        {
            string path = SupportFiles.ADDRESSABLE_CHECK ? SupportFiles.ALL_GAMEOBJECT_BIN_FILE : SupportFiles.ALL_GAMEOBJECT_BIN;
            database = await GameObjectBinaryReader.LoadGameObjectDatabaseFromBinaryAsync(path, SupportFiles.ADDRESSABLE_CHECK);
            if (database == null)
            {
                Debug.LogError("Failed to load GameObjectDatabase from binary.");
            }
            await UniTask.CompletedTask.AttachExternalCancellation(destroyToken);
            isLoadDatabase = true;
        }

        public void LoadGroup(GameObjectGroup group, GroupCategory groupCategory, Action action = null)
        {
            LoadGroupAsync(group, groupCategory, action).Forget();
        }


        public async UniTask LoadGroupAsync(GameObjectGroup group, GroupCategory groupCategory, Action action = null)
        {
            while (database == null)
            {
                await UniTask.Yield(cancellationToken: destroyToken);
            }
            if (loadedGameObjects.ContainsKey(group)) return;
            var gameObjects = database.GroupedGameObjectsList.FirstOrDefault(data => data.Group == group);
            if (gameObjects == null) return;

            loadedGameObjects[group] = new Dictionary<GameObjectID, AddressableData<UnityEngine.GameObject>>();
            var tasks = new List<UniTask>();

            foreach (var go in gameObjects.GameObjects)
            {
                var addressable = new AddressableData<UnityEngine.GameObject>(groupCategory, AssetCategory.Prefab, go.AddressablePath);
                tasks.Add(addressable.LoadAsync( obj =>
                {
                    if (addressable.IsLoadedAndSetup)
                    {
                        loadedGameObjects[group][go.GameObjectID] = addressable;
                    }
                }, ex =>
                {
                    Debug.LogError($"Failed to load gameobject for {go.GameObjectID} at {go.AddressablePath}: {ex.Message}");
                }).AttachExternalCancellation(destroyToken));
            }

            await UniTask.WhenAll(tasks);
            action?.Invoke();
        }

        public void UnloadGroup(GameObjectGroup group, GroupCategory groupCategory, Action action = null)
        {
            UnloadGroupAsync(group, groupCategory, action).Forget();
        }

        public async UniTask UnloadGroupAsync(GameObjectGroup group, GroupCategory groupCategory, Action action = null)
        {
            if (!loadedGameObjects.TryGetValue(group, out var gameObjects)) return;

            foreach (var addressable in gameObjects.Values)
            {
                addressable.Release();
            }
            loadedGameObjects.Remove(group);
            AddressableDataCore.Instance.ReleaseCategory(groupCategory, AssetCategory.Prefab);
            action?.Invoke();
            await UniTask.CompletedTask.AttachExternalCancellation(destroyToken);
        }

        public void UnloadAll(Action action = null)
        {
            UnloadAllAsync(action).Forget();
        }

        public async UniTask UnloadAllAsync(Action action = null)
        {
            foreach(var group in loadedGameObjects.Values)
            {
                foreach(var data in group.Values)
                {
                    data.Release();
                }
                await UniTask.Yield(destroyToken);
                group.Clear();
            }
            loadedGameObjects.Clear();

            AddressableDataCore.Instance.ReleaseAssetsAll(AssetCategory.Prefab);
            await UniTask.Yield(destroyToken);
            action?.Invoke();
            await UniTask.CompletedTask.AttachExternalCancellation(destroyToken);

        }

        // =============================================================
        // 個別ID単位のロード／アンロード
        // 既存の loadedGameObjects（グループロードと同じキャッシュ）をそのまま使う。
        // 個別専用の管理は持たない。
        // =============================================================
        internal void LoadSingle(GameObjectGroup group, GameObjectID id, GroupCategory groupCategory, Action action = null)
            => LoadSingleAsync(group, id, groupCategory, action).Forget();

        internal async UniTask LoadSingleAsync(GameObjectGroup group, GameObjectID id, GroupCategory groupCategory, Action action = null)
        {
            while (database == null)
            {
                await UniTask.Yield(cancellationToken: destroyToken);
            }
            if (loadedGameObjects.TryGetValue(group, out var existing) && existing.ContainsKey(id))
            {
                action?.Invoke();
                return;
            }

            var groupData = database.GroupedGameObjectsList.FirstOrDefault(d => d.Group == group);
            var target = groupData?.GameObjects.FirstOrDefault(g => g.GameObjectID == id);
            if (target == null) { action?.Invoke(); return; }

            if (!loadedGameObjects.ContainsKey(group))
                loadedGameObjects[group] = new Dictionary<GameObjectID, AddressableData<UnityEngine.GameObject>>();

            var addressable = new AddressableData<UnityEngine.GameObject>(groupCategory, AssetCategory.Prefab, target.AddressablePath);
            await addressable.LoadAsync(obj =>
            {
                if (addressable.IsLoadedAndSetup)
                {
                    loadedGameObjects[group][id] = addressable;
                }
            }, ex =>
            {
                Debug.LogError($"Failed to load single gameobject {id} at {target.AddressablePath}: {ex.Message}");
            }).AttachExternalCancellation(destroyToken);

            action?.Invoke();
        }

        public void UnloadSingle(GameObjectGroup group, GameObjectID id, Action action = null)
            => UnloadSingleAsync(group, id, action).Forget();

        public async UniTask UnloadSingleAsync(GameObjectGroup group, GameObjectID id, Action action = null)
        {
            if (loadedGameObjects.TryGetValue(group, out var dict) && dict.TryGetValue(id, out var addressable))
            {
                addressable.ReleaseAndUntrack();
                dict.Remove(id);
            }
            action?.Invoke();
            await UniTask.CompletedTask.AttachExternalCancellation(destroyToken);
        }

        // =============================================================
        // SubGroup単位のロード／アンロード（内部実装）
        // 公開APIは {Category}CoreSubGroups.cs 側で、グループごとの
        // 専用enum（例: GameObject_EnemyID）を受け取るオーバーロードとして生成される。
        // どのアイテムがどのSubGroupに属するかは GameObjectData.SubGroupId
        // （バイナリ生成時に書き出し済み）から都度判定する。
        // 既存の loadedGameObjects キャッシュをそのまま使い、専用の管理は持たない。
        // =============================================================
        internal void LoadSubGroupInternal(GameObjectGroup group, int subGroupId, GroupCategory groupCategory, Action action = null)
            => LoadSubGroupInternalAsync(group, subGroupId, groupCategory, action).Forget();

        internal async UniTask LoadSubGroupInternalAsync(GameObjectGroup group, int subGroupId, GroupCategory groupCategory, Action action = null)
        {
            while (database == null)
            {
                await UniTask.Yield(cancellationToken: destroyToken);
            }
            var groupData = database.GroupedGameObjectsList.FirstOrDefault(d => d.Group == group);
            if (groupData == null) { action?.Invoke(); return; }

            if (!loadedGameObjects.ContainsKey(group))
                loadedGameObjects[group] = new Dictionary<GameObjectID, AddressableData<UnityEngine.GameObject>>();

            var tasks = new List<UniTask>();
            foreach (var go in groupData.GameObjects)
            {
                if (go.SubGroupId != subGroupId) continue;
                if (loadedGameObjects[group].ContainsKey(go.GameObjectID)) continue;

                var addressable = new AddressableData<UnityEngine.GameObject>(groupCategory, AssetCategory.Prefab, go.AddressablePath);
                tasks.Add(addressable.LoadAsync(obj =>
                {
                    if (addressable.IsLoadedAndSetup)
                        loadedGameObjects[group][go.GameObjectID] = addressable;
                }, ex =>
                {
                    Debug.LogError($"Failed to load gameobject for {go.GameObjectID} at {go.AddressablePath}: {ex.Message}");
                }).AttachExternalCancellation(destroyToken));
            }

            await UniTask.WhenAll(tasks);
            action?.Invoke();
        }

        internal void UnloadSubGroupInternal(GameObjectGroup group, int subGroupId, Action action = null)
            => UnloadSubGroupInternalAsync(group, subGroupId, action).Forget();

        internal async UniTask UnloadSubGroupInternalAsync(GameObjectGroup group, int subGroupId, Action action = null)
        {
            if (loadedGameObjects.TryGetValue(group, out var dict) && database != null)
            {
                var groupData = database.GroupedGameObjectsList.FirstOrDefault(d => d.Group == group);
                if (groupData != null)
                {
                    foreach (var go in groupData.GameObjects)
                    {
                        if (go.SubGroupId != subGroupId) continue;
                        if (dict.TryGetValue(go.GameObjectID, out var addressable))
                        {
                            addressable.ReleaseAndUntrack();
                            dict.Remove(go.GameObjectID);
                        }
                    }
                }
            }

            action?.Invoke();
            await UniTask.CompletedTask.AttachExternalCancellation(destroyToken);
        }

        public UnityEngine.GameObject GetGameObject(GameObjectGroup group, GameObjectID id)
        {
            if (loadedGameObjects.TryGetValue(group, out var groupGameObjects) && groupGameObjects.TryGetValue(id, out var addressable))
            {
                return addressable.GetAddressableObjectResult();
            }
            return null;
        }

        private void OnDestroy()
        {
            foreach (var group in loadedGameObjects.Values)
            {
                foreach (var go in group.Values)
                {
                    go.Release();
                }
            }
            loadedGameObjects.Clear();
        }
    }
}

