
// SubEventInfo.cs
using System.IO;
using System.Text;

namespace GameCore.Scenario
{
    // サブイベントの情報を格納するクラス
    public class ScenarioSubEventInfo
    {
        public int SubEventId { get; set; } // サブイベントID (4バイト)
        public string SubEventName { get; set; } // サブイベント名
        public long SubEventOffset { get; set; } // サブイベントのシーク座標 (8バイト)

        public ScenarioSubEventInfo(int subEventId, string subEventName, long subEventOffset)
        {
            SubEventId = subEventId;
            SubEventName = subEventName ?? string.Empty;
            SubEventOffset = subEventOffset;
        }


    }
}
