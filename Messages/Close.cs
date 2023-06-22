namespace QtKaneko.SignalR.Client.Messages;

public struct Close : IMessage
{
  public IMessage.Types Type => IMessage.Types.Close;
  public IDictionary<string, string>? Headers;
  public string? Error;
  public bool? AllowReconnect;
}