using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using ServerApp.API.Models;

namespace ServerApp.Client.Test;

public class WebSocketClient
{
    private readonly Uri _uri;
    private readonly ClientWebSocket _client;
    private  bool _isSetId { get; set; }
    public int PlayerId { get; private set; }

    public WebSocketClient(Uri uri)
    {
        _uri = uri;
        _client = new ClientWebSocket();
    }
    
    public async Task ConnectAsync(CancellationToken cancellationToken)
    {

        await _client.ConnectAsync(_uri, cancellationToken);
        Console.WriteLine("Connected to server.");
        await Task.Factory.StartNew(async () => await ReceiveLoopAsync(), TaskCreationOptions.LongRunning);
        var message = new PlayerPosition();
        await SendMessageAsync(message);
        Console.WriteLine($"Player ID Set: {PlayerId}");

    }


    public async Task SendMessageAsync<T>(T message)
    {
        var json = JsonConvert.SerializeObject(message);
        var bytes = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(bytes);
        await _client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
    }


    private async Task ReceiveLoopAsync()
    {
        Console.WriteLine("Run update listener async");
        var buffer = new ArraySegment<byte>(new byte[1024]);

        while (_client.State == WebSocketState.Open)
        {
            var result = await _client.ReceiveAsync(buffer, CancellationToken.None);

            if (!_isSetId)
            {
                var playerData = await DeserializeAsync<PlayerPosition>(buffer, result);
                if(playerData is null)
                    return;
                PlayerId = playerData.PlayerId;
                _isSetId = true;
            }
        }
    }


    public async Task CloseAsync(string closeStatusDescription)
    {
        try
        {
            await _client.CloseAsync(WebSocketCloseStatus.NormalClosure, closeStatusDescription, CancellationToken.None);
        }
        catch (OperationCanceledException)
        {
            //ignored
        }
    }


    private static async Task<T?> DeserializeAsync<T>(ArraySegment<byte> buffer, WebSocketReceiveResult result)
    {
        var json = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, result.Count);
        return await Task.Run(() => JsonConvert.DeserializeObject<T>(json));
    }
}