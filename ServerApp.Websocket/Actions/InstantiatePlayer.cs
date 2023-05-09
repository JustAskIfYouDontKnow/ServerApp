using Newtonsoft.Json;
using ServerApp.API.Models;
namespace ServerApp.Websocket.Actions;

public static class InstantiatePlayer
{
    
    public static async Task Action(HttpContext context, int playerId)
    {
        var playerData = new PlayerPosition
        {
            PlayerId = playerId
        };
        await SendSamePlayer(playerId, JsonConvert.SerializeObject(playerData));
            
        var otherPlayerData = new OtherPlayer
        {
            Ids = Players.Values.ToList()
        };
        await SendSamePlayer(playerId, JsonConvert.SerializeObject(otherPlayerData));

        var playerAction = new CreatePlayer
        {
            CreatedPlayerId = playerId
        };
        await SendOther(playerId, JsonConvert.SerializeObject(playerAction));

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