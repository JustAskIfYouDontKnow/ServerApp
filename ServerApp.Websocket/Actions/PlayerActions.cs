using Newtonsoft.Json;
using ServerApp.API.Models;
using ServerApp.Websocket.Services.Player;
using ServerApp.Websocket.Services.Sender;
namespace ServerApp.Websocket.Actions;

public class PlayerActions : IPlayerActions
{
    private readonly ISender _sender;

    public PlayerActions(ISender sender)
    {
        _sender = sender;
    }


    public async Task OnInstantiate(int playerId)
    {
        var playerData = new PlayerPosition
        {
            PlayerId = playerId
        };
        await _sender.SendMe(playerId, JsonConvert.SerializeObject(playerData));
            
        var otherPlayerData = new OtherPlayer
        {
            Ids = PlayerService.Players.Values.ToList()
        };
        await _sender.SendMe(playerId, JsonConvert.SerializeObject(otherPlayerData));

        var playerAction = new CreatePlayer
        {
            CreatedPlayerId = playerId
        };
        await _sender.SendOther(playerId, JsonConvert.SerializeObject(playerAction));

    }
    class OtherPlayer
    { 
        public List<int>? Ids { get; set; }
    }
    class CreatePlayer
    { 
        public int CreatedPlayerId { get; set; }
    }
}