using System.Text.Json;

namespace QtKaneko.SignalR.Client.Messages;

public struct Invocation : IMessage
{
  public IMessage.Types Type => IMessage.Types.Invocation;
  public IDictionary<string, string>? Headers;
  public string? InvocationId;
  public required string Target;
  public required IEnumerable<JsonElement> Arguments;
  public IEnumerable<string>? StreamIds;
}