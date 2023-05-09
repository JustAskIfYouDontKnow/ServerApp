using ServerApp.API.Models;

namespace ServerApp.Client.Test;

public static class DataSender
{
    public static async Task Send(WebSocketClient client)
    {
        var rnd = new Random();
        var message = new PlayerPosition
            {
                PlayerId = client.PlayerId,
                PositionX = rnd.Next(1,10) * rnd.Next(2,3),
                PositionZ = rnd.Next(1,10) * rnd.Next(2,3),
                PositionY = rnd.Next(1,10) * rnd.Next(2,3)
            };
        await client.SendMessageAsync(message);

        Console.WriteLine($"Send ID {client.PlayerId} X {message.PositionX} Y {message.PositionY} Z {message.PositionZ}");

    }
}