using System;
using System.Net.WebSockets;
using UnityEngine;
using WebSocketSharp;
using WebSocket = WebSocketSharp.WebSocket;

public class DebugLogBridgeRuntime : MonoBehaviour
{
    private WebSocket ws;
    private const string WebSocketUrl = "ws://localhost:8765"; // Python WebSocketサーバーのURL
    private float reconnectTimer;
    private bool isConnecting;

    void Awake()
    {
        TryConnect();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        // 再接続監視
        reconnectTimer += Time.deltaTime;
        if (reconnectTimer > 5f)
        {
            reconnectTimer = 0f;
            if (ws == null || ws.ReadyState != WebSocketSharp.WebSocketState.Open)
            {
                TryConnect();
            }
        }
    }

    private void TryConnect()
    {
        if (isConnecting) return; // 接続試行中の重複防止
        isConnecting = true;

        try
        {
            // 既存の接続を閉じる
            ws?.Close();

            // 新しいWebSocketを作成
            ws = new WebSocket(WebSocketUrl);

            // イベントハンドラを設定（メインスレッドで実行）
            ws.OnOpen += (sender, e) =>
            {
                UnityEngine.Debug.Log("WebSocket connected successfully!");
            };

            ws.OnError += (sender, e) =>
            {
                UnityEngine.Debug.LogWarning($"WebSocket error: {e.Message}");
                ws = null; // 再接続をトリガー
            };

            ws.OnClose += (sender, e) =>
            {
                UnityEngine.Debug.Log($"WebSocket disconnected. Reason: {e.Reason}");
                ws = null; // 再接続をトリガー
            };

            // オプション: サーバーからのメッセージ受信（必要に応じて有効化）
            // ws.OnMessage += (sender, e) =>
            // {
            //     UnityEngine.Debug.Log($"Received message: {e.Data}");
            // };

            // 非同期接続を試行
            ws.ConnectAsync();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"DebugBridge WebSocket connection failed: {e.Message}");
            ws = null;
        }
        finally
        {
            isConnecting = false;
        }
    }

    public void SendLog(string message, string type)
    {
        if (ws == null || ws.ReadyState != WebSocketSharp.WebSocketState.Open)
        {
            UnityEngine.Debug.LogWarning("WebSocket not connected. Skipping send.");
            return;
        }

        var json = JsonUtility.ToJson(new LogData
        {
            message = message,
            type = type,
            time = DateTime.Now.ToString("HH:mm:ss")
        });

        try
        {
            ws.Send(json);
            UnityEngine.Debug.Log($"Sent log: [{type}] {message}");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogWarning($"Failed to send log: {e.Message}");
            ws = null; // 再接続をトリガー
        }
    }

    [Serializable]
    private class LogData
    {
        public string message;
        public string type;
        public string time;
    }

    void OnDestroy()
    {
        if (ws != null)
        {
            ws.Close();
            ws = null;
        }
    }
}
