
        
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GameCore.Enums;
using UnityEngine.AddressableAssets;         
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace GameCore.Sound
{
    public class SoundBinaryReader
    {

        public static async UniTask<SoundDatabase> LoadSoundDatabaseFromBinaryAsync(string filePath, bool addressable = false)
        {
            if (!addressable)
            {
                if (!File.Exists(filePath))
                {
                    UnityEngine.Debug.LogError($"Binary file not found: {filePath}");
                    return null;
                }

                return await UniTask.RunOnThreadPool(() =>
                {
                    using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                    {
                        return ReadDatabase(reader);
                    }
                });
            }
            else
            {
                // ====================== Addressableの場合 ======================

                AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(filePath);

                await handle.ToUniTask();   // ここはメインスレッドで待機完了

                if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
                {
                    UnityEngine.Debug.LogError($"Failed to load Addressable binary: {filePath}");
                    if (handle.IsValid()) Addressables.Release(handle);
                    return null;
                }

                TextAsset textAsset = handle.Result;

                // ★★★ ここでメインスレッド上で .bytes を取得 ★★★
                byte[] rawBytes = textAsset.bytes;        // ← これを先に取る！

                // 解析だけ別スレッドに逃がす
                SoundDatabase database;
                using (MemoryStream ms = new MemoryStream(rawBytes))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    database = ReadDatabase(reader);
                }

                Addressables.Release(handle);

                return database;
            }
        }
        public static SoundDatabase LoadSoundDatabaseFromBinary(string filePath, bool addressable = false)
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
                // sound_data.bytes
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
                    SoundDatabase database = ReadDatabase(reader);
                    Addressables.Release(handle);
                    return database;
                }
            }
        }

        // 共通読み込みロジック
        private static SoundDatabase ReadDatabase(BinaryReader reader)
        {
            SoundDatabase database = new SoundDatabase();

            int groupCount = reader.ReadInt32();
            int[] offsets = new int[groupCount];

            for (int i = 0; i < groupCount; i++)
            {
                offsets[i] = reader.ReadInt32();
            }

            string[] groupNames = Enum.GetNames(typeof(SoundGroup));
            if (groupCount > groupNames.Length - 1)
            {
                Debug.LogError("Binary contains more groups than defined in SoundGroup enum.");
                return null;
            }

            for (int i = 0; i < groupCount; i++)
            {
                reader.BaseStream.Seek(offsets[i], SeekOrigin.Begin);
                int soundCount = reader.ReadInt32();
                List<SoundDatabase.SoundData> sounds = new List<SoundDatabase.SoundData>();

                for (int j = 0; j < soundCount; j++)
                {
                    int id = reader.ReadInt32();
                    string addressablePath = ReadNullTerminatedString(reader);
                    float volume = reader.ReadSingle();
                    byte typeByte = reader.ReadByte();
                    SoundType type = (typeByte == 0) ? SoundType.SE : SoundType.BGM;
                    int subGroupId = reader.ReadInt32();

                    string enumName = Enum.GetName(typeof(SoundID), id) ?? $"Unknown_{id}";

                    sounds.Add(new SoundDatabase.SoundData(
                        idName: enumName,
                        addressablePath: addressablePath,
                        baseVolume: volume,
                        type: type,
                        soundID: (SoundID)id,
                        subGroupId: subGroupId
                    ));
                }

                database.GroupedSoundsList.Add(new SoundDatabase.GroupedSounds(
                    group: (SoundGroup)(i + 1),
                    sounds: sounds
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

