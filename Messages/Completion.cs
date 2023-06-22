using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace QtKaneko.SignalR.Client.Messages;

public struct Completion : IMessage
{
  public IMessage.Types Type => IMessage.Types.Completion;
  public IDictionary<string, string>? Headers;
  public required string InvocationId;
  public JsonElement? Result;
  public string? Error;

  [MemberNotNullWhen(false, nameof(Result))]
  internal bool IsError => Error != null;
}