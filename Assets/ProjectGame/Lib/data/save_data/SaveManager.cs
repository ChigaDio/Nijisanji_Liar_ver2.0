
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameCore.SaveSystem
{


    public class SaveManager
    {
        private readonly string systemDataPath;
        private readonly string playerDataPath;
        private readonly CancellationTokenSource cts;
        private readonly byte[] encryptionKey = { 0xA1, 0xB2, 0xC3, 0xD4, 0xE5, 0xF6, 0x07, 0x18 };
        public SystemData SystemSettings { get; private set; } = new SystemData();
        public PlayerData PlayerProgress { get; private set; } = new PlayerData();
        public bool IsSaving { get; private set; }
        public bool IsLoading { get; private set; }

        public SaveManager(GameObject linkedGameObject)
        {
            string saveDir;
#if UNITY_EDITOR
            saveDir = Path.Combine(Application.dataPath, "SaveData");
#else
            saveDir = Path.Combine(Application.dataPath, "SaveData");
#endif
            Directory.CreateDirectory(saveDir);

            systemDataPath = Path.Combine(saveDir, "systemData.bytes");
            playerDataPath = Path.Combine(saveDir, "playerData.bytes");

            cts = new CancellationTokenSource();
            if (linkedGameObject != null)
            {
                CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
                linkedGameObject.GetCancellationTokenOnDestroy().Register(() => linkedCts.Cancel());
            }
        }

        private byte[] EncryptDecrypt(byte[] data)
        {
            byte[] result = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (byte)(data[i] ^ encryptionKey[i % encryptionKey.Length]);
            }
            return result;
        }

        private byte[] SerializeToBinary<T>(T data)
        {
            string json = JsonUtility.ToJson(data);
            return System.Text.Encoding.UTF8.GetBytes(json);
        }

        private T DeserializeFromBinary<T>(byte[] data)
        {
            string json = System.Text.Encoding.UTF8.GetString(data);
            return JsonUtility.FromJson<T>(json);
        }


        public async UniTask LoadAllDataAsync(Action onComplete = null)
        {
            try
            {
                IsLoading = true;
                await UniTask.WhenAll(
                    LoadSystemDataAsync(),
                    LoadPlayerDataAsync()
                );
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("LoadAllDataAsync���L�����Z������܂����B");
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadAllDataAsync�ŃG���[: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                onComplete?.Invoke();
            }
        }

        public async UniTask SaveAllDataAsync(Action onComplete = null)
        {
            try
            {
                IsSaving = true;
                await UniTask.WhenAll(
                    SaveSystemDataAsync(),
                    SavePlayerDataAsync()
                );
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("SaveAllDataAsync���L�����Z������܂����B");
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveAllDataAsync�ŃG���[: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
                onComplete?.Invoke();
            }
        }

        public async UniTask LoadSystemDataAsync(Action onComplete = null)
        {
            try
            {
                IsLoading = true;
                await UniTask.RunOnThreadPool(() =>
                {
                    if (File.Exists(systemDataPath))
                    {
                        byte[] encryptedData = File.ReadAllBytes(systemDataPath);
                        byte[] decryptedData = EncryptDecrypt(encryptedData);
                        SystemSettings = DeserializeFromBinary<SystemData>(decryptedData) ?? new SystemData();
                        Debug.Log($"�V�X�e���f�[�^��ǂݍ��݂܂���: {systemDataPath}");
                    }
                    else
                    {
                        SystemSettings = new SystemData();
                        SaveSystemDataAsync().Forget();
                        Debug.Log($"�V�X�e���f�[�^�t�@�C����������܂���ł����B�f�t�H���g���g�p: {systemDataPath}");
                    }
                }, cancellationToken: cts.Token);
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("LoadSystemDataAsync���L�����Z������܂����B");
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadSystemDataAsync�ŃG���[: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                onComplete?.Invoke();
            }
        }

        public async UniTask SaveSystemDataAsync(Action onComplete = null)
        {
            try
            {
                IsSaving = true;
                await UniTask.RunOnThreadPool(() =>
                {
                    byte[] data = SerializeToBinary(SystemSettings);
                    byte[] encryptedData = EncryptDecrypt(data);
                    File.WriteAllBytes(systemDataPath, encryptedData);
                    Debug.Log($"�V�X�e���f�[�^��ۑ����܂���: {systemDataPath}");
                }, cancellationToken: cts.Token);
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("SaveSystemDataAsync���L�����Z������܂����B");
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveSystemDataAsync�ŃG���[: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
                onComplete?.Invoke();
            }
        }

        public async UniTask LoadPlayerDataAsync(Action onComplete = null)
        {
            try
            {
                IsLoading = true;
                await UniTask.RunOnThreadPool(() =>
                {
                    if (File.Exists(playerDataPath))
                    {
                        byte[] encryptedData = File.ReadAllBytes(playerDataPath);
                        byte[] decryptedData = EncryptDecrypt(encryptedData);
                        PlayerProgress = DeserializeFromBinary<PlayerData>(decryptedData) ?? new PlayerData();
                        Debug.Log($"�v���C���[�f�[�^��ǂݍ��݂܂���: {playerDataPath}");
                    }
                    else
                    {
                        PlayerProgress = new PlayerData();
                        SavePlayerDataAsync().Forget();
                        Debug.Log($"�v���C���[�f�[�^�t�@�C����������܂���ł����B�V�K�쐬: {playerDataPath}");
                    }
                }, cancellationToken: cts.Token);
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("LoadPlayerDataAsync���L�����Z������܂����B");
            }
            catch (Exception ex)
            {
                Debug.LogError($"LoadPlayerDataAsync�ŃG���[: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
                onComplete?.Invoke();
            }
        }

        public async UniTask SavePlayerDataAsync(Action onComplete = null)
        {
            try
            {
                IsSaving = true;
                await UniTask.RunOnThreadPool(() =>
                {
                    byte[] data = SerializeToBinary(PlayerProgress);
                    byte[] encryptedData = EncryptDecrypt(data);
                    File.WriteAllBytes(playerDataPath, encryptedData);
                    Debug.Log($"�v���C���[�f�[�^��ۑ����܂���: {playerDataPath}");
                }, cancellationToken: cts.Token);
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("SavePlayerDataAsync���L�����Z������܂����B");
            }
            catch (Exception ex)
            {
                Debug.LogError($"SavePlayerDataAsync�ŃG���[: {ex.Message}");
            }
            finally
            {
                IsSaving = false;
                onComplete?.Invoke();
            }
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
        }
    }
}
        