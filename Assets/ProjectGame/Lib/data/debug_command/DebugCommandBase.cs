

using System;
using System.Collections.Generic;

namespace GameCore.DebugCommand
{
    // 全DebugCommandの基底クラス（自動生成・編集不要）
    public abstract class DebugCommandBase
    {
        public abstract string CommandName { get; }

        // JS側から送られてきた引数(JsonObject)を受け取り実行し、結果(JsonObjectまたはnull)を返す
        public abstract JsonObject Invoke(JsonObject argsJson);
    }

    // 名前でDebugCommandを管理し、WebSocket経由で受信したメッセージをディスパッチする。
    // DebugCommandWebSocketHandler.cs から自動的に呼び出される。
    public static class DebugCommandRegistry
    {
        private static readonly Dictionary<string, DebugCommandBase> _commands = new Dictionary<string, DebugCommandBase>();


        public static void Register(DebugCommandBase command)
        {
            _commands[command.CommandName] = command;
        }

        public static bool TryGet(string name, out DebugCommandBase command)
        {
            return _commands.TryGetValue(name, out command);
        }

        // dbgServer.py から届いたJSON文字列を解析し、対応するDebugCommandを実行して
        // 応答用のJSON文字列を返す（type:"command" 以外のメッセージが来た場合はnullを返す）。
        public static string Dispatch(string receivedJson)
        {
            JsonObject root;
            try
            {
                root = JsonObject.Parse(receivedJson);
            }
            catch (Exception e)
            {
                return BuildError(null, null, $"JSON parse error: {e.Message}");
            }

            if (root.GetString("type") != "command")
            {
                return null;
            }

            string commandId = root.GetString("id", null);
            string name = root.GetString("name", null);
            var args = root.GetObject("args") ?? new JsonObject();

            if (!TryGet(name, out var command))
            {
                return BuildError(commandId, name, $"Unknown command: {name}");
            }

            try
            {
                var result = command.Invoke(args);
                var response = new JsonObject();
                response["type"] = "response";
                response["id"] = commandId;
                response["name"] = name;
                response["time"] = DateTime.Now.ToString("HH:mm:ss.fff");
                response["data"] = result;
                return response.ToString();
            }
            catch (Exception e)
            {
                return BuildError(commandId, name, e.Message);
            }
        }

        private static string BuildError(string commandId, string name, string message)
        {
            var response = new JsonObject();
            response["type"] = "response";
            response["id"] = commandId;
            response["name"] = name;
            response["time"] = DateTime.Now.ToString("HH:mm:ss.fff");
            response["error"] = message;
            return response.ToString();
        }
    }
}



