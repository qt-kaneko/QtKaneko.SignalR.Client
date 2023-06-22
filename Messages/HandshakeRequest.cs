using System.Text.Json.Serialization;

namespace QtKaneko.SignalR.Client.Messages;

public struct HandshakeRequest : IMessage
{
  [JsonIgnore]
  public IMessage.Types Type => IMessage.Types.HandshakeRequest;
  public IDictionary<string, string>? Headers;
  public required string Protocol;
  public required int Version;
}