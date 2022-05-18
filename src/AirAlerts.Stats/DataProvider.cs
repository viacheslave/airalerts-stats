using TL;
using WTelegram;

namespace AirAlerts.Stats;

internal class DataProvider
{
  private readonly Client _client;
  private readonly InputPeerChannel _channel;

  // reliable data starts on March 15th, 2022
  private static readonly DateTime _startDate = new(2022, 03, 15);

  public DataProvider(Client client)
  {
    _client = client;

    // Air alerts channel
    // TODO: abstract
    _channel = new InputPeerChannel(1766138888, -4234079108947491235);
  }

  internal async IAsyncEnumerable<Message> Get()
  {
    const int limit = 100;
    var offset = 0;

    while (true)
    {
      Messages_MessagesBase data = await _client.Messages_GetHistory(_channel, limit: limit, add_offset: offset);
      foreach (var message in data.Messages)
      {
        if (message is not TL.Message)
        {
          continue;
        }

        var tlMessage = (TL.Message)message;
        yield return new Message(message.ID, message.Date, tlMessage.message);
      }

      if (data.Messages[^1].Date < _startDate)
        break;

      offset += limit;
    }
  }
}
