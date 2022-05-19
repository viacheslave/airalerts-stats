namespace AirAlerts.Stats;

internal class DataProvider
{
  // reliable data starts on March 15th, 2022
  private static readonly DateTime _startDate = new(2022, 03, 15);

  private readonly ITelegramDataProvider _telegramDataProvider;
  private readonly ILocalDataProvider _localDataProvider;

  public DataProvider(ITelegramDataProvider telegramDataProvider,
    ILocalDataProvider localDataProvider)
  {
    _telegramDataProvider = telegramDataProvider;
    _localDataProvider = localDataProvider;
  }

  internal async Task<IEnumerable<Message>> Get()
  {
    var messages = _localDataProvider.GetLocal();

    var startDate = messages.Any()
      ? messages.Max(x => x.Date.Date).AddDays(-1)
      : _startDate;

    var fetched = await _telegramDataProvider.Fetch(startDate);

    messages = messages.Concat(fetched)
      .DistinctBy(m => m.Id)
      .OrderByDescending(m => m.Id)
      .ToList();

    _localDataProvider.SaveLocal(messages);

    return messages;
  }
}
