using System.Text.Json;
using System.Text.Json.Serialization;

namespace QtKaneko.SignalR.Client.Messages;

public partial interface IMessage
{
  class Converter : JsonConverter<IMessage>
  {
    public override IMessage? Read(ref Utf8JsonReader reader,
                                   Type typeToConvert,
                                   JsonSerializerOptions options)
    {
      var start = reader;

      var message = (IMessage)JsonSerializer.Deserialize<IMessage.Empty>(ref reader, options);

      message = message.Type switch {
        Types.HandshakeRequest => JsonSerializer.Deserialize<HandshakeRequest>(ref start, options),
        Types.HandshakeResponse => JsonSerializer.Deserialize<HandshakeResponse>(ref start, options),
        Types.Invocation => JsonSerializer.Deserialize<Invocation>(ref start, options),
        Types.StreamInvocation => JsonSerializer.Deserialize<StreamInvocation>(ref start, options),
        Types.StreamItem => JsonSerializer.Deserialize<StreamItem>(ref start, options),
        Types.Completion => JsonSerializer.Deserialize<Completion>(ref start, options),
        Types.CancelInvocation => JsonSerializer.Deserialize<CancelInvocation>(ref start, options),
        Types.Ping => JsonSerializer.Deserialize<Ping>(ref start, options),
        Types.Close => JsonSerializer.Deserialize<Close>(ref start, options),
        Types.Ack => JsonSerializer.Deserialize<Ack>(ref start, options),
        Types.Sequence => JsonSerializer.Deserialize<Sequence>(ref start, options),
        _ => throw new ArgumentOutOfRangeException(nameof(message.Type))
      };

      return message;
    }

    public override void Write(Utf8JsonWriter writer,
                               IMessage value,
                               JsonSerializerOptions options)
    {
      throw new NotSupportedException();
    }
  }
}