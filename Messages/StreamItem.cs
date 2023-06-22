using System.Text.Json;

namespace QtKaneko.SignalR.Client.Messages;

public struct StreamItem : IMessage
{
  public IMessage.Types Type => IMessage.Types.StreamItem;
  public IDictionary<string, string>? Headers;
  public required string InvocationId;
  public required JsonElement Item;
}