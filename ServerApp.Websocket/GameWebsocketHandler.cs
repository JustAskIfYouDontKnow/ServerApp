using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using ServerApp.API.Models;
using ServerApp.Websocket.Actions;
using ServerApp.Websocket.Services.Player;
using ServerApp.Websocket.Services.Sender;

namespace ServerApp.Websocket
{
    public class GameWebSocketHandler : WebSocketHandler
    {
        private readonly IPlayerService _playerService;
        private readonly ISender _sender;
        private readonly IPlayerActions _playerActions;
        
        private readonly byte[] _buffer = new  byte[1024 * 4];
        public GameWebSocketHandler(IPlayerService playerService, ISender sender, IPlayerActions playerActions)
        {
            _playerService = playerService;
            _sender = sender;
            _playerActions = playerActions;
        }
        public async override Task Handle(WebSocket webSocket)
        {
         
            
            var playerId = await OnConnected(webSocket);
            
            while (webSocket!.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(_buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(_buffer, 0, result.Count);
                    await OnMessage(message, playerId);
                } 
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await OnDisconnected(webSocket, playerId);
                }
            }
        }
        
        async protected override Task<int> OnConnected(WebSocket webSocket)
        { 
            var playerId = await _playerService.AddPlayerToServer(webSocket);
           await _playerActions.OnInstantiate(playerId);

           return playerId;
        }
        
        async protected override Task OnDisconnected(WebSocket webSocket, int playerId)
        {
            await _playerService.RemovePlayerFromServer(webSocket, playerId);
        }
        async protected override Task OnMessage(string message, int playerId)
        {
            //ToDo: Need Json Generic Serialize
            var data = JsonConvert.DeserializeObject<PlayerPosition>(message);
            var json = JsonConvert.SerializeObject(data);
            await _sender.SendOther(data.PlayerId, json);
        }
    }
}