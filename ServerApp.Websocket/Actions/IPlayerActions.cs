namespace ServerApp.Websocket.Actions;

public interface IPlayerActions
{
      Task OnInstantiate(int playerId);
}