using System.Net.WebSockets;
using System.Text;
using ServerApp.Websocket.Services.Player;

namespace ServerApp.Websocket.Services.Sender;

public class Sender : ISender
{
    public async Task SendAll(string data)
    {
        foreach (var socket in PlayerService.Sockets)
        {
            if (socket.Value.State == WebSocketState.Open)
            {
                await SendMessage(socket.Value, data);
            }
        }
    }


    public async Task SendOther(int playerId, string data)
    {
        foreach (var socket in PlayerService.Sockets.Where(x => PlayerService.Players[x.Key] != playerId))
        {
            if (socket.Value.State == WebSocketState.Open)
            {
                await SendMessage(socket.Value, data);
            }
        }
    }


    public async Task SendMe(int playerId, string data)
    {
        var socket = PlayerService.Sockets.FirstOrDefault(x => PlayerService.Players[x.Key] == playerId).Value;

        if (socket != null && socket.State == WebSocketState.Open)
        {
            await SendMessage(socket, data);
        }
    }
    
    
    private async Task SendMessage(WebSocket socket, string json)
    {
        await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}