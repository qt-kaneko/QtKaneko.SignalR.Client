using System.Buffers;
using System.Net.WebSockets;
using System.Runtime.InteropServices;

namespace QtKaneko.SignalR.Client.Extensions;

static class ClientWebSocketExtensions
{
  public static async ValueTask<string> ReceiveStringAsync(this ClientWebSocket clientWebSocket, CancellationToken cancellationToken = default)
  {
    using var resultStream = new MemoryStream();

    using var bufferMemory = MemoryPool<byte>.Shared.Rent(8192);
    MemoryMarshal.TryGetArray<byte>(bufferMemory.Memory, out var buffer);

    var package = default(ValueWebSocketReceiveResult);
    do
    {
      package = await clientWebSocket.ReceiveAsync(bufferMemory.Memory, cancellationToken);

      await resultStream.WriteAsync(buffer.Array!, buffer.Offset, package.Count);
    } while (!package.EndOfMessage);
    resultStream.Position = 0;

    using var resultReader = new StreamReader(resultStream);
    var result = await resultReader.ReadToEndAsync();
    return result;
  }
}