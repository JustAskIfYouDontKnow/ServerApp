using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace ServerApp.Websocket
{
    public abstract class WebSocketHandler
    {
        static protected readonly ConcurrentDictionary<string, WebSocket?> Sockets = new();
        static protected readonly ConcurrentDictionary<string, int> Players = new();

        public abstract Task Handle(HttpContext context, WebSocket webSocket);

        abstract protected Task OnConnected(HttpContext context, int playerId);

        abstract protected Task OnDisconnected(int playerId);

        abstract protected Task OnMessage(HttpContext context, string message, int playerId);

        protected string GetConnectionId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}