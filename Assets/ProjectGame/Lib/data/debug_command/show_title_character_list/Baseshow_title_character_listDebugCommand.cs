using System;
using UnityEngine;

namespace GameCore.DebugCommand
{
    // show_title_character_list コマンドの引数（自動生成）
    public class show_title_character_listArgs
    {

        public static show_title_character_listArgs FromJson(JsonObject json)
        {
            return new show_title_character_listArgs
            {
            };
        }
    }

    // show_title_character_list コマンドの戻り値（自動生成）。
    // 固有フィールドはここに追加されるが、値の設定は show_title_character_listDebugCommand.Execute() 側（手動実装ファイル）で行う。
    public class show_title_character_listResult : DebugCommandResultBase
    {

        protected override void WriteFields(JsonObject json)
        {
        }
    }

    // 自動生成される基底クラス。このファイルは毎回上書きされます。
    // 実際の処理は show_title_character_listDebugCommand 側（手動実装ファイル）に書いてください。
    public abstract class Baseshow_title_character_listDebugCommand : DebugCommandBase
    {
        public override string CommandName => "show_title_character_list";

        public override JsonObject Invoke(JsonObject argsJson)
        {
            var args = show_title_character_listArgs.FromJson(argsJson);
            var result = new show_title_character_listResult();
            // 時間・コマンド名は自動設定。Execute()側では手動で返したいデータのみ埋めればよい
            result.CommandName = CommandName;
            result.Time = DateTime.Now.ToString("HH:mm:ss.fff");
            Execute(args, result);
            return result.ToJson();
        }

        protected abstract void Execute(show_title_character_listArgs args, show_title_character_listResult result);
    }
}