namespace QtKaneko.SignalR.Client.Messages;

public struct CancelInvocation : IMessage
{
  public IMessage.Types Type => IMessage.Types.CancelInvocation;
  public IDictionary<string, string>? Headers;
  public required string InvocationId;
}