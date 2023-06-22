namespace QtKaneko.SignalR.Client.Messages;

public struct Ack : IMessage
{
  public IMessage.Types Type => IMessage.Types.Ack;
  public IDictionary<string, string>? Headers;
  public required int SequenceId;
}