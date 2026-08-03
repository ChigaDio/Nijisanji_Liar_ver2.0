

using Cysharp.Threading.Tasks;
using GameCore.Tables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace GameCore.Scenario
{
    // ヘッダー全体を管理するクラス
    public class ScenarioEventBinaryHeader
    {
        // staticなフィールドでヘッダー情報を保持
        private static List<ScenarioEventInfo> _events = null;

        // 読み込んだイベントリストを返すプロパティ
        public static List<ScenarioEventInfo> Events
        {
            get
            {
                if (_events == null)
                {
                    _events = new List<ScenarioEventInfo>();
                }
                return _events;
            }
            private set
            {
                _events = value;
            }
        }

        public static long GetEventSeekPos(string eventName,string subName)
        {
            var find = _events.Find(data => data.EventId == eventName);
            var findSub = find.SubEvents.Find(data => data.SubEventName == subName);
            return findSub.SubEventOffset;
        }
        public static long GetEventSeekPos(string eventName, int subID)
        {
            var find = _events.Find(data => data.EventId == eventName);
            var findSub = find.SubEvents.Find(data => data.SubEventId == subID);
            return findSub.SubEventOffset;
        }
        public static async UniTask ReadHeaderAsync(Action action = null, bool addressable = false)
        {
            Stream stream = null;
            AsyncOperationHandle<TextAsset> handle = default;

            try
            {
                (stream, handle) = await GetDataStreamAsync(addressable);

                using var reader = new BinaryReader(stream, Encoding.UTF8);

                int eventCount = reader.ReadInt32();
                if (eventCount <= 0)
                    throw new InvalidDataException($"Invalid event count: {eventCount}");

                Events.Clear();

                for (int i = 0; i < eventCount; i++)
                {
                    var eventInfo = await ReadScenarioEventInfoAsync(reader, stream);
                    Events.Add(eventInfo);

                    await UniTask.Yield(); // 1フレームに1イベント処理（重い場合のフリーズ防止）
                }

                action?.Invoke();
            }
            finally
            {
                stream?.Dispose();

                if (handle.IsValid())
                    Addressables.Release(handle);
            }
        }

        /// <summary>
        /// Addressable または FileStream から Stream と Handle を取得
        /// </summary>
        private static async UniTask<(Stream stream, AsyncOperationHandle<TextAsset> handle)> GetDataStreamAsync(bool addressable)
        {
            if (addressable)
            {
                var handle = Addressables.LoadAssetAsync<TextAsset>(SupportFiles.ALL_SCENARIO_EVENT_BIN_FILE);
                TextAsset textAsset = await handle.ToUniTask();

                if (textAsset == null)
                {
                    Debug.LogError($"Failed to load Addressable binary: {SupportFiles.ALL_SCENARIO_EVENT_BIN_FILE}");
                    if (handle.IsValid()) Addressables.Release(handle);
                    throw new InvalidOperationException("Failed to load scenario event binary from Addressables.");
                }

                return (new MemoryStream(textAsset.bytes), handle);
            }
            else
            {
                var stream = new FileStream(
                    SupportFiles.ALL_SCENARIO_EVENTS_BIN,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

                return (stream, default);
            }
        }

        /// <summary>
        /// 1つのイベント（サブイベント含む）を読み込む
        /// </summary>
        private static async UniTask<ScenarioEventInfo> ReadScenarioEventInfoAsync(BinaryReader reader, Stream baseStream)
        {
            // Event ID
            string eventId = ReadLengthPrefixedString(reader, baseStream, "event ID");

            // Event Name
            string eventName = ReadLengthPrefixedString(reader, baseStream, "event name");

            // Event Offset
            long eventOffset = reader.ReadInt64();
            ValidateOffset(eventOffset, baseStream, "event offset");

            // Sub Events
            int subEventCount = reader.ReadInt32();
            ValidateCount(subEventCount, 1000, "subEvent count", baseStream);

            var subEvents = new List<ScenarioSubEventInfo>(subEventCount);

            for (int j = 0; j < subEventCount; j++)
            {
                int subEventId = reader.ReadInt32();
                string subEventName = ReadLengthPrefixedString(reader, baseStream, "subEvent name");

                long subEventOffset = reader.ReadInt64();
                ValidateOffset(subEventOffset, baseStream, "subEvent offset");

                subEvents.Add(new ScenarioSubEventInfo(subEventId, subEventName, subEventOffset));

                await UniTask.Yield(PlayerLoopTiming.Initialization);
            }

            return new ScenarioEventInfo(eventId, eventName, eventOffset, subEvents);
        }

        /// <summary>
        /// 長さプレフィックス付きの文字列を安全に読み込む
        /// </summary>
        private static string ReadLengthPrefixedString(BinaryReader reader, Stream baseStream, string fieldName)
        {
            int length = reader.ReadInt32();
            ValidateCount(length, 1000, $"{fieldName} length", baseStream);

            if (length == 0)
                return string.Empty;

            byte[] bytes = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// カウント値（長さや個数）のバリデーション
        /// </summary>
        private static void ValidateCount(int count, int max, string fieldName, Stream stream)
        {
            if (count < 0 || count > max)
            {
                throw new InvalidDataException(
                    $"Invalid {fieldName}: {count} at position {stream.Position - 4}");
            }
        }

        /// <summary>
        /// オフセット値のバリデーション
        /// </summary>
        private static void ValidateOffset(long offset, Stream stream, string fieldName)
        {
            if (offset < 0 || offset > stream.Length)
            {
                throw new InvalidDataException(
                    $"Invalid {fieldName}: {offset} at position {stream.Position - 8}");
            }
        }

        // イベント名とサブイベントIDからサブイベントのシーク座標を取得するメソッド
        public static long GetSubEventOffset(string eventName, int subEventId)
        {
            if (_events == null)
            {
                throw new InvalidOperationException("Header has not been loaded. Call ReadHeaderAsync first.");
            }

            foreach (var eventInfo in _events)
            {
                if (eventInfo.EventName == eventName)
                {
                    foreach (var subEventInfo in eventInfo.SubEvents)
                    {
                        if (subEventInfo.SubEventId == subEventId)
                        {
                            return subEventInfo.SubEventOffset;
                        }
                    }
                }
            }

            throw new KeyNotFoundException($"SubEvent with ID {subEventId} in Event '{eventName}' not found.");
        }
    }
}

