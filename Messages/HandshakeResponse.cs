using System.Text.Json.Serialization;

namespace QtKaneko.SignalR.Client.Messages;

public struct HandshakeResponse : IMessage
{
  [JsonIgnore]
  public IMessage.Types Type => IMessage.Types.HandshakeResponse;
  public IDictionary<string, string>? Headers;
  public string? Error;
}