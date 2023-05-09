using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ServerApp.Websocket
{
    public abstract class WebSocketHandler
    {
        public abstract Task Handle(WebSocket webSocket);

        abstract protected Task<int> OnConnected(WebSocket webSocket);

        abstract protected Task OnDisconnected(WebSocket webSocket, int playerId);

        abstract protected Task OnMessage(string message, int playerId);

      
    }
}