namespace QtKaneko.SignalR.Client.Messages;

public partial interface IMessage
{
  struct Empty : IMessage
  {
    public IMessage.Types Type { get; init; }
  }
}