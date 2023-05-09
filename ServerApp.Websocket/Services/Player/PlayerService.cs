using System.Collections.Concurrent;
using System.Net.WebSockets;
using ServerApp.Websocket.Services.Sender;

namespace ServerApp.Websocket.Services.Player;

public class PlayerService : IPlayerService
{
    public static readonly ConcurrentDictionary<string, WebSocket?> Sockets = new();
    public static readonly ConcurrentDictionary<string, int> Players = new();
    
    private readonly ISender _sender;

    public PlayerService(ISender sender)
    {
        _sender = sender;
    }


    private string GetConnectionId()
    {
        return Guid.NewGuid().ToString();
    }


    public async Task<int> AddPlayerToServer(WebSocket webSocket)
    {
        var connectionId = GetConnectionId();
        var playerId = await GetNextAvailablePlayerId();

        Sockets.TryAdd(connectionId, webSocket);
        Players.TryAdd(connectionId, playerId);

        Console.WriteLine("ID " + connectionId + "Socket " + webSocket);;
        
        return playerId;
    }


    public async Task RemovePlayerFromServer(WebSocket webSocket, int playerId)
    {
        var connectionId = GetConnectionId(webSocket);
        Players.TryRemove(connectionId, out playerId);
        Sockets.TryRemove(connectionId, out _);

        await CloseConnection(webSocket, playerId);
    }
    
    private string GetConnectionId(WebSocket webSocket)
    {
        return Sockets.FirstOrDefault(x => x.Value == webSocket).Key;
    }
    
    
    private async Task CloseConnection(WebSocket webSocket, int playerId)
    {
        if (webSocket.State == WebSocketState.CloseReceived)
        {
            await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
            await _sender.SendAll($"Player {playerId} disconnected");
        }
        else if (webSocket.State == WebSocketState.Open)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
            await _sender.SendAll($"Player {playerId} disconnected");
        }
    }


    private Task<int> GetNextAvailablePlayerId()
    {
        var playerId = 1;
        if (Players.Values.Any())
        {
            playerId = Players.Values.Max() + 1;
        }
            
        Console.WriteLine($"Player with ID '{playerId}' has been added to the game.");
            
        return Task.FromResult(playerId);
    }
}