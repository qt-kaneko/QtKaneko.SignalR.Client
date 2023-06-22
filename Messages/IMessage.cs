using System.Text.Json.Serialization;

namespace QtKaneko.SignalR.Client.Messages;

[JsonConverter(typeof(IMessage.Converter))]
public partial interface IMessage
{
  public enum Types
  {
    // Actually, HandshakeRequest has no type number and can't be received by client.
    HandshakeRequest = -1,
    // Actually, HandshakeResponse has no type number, so when it is received, default value will be 0.
    HandshakeResponse = 0,
    Invocation = 1,
    StreamInvocation = 4,
    StreamItem = 2,
    Completion = 3,
    CancelInvocation = 5,
    Ping = 6,
    Close = 7,
    Ack = 8,
    Sequence = 9
  }

  public Types Type { get; }
}