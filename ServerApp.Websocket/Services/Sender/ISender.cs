namespace ServerApp.Websocket.Services.Sender;

public interface ISender
{
    public Task SendAll(string data);
    public Task SendOther(int playerId, string data);
    public Task SendMe(int playerId, string data);
}