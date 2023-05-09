using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using ServerApp.API.Models;
using ServerApp.Websocket.Actions;

namespace ServerApp.Websocket
{
    public class GameWebSocketHandler : WebSocketHandler
    {
        public async override Task Handle(HttpContext context, WebSocket webSocket)
        {
            var connectionId = GetConnectionId();
            
            Sockets.TryAdd(connectionId, webSocket);
            Console.WriteLine("ID " + connectionId + "Socket " + webSocket);;

            var playerId = await GetNextAvailablePlayerId();
            
            Players.TryAdd(connectionId, playerId);

            await OnConnected(context, playerId);

            var buffer = new byte[1024 * 4];

            while (webSocket!.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                switch (result.MessageType)
                {
                    case WebSocketMessageType.Text:
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await OnMessage(context, message, playerId);
                        break;
                    }

                    case WebSocketMessageType.Close:
                        Players.TryRemove(connectionId, out playerId); 
                        Sockets.TryRemove(connectionId, out webSocket!);
                        await CloseConnection(webSocket, playerId);
                        break;
                }
            }
        }
        
        private async Task CloseConnection(WebSocket webSocket, int playerId)
        {
            if (webSocket.State == WebSocketState.CloseReceived)
            {
                await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                await OnDisconnected(playerId);
            }
            else if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection", CancellationToken.None);
                await OnDisconnected(playerId);
            }
        }

        async protected override Task OnConnected(HttpContext context, int playerId)
        {
            await InstantiatePlayer.Action(context, playerId);
        }
        
        async protected override Task OnDisconnected(int playerId)
        {
            Console.WriteLine("  PlayerID " + playerId + " Disconnected");
            Console.WriteLine("Connected Players " + Players.Count);
            Console.WriteLine("Connected Sockets " + Sockets.Count);
            await SendAll($"Player {playerId} disconnected");
        }
        
        async protected override Task OnMessage(HttpContext context, string message, int playerId)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<PlayerPosition>(message);
                // Console.WriteLine($"ID {data.PlayerId} X {data.PositionX} Y {data.PositionY} Z {data.PositionZ}");
                var json = JsonConvert.SerializeObject(data);
                await SendOther(data.PlayerId, json);
            }
            catch (Exception e)
            {
                //ignored
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
}