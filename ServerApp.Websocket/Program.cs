using ServerApp.Websocket;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(180),
    ReceiveBufferSize = 4 * 1024
};

app.UseWebSockets(webSocketOptions);

app.Use(async (context, next) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var handler = new GameWebSocketHandler();
        await handler.Handle(context, webSocket);
    }
    else
    {
        await next();
    }
});

app.Run();