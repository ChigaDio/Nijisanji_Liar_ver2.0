
// EventInfo.cs
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GameCore.Scenario
{
    // イベントの情報を格納するクラス
    public class ScenarioEventInfo
    {
        public string EventId { get; set; } // イベントID
        public string EventName { get; set; } // イベント名
        public long EventOffset { get; set; } // イベントのシーク座標
        public List<ScenarioSubEventInfo> SubEvents { get; set; } // サブイベントのリスト

        public ScenarioEventInfo(string eventId, string eventName, long eventOffset,List<ScenarioSubEventInfo> value)
        {
            EventId = eventId ?? string.Empty;
            EventName = eventName ?? string.Empty;
            EventOffset = eventOffset;
            SubEvents = value;
        }

    }
}
