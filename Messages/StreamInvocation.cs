using System.Text.Json;

namespace QtKaneko.SignalR.Client.Messages;

public struct StreamInvocation : IMessage
{
  public IMessage.Types Type => IMessage.Types.StreamInvocation;
  public IDictionary<string, string>? Headers;
  public required string InvocationId;
  public required string Target;
  public required IEnumerable<JsonElement> Arguments;
  public IEnumerable<string>? StreamIds;
}