
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Net.WebSockets;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Diagnostics;

namespace GameCore.DebugCommand
{
    // dbgServer.py (既定: ws://localhost:8765) に接続し、JS(DebugCommandConsole)から
    // 送られてくる DebugCommand を受信して実行し、結果を送り返すハンドラ。
    // 空のGameObjectに1つアタッチしてください（自動生成・編集不要）。
    public class DebugCommandWebSocketHandler : MonoBehaviour
    {
        [SerializeField] private string url = "ws://localhost:8765";

        private ClientWebSocket _socket;
        private CancellationTokenSource _cts;
        
        [Conditional("UNITY_EDITOR")]
        [Conditional("UNITY_ENABLE_CHECKS")]
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (UnityEngine.Debug.isDebugBuild || Application.isEditor)
            {
                var go = new GameObject("DebugCommand");
                UnityEngine.Object.DontDestroyOnLoad(go);
                go.AddComponent<DebugCommandWebSocketHandler>();
                //DebugCommandInstaller.InstallAll();
            }
        }
        [Conditional("UNITY_EDITOR")]
        [Conditional("UNITY_ENABLE_CHECKS")]
        private void Start()
        {
            _cts = new CancellationTokenSource();
            ConnectAsync().Forget();
        }


        private async UniTaskVoid ConnectAsync()
        {
            _socket = new ClientWebSocket();
            try
            {
                await _socket.ConnectAsync(new Uri(url), _cts.Token);
                UnityEngine.Debug.Log($"[DebugCommand] WebSocket connected: {url}");
                ReceiveLoop().Forget();
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[DebugCommand] WebSocket connect failed: {e.Message}");
            }
        }
        private async UniTaskVoid ReceiveLoop()
        {
            var buffer = new byte[8192];
            while (_socket != null && _socket.State == WebSocketState.Open && !_cts.IsCancellationRequested)
            {
                string text;
                using (var ms = new MemoryStream())
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _socket.ReceiveAsync(new ArraySegment<byte>(buffer), _cts.Token);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "closed", _cts.Token);
                            return;
                        }
                        ms.Write(buffer, 0, result.Count);
                    } while (!result.EndOfMessage);

                    text = Encoding.UTF8.GetString(ms.ToArray());
                }

                HandleMessage(text).Forget();
            }
        }
        private async UniTaskVoid HandleMessage(string text)
        {
            string response;
            try
            {
                // type:"command" 以外のメッセージ（通常のDebugLog等）の場合は null が返る
                response = DebugCommandRegistry.Dispatch(text);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[DebugCommand] Dispatch error: {e.Message}");
                return;
            }

            if (string.IsNullOrEmpty(response)) return;

            await SendAsync(response);
        }
        private async UniTask SendAsync(string text)
        {
            if (_socket == null || _socket.State != WebSocketState.Open) return;
            var bytes = Encoding.UTF8.GetBytes(text);
            await _socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, _cts.Token);
        }

        [Conditional("UNITY_EDITOR")]
        [Conditional("UNITY_ENABLE_CHECKS")]
        private async void OnDestroy()
        {
            _cts?.Cancel();
            try
            {
                if (_socket != null && _socket.State == WebSocketState.Open)
                {
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "destroyed", CancellationToken.None);
                }
            }
            catch
            {
                // シャットダウン時の例外は無視する
            }
            finally
            {
                _socket?.Dispose();
            }
        }
    }
}

