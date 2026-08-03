
        using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace GameCore.SaveSystem
{
    [AddComponentMenu("GameCore/Save Manager Core")]
    public class SaveManagerCore : BaseSingleton<SaveManagerCore>
    {
        private SaveManager saveManager;

        public SystemData SystemSettings => saveManager.SystemSettings;
        public PlayerData PlayerProgress => saveManager.PlayerProgress;
        public bool IsSaving => saveManager.IsSaving;
        public bool IsLoading => saveManager.IsLoading;

        public bool IsSaveLoadActionNow()
        {
            return (IsSaving | IsLoading);
        }

        public override void AwakeSingleton()
        {
            base.AwakeSingleton();
            DontDestroyOnLoad(gameObject);
            saveManager = new SaveManager(gameObject);
            LoadAllDataAsync().Forget();
        }

        public async UniTask LoadAllDataAsync(Action onComplete = null)
        {
            await saveManager.LoadAllDataAsync(onComplete);
        }

        public async UniTask SaveAllDataAsync(Action onComplete = null)
        {
            await saveManager.SaveAllDataAsync(onComplete);
        }

        public async UniTask LoadSystemDataAsync(Action onComplete = null)
        {
            await saveManager.LoadSystemDataAsync(onComplete);
        }

        public async UniTask SaveSystemDataAsync(Action onComplete = null)
        {
            await saveManager.SaveSystemDataAsync(onComplete);
        }

        public async UniTask LoadPlayerDataAsync(Action onComplete = null)
        {
            await saveManager.LoadPlayerDataAsync(onComplete);
        }

        public async UniTask SavePlayerDataAsync(Action onComplete = null)
        {
            await saveManager.SavePlayerDataAsync(onComplete);
        }

        private void OnDestroy()
        {
            saveManager?.Dispose();
        }
    }
}
        