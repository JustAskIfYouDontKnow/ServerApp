using System.Net.WebSockets;
using System.Text;

namespace ServerApp.Websocket.Services;

public class Sender
{
    private async Task SendSamePlayer(int playerId, string json, We)
    {
        var socket = Sockets.FirstOrDefault(x => Players[x.Key] == playerId).Value;

        if (socket != null && socket.State == WebSocketState.Open)
        {
            await SendMessage(socket, json);
        }
    }


    private async Task SendOther(int playerId, string json)
    {
        foreach (var socket in Sockets.Where(x => Players[x.Key] != playerId))
        {
            if (socket.Value.State == WebSocketState.Open)
            {
                await SendMessage(socket.Value, json);
            }
        }
    }


    private async Task SendAll(string json)
    {
        foreach (var socket in Sockets)
        {
            if (socket.Value.State == WebSocketState.Open)
            {
                await SendMessage(socket.Value, json);
            }
        }
    }


    private async Task SendMessage(WebSocket socket, string json)
    {
        await socket.SendAsync(
            new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, CancellationToken.None
        );
    }
}