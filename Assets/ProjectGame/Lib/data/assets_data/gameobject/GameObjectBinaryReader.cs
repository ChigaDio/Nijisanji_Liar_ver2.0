
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GameCore.Enums;
using UnityEngine.AddressableAssets;         
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace GameCore.Gameobject
{
    public class GameObjectBinaryReader
    {
        public static GameObjectDatabase LoadGameObjectDatabaseFromBinary(string filePath, bool addressable = false)
        {
            if (!addressable)
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogError($"Binary file not found: {filePath}");
                    return null;
                }

                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    return ReadDatabase(reader);
                }
            }
            else
            {
                // ====================== Addressableの場合 ======================
                AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(filePath);

                handle.WaitForCompletion();


                if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                {
                    Debug.LogError($"Failed to load Addressable binary: {filePath}");
                    if (handle.IsValid()) Addressables.Release(handle);
                    return null;
                }

                TextAsset textAsset = handle.Result;

                using (MemoryStream ms = new MemoryStream(textAsset.bytes))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    GameObjectDatabase database = ReadDatabase(reader);
                    Addressables.Release(handle);
                    return database;
                }
            }
        }

        public static async UniTask<GameObjectDatabase> LoadGameObjectDatabaseFromBinaryAsync(string filePath, bool addressable = false)
        {
            if (!addressable)
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogError($"Binary file not found: {filePath}");
                    return null;
                }

                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    return ReadDatabase(reader);
                }
            }
            else
            {
                // ====================== Addressableの場合 ======================
                AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(filePath);

                await handle.ToUniTask();


                if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                {
                    Debug.LogError($"Failed to load Addressable binary: {filePath}");
                    if (handle.IsValid()) Addressables.Release(handle);
                    return null;
                }

                TextAsset textAsset = handle.Result;

                byte[] rawBytes = textAsset.bytes;

                using (MemoryStream ms = new MemoryStream(rawBytes))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    GameObjectDatabase database = ReadDatabase(reader);
                    Addressables.Release(handle);
                    return database;
                }
            }
        }

        // 共通読み込みロジック
        private static GameObjectDatabase ReadDatabase(BinaryReader reader)
        {
            GameObjectDatabase database = new GameObjectDatabase();

            int groupCount = reader.ReadInt32();
            int[] offsets = new int[groupCount];

            for (int i = 0; i < groupCount; i++)
            {
                offsets[i] = reader.ReadInt32();
            }

            string[] groupNames = Enum.GetNames(typeof(GameObjectGroup));
            if (groupCount > groupNames.Length - 1)
            {
                Debug.LogError("Binary contains more groups than defined in GameObjectGroup enum.");
                return null;
            }

            for (int i = 0; i < groupCount; i++)
            {
                reader.BaseStream.Seek(offsets[i], SeekOrigin.Begin);
                int gameObjectCount = reader.ReadInt32();
                List<GameObjectDatabase.GameObjectData> gameObjects = new List<GameObjectDatabase.GameObjectData>();

                for (int j = 0; j < gameObjectCount; j++)
                {
                    int gameObjectId = reader.ReadInt32();
                    string idName = ReadNullTerminatedString(reader);
                    string addressablePath = ReadNullTerminatedString(reader);
                    int subGroupId = reader.ReadInt32();

                    gameObjects.Add(new GameObjectDatabase.GameObjectData(
                        gameObjectID: (GameObjectID)gameObjectId,
                        idName: idName,
                        addressablePath: addressablePath,
                        subGroupId: subGroupId
                    ));
                }

                database.GroupedGameObjectsList.Add(new GameObjectDatabase.GroupedGameObjects(
                    group: (GameObjectGroup)(i + 1),
                    gameObjects: gameObjects
                ));
            }

            return database;
        }

        private static string ReadNullTerminatedString(BinaryReader reader)
        {
            List<byte> bytes = new List<byte>();
            byte b;
            while ((b = reader.ReadByte()) != 0)
            {
                bytes.Add(b);
            }
            return System.Text.Encoding.UTF8.GetString(bytes.ToArray());
        }
    }
}
        
        