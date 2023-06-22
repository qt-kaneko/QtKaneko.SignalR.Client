namespace QtKaneko.SignalR.Client.Messages;

public struct Ping : IMessage
{
  public IMessage.Types Type => IMessage.Types.Ping;
}