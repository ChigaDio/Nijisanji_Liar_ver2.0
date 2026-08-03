

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace GameCore.DebugCommand
{
    // 外部ライブラリに依存しない最小限のJSONパーサ・シリアライザ（DebugCommand専用）
    public static class MiniJson
    {
        public static object Parse(string json)
        {
            int i = 0;
            return ParseValue(json, ref i);
        }

        public static string Serialize(object obj)
        {
            var sb = new StringBuilder();
            WriteValue(sb, obj);
            return sb.ToString();
        }

        private static object ParseValue(string s, ref int i)
        {
            SkipWhitespace(s, ref i);
            if (i >= s.Length) return null;
            char c = s[i];
            if (c == '{') return ParseObject(s, ref i);
            if (c == '[') return ParseArray(s, ref i);
            if (c == '"') return ParseString(s, ref i);
            if (c == 't' || c == 'f') return ParseBool(s, ref i);
            if (c == 'n') { i += 4; return null; }
            return ParseNumber(s, ref i);
        }

        private static Dictionary<string, object> ParseObject(string s, ref int i)
        {
            var dict = new Dictionary<string, object>();
            i++; // {
            SkipWhitespace(s, ref i);
            if (i < s.Length && s[i] == '}') { i++; return dict; }
            while (true)
            {
                SkipWhitespace(s, ref i);
                string key = ParseString(s, ref i);
                SkipWhitespace(s, ref i);
                i++; // :
                var value = ParseValue(s, ref i);
                dict[key] = value;
                SkipWhitespace(s, ref i);
                if (i < s.Length && s[i] == ',') { i++; continue; }
                if (i < s.Length && s[i] == '}') { i++; break; }
                break;
            }
            return dict;
        }

        private static List<object> ParseArray(string s, ref int i)
        {
            var list = new List<object>();
            i++; // [
            SkipWhitespace(s, ref i);
            if (i < s.Length && s[i] == ']') { i++; return list; }
            while (true)
            {
                list.Add(ParseValue(s, ref i));
                SkipWhitespace(s, ref i);
                if (i < s.Length && s[i] == ',') { i++; continue; }
                if (i < s.Length && s[i] == ']') { i++; break; }
                break;
            }
            return list;
        }

        private static string ParseString(string s, ref int i)
        {
            var sb = new StringBuilder();
            i++; // opening "
            while (i < s.Length && s[i] != '"')
            {
                char c = s[i];
                if (c == '\\' && i + 1 < s.Length)
                {
                    i++;
                    char e = s[i];
                    switch (e)
                    {
                        case 'n': sb.Append('\n'); break;
                        case 't': sb.Append('\t'); break;
                        case 'r': sb.Append('\r'); break;
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'u':
                            string hex = s.Substring(i + 1, 4);
                            sb.Append((char)Convert.ToInt32(hex, 16));
                            i += 4;
                            break;
                        default: sb.Append(e); break;
                    }
                }
                else
                {
                    sb.Append(c);
                }
                i++;
            }
            i++; // closing "
            return sb.ToString();
        }

        private static bool ParseBool(string s, ref int i)
        {
            if (i + 4 <= s.Length && s.Substring(i, 4) == "true") { i += 4; return true; }
            i += 5; // false
            return false;
        }

        private static double ParseNumber(string s, ref int i)
        {
            int start = i;
            while (i < s.Length && (char.IsDigit(s[i]) || s[i] == '-' || s[i] == '+' || s[i] == '.' || s[i] == 'e' || s[i] == 'E'))
            {
                i++;
            }
            return double.Parse(s.Substring(start, i - start), CultureInfo.InvariantCulture);
        }

        private static void SkipWhitespace(string s, ref int i)
        {
            while (i < s.Length && char.IsWhiteSpace(s[i])) i++;
        }

        private static void WriteValue(StringBuilder sb, object obj)
        {
            switch (obj)
            {
                case null:
                    sb.Append("null");
                    break;
                case string str:
                    WriteString(sb, str);
                    break;
                case bool b:
                    sb.Append(b ? "true" : "false");
                    break;
                case JsonObject jo:
                    WriteObject(sb, jo.Raw);
                    break;
                case Dictionary<string, object> dict:
                    WriteObject(sb, dict);
                    break;
                case List<object> list:
                    WriteArray(sb, list);
                    break;
                case float f:
                    sb.Append(f.ToString(CultureInfo.InvariantCulture));
                    break;
                case double d:
                    sb.Append(d.ToString(CultureInfo.InvariantCulture));
                    break;
                case int ii:
                    sb.Append(ii.ToString(CultureInfo.InvariantCulture));
                    break;
                case uint ui:
                    sb.Append(ui.ToString(CultureInfo.InvariantCulture));
                    break;

                                    // Dictionary<string,object> に限らず、IDictionary を実装している型なら
                // すべてオブジェクトとしてシリアライズする（Dictionary<string,object> はこちらでカバーされる）
                case System.Collections.IDictionary dict:
                    WriteObject(sb, dict);
                    break;
                // List<object> に限らず、List<JsonObject> / 配列 / その他のコレクションもすべて
                // 配列としてシリアライズする。ジェネリクスは不変(invariant)なため、
                // 従来の "case List<object> list" では List<JsonObject> 等にマッチせず、
                // default節に落ちて ToString() 表記（"System.Collections.Generic.List`1[...]"）に
                // なってしまっていた。IEnumerable で受けることでこれを解消する。
                case System.Collections.IEnumerable list:
                    WriteArray(sb, list);
                    break;
                default:
                    WriteString(sb, obj.ToString());
                    break;
            }
        }

        private static void WriteObject(StringBuilder sb, System.Collections.IDictionary dict)
        {
            sb.Append('{');
            bool first = true;
            foreach (System.Collections.DictionaryEntry entry in dict)
            {
                if (!first) sb.Append(',');
                first = false;
                WriteString(sb, entry.Key.ToString());
                sb.Append(':');
                WriteValue(sb, entry.Value);
            }
            sb.Append('}');
        }
        private static void WriteArray(StringBuilder sb, System.Collections.IEnumerable list)
        {
            sb.Append('[');
            bool first = true;
            foreach (var item in list)
            {
                if (!first) sb.Append(',');
                first = false;
                WriteValue(sb, item);
            }
            sb.Append(']');
        }
        private static void WriteObject(StringBuilder sb, Dictionary<string, object> dict)
        {
            sb.Append('{');
            bool first = true;
            foreach (var kv in dict)
            {
                if (!first) sb.Append(',');
                first = false;
                WriteString(sb, kv.Key);
                sb.Append(':');
                WriteValue(sb, kv.Value);
            }
            sb.Append('}');
        }
        private static void WriteArray(StringBuilder sb, List<object> list)
        {
            sb.Append('[');
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0) sb.Append(',');
                WriteValue(sb, list[i]);
            }
            sb.Append(']');
        }
        private static void WriteString(StringBuilder sb, string s)
        {
            sb.Append('"');
            foreach (char c in s)
            {
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default: sb.Append(c); break;
                }
            }
            sb.Append('"');
        }
    }

    // Newtonsoft の JObject の代わりに使う軽量ラッパー（外部ライブラリ非依存）
    public class JsonObject
    {
        private readonly Dictionary<string, object> _data;

        public JsonObject() { _data = new Dictionary<string, object>(); }
        public JsonObject(Dictionary<string, object> data) { _data = data ?? new Dictionary<string, object>(); }

        internal Dictionary<string, object> Raw => _data;

        public static JsonObject Parse(string json)
        {
            var obj = MiniJson.Parse(json) as Dictionary<string, object>;
            return new JsonObject(obj ?? new Dictionary<string, object>());
        }

        public object this[string key]
        {
            get => _data.TryGetValue(key, out var v) ? v : null;
            set => _data[key] = (value is JsonObject jo) ? (object)jo.Raw : value;
        }

        public bool Has(string key) => _data.ContainsKey(key);

        public JsonObject GetObject(string key)
        {
            if (_data.TryGetValue(key, out var v) && v is Dictionary<string, object> d) return new JsonObject(d);
            return null;
        }

        public string GetString(string key, string def = "")
        {
            return _data.TryGetValue(key, out var v) && v != null ? v.ToString() : def;
        }

        public bool GetBool(string key, bool def = false)
        {
            return (_data.TryGetValue(key, out var v) && v is bool b) ? b : def;
        }

        public int GetInt(string key, int def = 0)
        {
            return (_data.TryGetValue(key, out var v) && v is double d) ? (int)d : def;
        }

        public uint GetUInt(string key, uint def = 0)
        {
            return (_data.TryGetValue(key, out var v) && v is double d) ? (uint)d : def;
        }

        public float GetFloat(string key, float def = 0f)
        {
            return (_data.TryGetValue(key, out var v) && v is double d) ? (float)d : def;
        }

        public double GetDouble(string key, double def = 0)
        {
            return (_data.TryGetValue(key, out var v) && v is double d) ? d : def;
        }

        public Vector2 GetVector2(string key)
        {
            var o = GetObject(key);
            return o == null ? Vector2.zero : new Vector2(o.GetFloat("x"), o.GetFloat("y"));
        }

        public Vector3 GetVector3(string key)
        {
            var o = GetObject(key);
            return o == null ? Vector3.zero : new Vector3(o.GetFloat("x"), o.GetFloat("y"), o.GetFloat("z"));
        }

        public void SetVector2(string key, Vector2 v)
        {
            _data[key] = new Dictionary<string, object> { ["x"] = (double)v.x, ["y"] = (double)v.y };
        }

        public void SetVector3(string key, Vector3 v)
        {
            _data[key] = new Dictionary<string, object> { ["x"] = (double)v.x, ["y"] = (double)v.y, ["z"] = (double)v.z };
        }

        public override string ToString()
        {
            return MiniJson.Serialize(_data);
        }
    }
}



