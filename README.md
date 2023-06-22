# ONLY WebSocket + JSON protocol is supported!
## How to use
```cs
using QtKaneko.SignalR.Client;
using QtKaneko.SignalR.Client.Messages;

var url = new Uri("wss://example.com/Hub");
var client = new SignalRClient();

await foreach (var receive in client.ConnectAsync(url))
{
  // Handle received messages
  if (receive is HandshakeResponse)
  {
    var send = new StreamItem() {
      Item = JsonSerializer.SerializeToElement(
        new Data() {Value = 10} 
      ),
      InvocationId = "0"
    };

    // Send messages
    await client.SendAsync(send);
  }
  else if (receive is StreamItem streamItem)
  {
    var data = JsonSerializer.Deserialize<Data>(streamItem.Item);

    Console.WriteLine(data.Value);
  }
  // ...
}

struct Data
{
  public int Value { get; set; }
}
```