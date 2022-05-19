using TL;
using WTelegram;

namespace AirAlerts.Stats;

internal interface ITelegramDataProvider
{
  Task<IEnumerable<Message>> Fetch(DateTime startDate);
}

internal class TelegramDataProvider : ITelegramDataProvider
{
  private readonly Client _client;
  private readonly InputPeerChannel _channel;

  public TelegramDataProvider(Client client)
  {
    _client = client;

    // Air alerts channel
    // TODO: abstract
    _channel = new InputPeerChannel(1766138888, -4234079108947491235);
  }

  public async Task<IEnumerable<Message>> Fetch(DateTime startDate)
  {
    var messages = new List<Message>();

    const int limit = 100;
    var offset = 0;

    while (true)
    {
      var data = await _client.Messages_GetHistory(_channel, limit: limit, add_offset: offset);
      foreach (var item in data.Messages)
      {
        if (item is not TL.Message)
        {
          continue;
        }

        var tlMessage = (TL.Message)item;
        var message = new Message(tlMessage.ID, tlMessage.Date, tlMessage.message);

        messages.Add(message);
      }

      if (data.Messages[^1].Date < startDate)
        break;

      offset += limit;
    }

    return messages;
  }
}
