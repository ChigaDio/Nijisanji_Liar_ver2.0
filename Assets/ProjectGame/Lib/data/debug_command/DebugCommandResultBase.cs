
namespace GameCore.DebugCommand
{
    // 全DebugCommandの戻り値(Result)が継承する基底クラス（自動生成・編集不要）
    //
    //  ・CommandName / Time は Base{Name}DebugCommand.Invoke() が自動的にセットします。
    //    Execute() 内で手動で設定する必要はありません（してはいけません）。
    //  ・TextLog / TextData は、専用の戻り値フィールドを増やすほどではない
    //    簡易なログ文字列や任意のデータ（List<object> / Dictionary<string,object> など、
    //    MiniJson がそのままシリアライズできる型）を返したい場合に使ってください。
    //    例: result.TextData = charactersList; // List<object>
    public abstract class DebugCommandResultBase
    {
        // フレームワークが自動的に設定する。Execute() 内では触らないこと。
        public string CommandName { get; internal set; }
        public string Time { get; internal set; }

        // 手動で使いたい場合の汎用フィールド（不要なら未使用のままでよい）
        public string TextLog;
        public object TextData;

        // 各コマンド固有のフィールドをJsonObjectに詰める処理。
        // 自動生成される {Name}Result 側でオーバーライドされる。
        protected virtual void WriteFields(JsonObject json) { }

        public JsonObject ToJson()
        {
            var json = new JsonObject();
            json["commandName"] = CommandName;
            json["time"] = Time;
            if (TextLog != null) json["textLog"] = TextLog;
            if (TextData != null) json["textData"] = TextData;
            WriteFields(json);
            return json;
        }
    }
}
