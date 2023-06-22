namespace QtKaneko.SignalR.Client.Messages;

public struct Sequence : IMessage
{
  public IMessage.Types Type => IMessage.Types.Sequence;
  public IDictionary<string, string>? Headers;
  public int? SequenceId;
}