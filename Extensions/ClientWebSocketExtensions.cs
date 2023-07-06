using System.Net.WebSockets;

namespace QtKaneko.SignalR.Client.Extensions;

static class ClientWebSocketExtensions
{
  public static async ValueTask<(ValueWebSocketReceiveResult, ArraySegment<byte>)> ReceiveToEndAsync(
    this ClientWebSocket @this,
    byte[] buffer, CancellationToken ct = default)
  {
    var position = 0;

    var message = default(ValueWebSocketReceiveResult);
    do
    {
      var segment = new Memory<byte>(buffer, position, buffer.Length - position);

      message = await @this.ReceiveAsync(segment, ct).ConfigureAwait(false);
      position += message.Count;
    }
    while (!message.EndOfMessage && !ct.IsCancellationRequested);

    return (message, new ArraySegment<byte>(buffer, 0, position));
  }
}