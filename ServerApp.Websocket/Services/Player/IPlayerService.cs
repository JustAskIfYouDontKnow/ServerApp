using System.Net.WebSockets;

namespace ServerApp.Websocket.Services.Player;

public interface IPlayerService
{
    public Task<int> AddPlayerToServer(WebSocket webSocket);

    public Task RemovePlayerFromServer(WebSocket webSocket, int playerId);
}