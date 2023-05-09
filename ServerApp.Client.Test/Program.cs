
namespace ServerApp.Client.Test
{
    static internal class Program
    {
        static internal async Task Main()
        {
            var clients = new List<WebSocketClient>();
            
            for (var i = 0; i < 2; i++)
            {
                var client = new WebSocketClient(new Uri("ws://localhost:5232"));
                await client.ConnectAsync(CancellationToken.None);
                clients.Add(client);
            }

            // Send data concurrently
            var tasks = new List<Task>();
            for (var i = 0; i < clients.Count; i++)
            {
                var client = clients[i];
                tasks.Add(Task.Run(async () =>
                {
                    for (var j = 0; j < 10; j++)
                    {
                        await DataSender.Send(client);
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                    }
                }));
            }
            await Task.WhenAll(tasks);

            // Close all clients
            foreach (var client in clients)
            {
                await client.CloseAsync("Closing");
            }
        }
    }
}