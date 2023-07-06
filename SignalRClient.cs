using System.Buffers;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

using QtKaneko.SignalR.Client.Extensions;
using QtKaneko.SignalR.Client.Messages;

namespace QtKaneko.SignalR.Client;

/// <summary> SignalR client. <para/>
/// <seealso href="https://github.com/dotnet/aspnetcore/blob/main/src/SignalR/docs/specs/HubProtocol.md#overview"/>
/// </summary>
/// <remarks> ONLY WebSocket + JSON protocol is supported! </remarks>
public struct SignalRClient
{
  const int _messageCapacity = 32768;
  static readonly JsonSerializerOptions _json = new() {
    IncludeFields = true,
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
  };
  readonly ClientWebSocket _webSocket = new();

  /// <summary> SignalR client. <para/>
  /// <seealso href="https://github.com/dotnet/aspnetcore/blob/main/src/SignalR/docs/specs/HubProtocol.md#overview"/>
  /// </summary>
  /// <remarks> ONLY WebSocket + JSON protocol is supported! </remarks>
  public SignalRClient() {}

  /// <summary> Connects to <paramref name="uri"/> starting to listen for messages. </summary>
  /// <param name="uri"> The URI of the WebSocket server to connect to. </param>
  public async IAsyncEnumerable<IMessage> ConnectAsync(Uri uri,
                                                       [EnumeratorCancellation]
                                                       CancellationToken ct = default)
  {
    await _webSocket.ConnectAsync(uri, ct).ConfigureAwait(false);

    await this.SendAsync(new HandshakeRequest() {Protocol = "json", Version = 1}, ct)
              .ConfigureAwait(false);

    while (_webSocket.State == WebSocketState.Open || _webSocket.State == WebSocketState.CloseSent
           && !ct.IsCancellationRequested)
    {
      var messageBuffer = ArrayPool<byte>.Shared.Rent(_messageCapacity);

      (var message, var messageBytes) = await _webSocket.ReceiveToEndAsync(messageBuffer, ct)
                                                        .ConfigureAwait(false);

      #if DEBUG
        var debugMessageJson = Encoding.UTF8.GetString(messageBytes);

        System.Diagnostics.Debug.WriteLine($"[Receive 🔽]\n{debugMessageJson}");
      #endif

      foreach (var recordBytes in messageBytes.Split((byte)'', ArraySegmentSplitOptions.RemoveEmptyEntries))
      {
        var record = JsonSerializer.Deserialize<IMessage>(recordBytes, _json)!;

        if (record is Ping) await SendAsync(new Ping()).ConfigureAwait(false);
        
        yield return record;
      }

      ArrayPool<byte>.Shared.Return(messageBuffer);
    }
  }

  /// <summary> Sends <paramref name="message"/> to connected WebSocket. </summary>
  /// <param name="message"> Message to send. </param>
  public async ValueTask SendAsync<TMessage>(TMessage message,
                                             CancellationToken ct = default)
  where TMessage : IMessage
  {
    var messageBuffer = ArrayPool<byte>.Shared.Rent(_messageCapacity);

    using var messageStream = new MemoryStream(messageBuffer, 0, messageBuffer.Length, true, true);
    JsonSerializer.Serialize(messageStream, message, _json);
    messageStream.Write(stackalloc byte[] {(byte)''});

    var messageBytes = new Memory<byte>(messageBuffer, 0, (int)messageStream.Position);

    #if DEBUG
      var debugMessageJson = Encoding.UTF8.GetString(messageBytes.Span);

      System.Diagnostics.Debug.WriteLine($"[Send 🔼]\n{debugMessageJson}");
    #endif

    await _webSocket.SendAsync(messageBytes, WebSocketMessageType.Text, true, ct)
                    .ConfigureAwait(false);

    ArrayPool<byte>.Shared.Return(messageBuffer);
  }
}