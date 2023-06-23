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
  static readonly JsonSerializerOptions _options = new() {
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
                                                       CancellationToken cancellationToken = default)
  {
    await _webSocket.ConnectAsync(uri, cancellationToken);

    await SendAsync(
      new HandshakeRequest() {
        Protocol = "json",
        Version = 1
      },
      cancellationToken
    );

    while (!cancellationToken.IsCancellationRequested)
    {
      var messageJson = await _webSocket.ReceiveStringAsync(cancellationToken);

      #if DEBUG
        Console.WriteLine(($"[Receive] {messageJson}"));
      #endif

      foreach (var recordJson in messageJson.Split("", StringSplitOptions.RemoveEmptyEntries))
      {
        var record = JsonSerializer.Deserialize<IMessage>(recordJson, _options)!;

        yield return record;

        if (record is Ping) await SendAsync(new Ping());
        else if (record is Completion) yield break;
      }
    }
  }

  /// <summary> Sends <paramref name="message"/> to connected WebSocket. </summary>
  /// <param name="message"> Message to send. </param>
  public async ValueTask SendAsync<TMessage>(TMessage message,
                                             CancellationToken cancellationToken = default)
  where TMessage : IMessage
  {
    var messageJson = JsonSerializer.Serialize(message, _options);

    #if DEBUG
      Console.WriteLine(($"[Send] {messageJson}"));
    #endif

    await _webSocket.SendAsync(
      Encoding.UTF8.GetBytes(messageJson + ""),
      WebSocketMessageType.Text,
      true,
      cancellationToken
    );
  }
}