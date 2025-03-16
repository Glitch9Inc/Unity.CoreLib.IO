// using System;
// using System.Net.WebSockets;
// using System.Threading;
// using Cysharp.Threading.Tasks;
// using Glitch9.IO.Network.WebSocket;

// namespace Glitch9.IO.RESTApi
// {
//     public abstract partial class CRUDClient<TSelf>
//         where TSelf : CRUDClient<TSelf>
//     {
//         private IWebSocketClient _webSocketClient;

//         /// <summary>
//         /// Initialize the WebSocket connection.
//         /// Pass the WebSocket endpoint URL to connect.
//         /// </summary>
//         /// <param name="url">
//         /// The WebSocket endpoint URL.
//         /// </param>
//         public async UniTask CreateWebSocketConnectionAsync<TWebSocketEvent>(string url, Action<TWebSocketEvent> onEventReceived, bool autoManage = true)
//             where TWebSocketEvent : IWebSocketEvent
//         {
//             _webSocketClient = new WebSocketClient<TWebSocketEvent>(onEventReceived, JsonSettings, autoManage);
//             await _webSocketClient.CreateWebSocketConnectionAsync(url);
//         }

//         public async UniTask SendWebSocketEventAsync<TWebSocketEvent>(TWebSocketEvent webSocketEvent)
//             where TWebSocketEvent : IWebSocketEvent
//         {
//             await _webSocketClient.SendWebSocketEventAsync(webSocketEvent);
//         }

//         public async UniTask CloseWebSocket(string closeReason = "No reason provided.")
//         {
//             await _webSocketClient.CloseWebSocketConnectionAsync(closeReason);
//         }
//     }
// }