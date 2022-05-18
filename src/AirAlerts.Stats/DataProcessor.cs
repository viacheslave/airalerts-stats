using System.Text.RegularExpressions;

namespace AirAlerts.Stats;

internal static class DataProcessor
{
  internal static async Task<IReadOnlyList<Event>> GetEvents(IAsyncEnumerable<Message> messages)
  {
    var regex = new Regex("(?<time>\\d{2}:\\d{2})\\s(?<content>.*)\\s(?<tag>#(Харківська_область|Харківський_район))", RegexOptions.Multiline);

    var events = new List<Event>();

    await foreach (var message in messages)
    {
      var text = message.Text.Replace('\n', ' ');

      var matchCollection = regex.Matches(text);
      if (matchCollection.Count > 0)
      {
        var contentStr = matchCollection[0].Groups["content"].Value;
        var tagStr = matchCollection[0].Groups["tag"].Value;

        var isOn = contentStr.Contains("Повітряна");
        var isOff = contentStr.Contains("Відбій");

        if (isOn || isOff)
        {
          events.Add(
            new Event(message.Date, isOn ? EventType.On : EventType.Off, tagStr));
        }
      }
    }

    return events;
  }

  internal static IReadOnlyList<Event> Compact(IReadOnlyList<Event> events)
  {
    events = events
      .OrderBy(e => e.Date)
      .SkipWhile(e => e.Type == EventType.Off)
      .ToList();

    var compactedEvents = new List<Event>();

    for (var i = 0; i < events.Count; i++)
    {
      if (i == 0)
      {
        compactedEvents.Add(events[0]);
        continue;
      }

      if (events[i].Type == EventType.On && compactedEvents[^1].Type == EventType.On)
      {
        continue;
      }

      if (events[i].Type == EventType.Off && compactedEvents[^1].Type == EventType.Off)
      {
        compactedEvents[^1] = events[i];
        continue;
      }

      compactedEvents.Add(events[i]);
    }

    return compactedEvents;
  }

  internal static IReadOnlyList<Period> GetPeriods(IReadOnlyList<Event> events)
  {
    var periods = new List<Period>();

    for (var i = 1; i < events.Count; i += 2)
    {
      periods.Add(
        new Period(events[i - 1].Date, events[i].Date));
    }

    return periods;
  }

  internal static IDictionary<DateTime, int> GetDailyStats(IEnumerable<Period> periods)
  {
    var daily = new Dictionary<DateTime, int>();

    var minDate = periods.Min(e => e.Start.Date);
    var maxDate = periods.Max(e => e.End.Date);

    for (var date = minDate; date <= maxDate; date = date.AddDays(1))
    {
      daily.Add(date, 0);
    }

    foreach (var period in periods)
    {
      if (period.Start.Date == period.End.Date)
      {
        daily[period.Start.Date] += (int)((period.End - period.Start).TotalMinutes) + 1;
        continue;
      }

      var from = period.Start.Date.AddDays(1);
      var to = period.End.Date;

      if (from != to)
      {
        for (var date = from; date < to; date = date.AddDays(1))
        {
          daily[date] += 60 * 24;
        }
      }

      daily[period.Start.Date] += (int)(from - period.Start).TotalMinutes;
      daily[period.End.Date] += (int)(period.End - to).TotalMinutes + 1;
    }

    return daily;
  }

  internal static IDictionary<DateTime, int> GetDailyLookback(IDictionary<DateTime, int> daily, int lookback)
  {
    var runningSum = 0;
    var runningDate = daily.Keys.Max();

    for (var i = 0; i < lookback; i++)
    {
      var date = runningDate.AddDays(-i);

      if (daily.ContainsKey(date))
      {
        runningSum += daily[date];
      }
    }

    var data = new Dictionary<DateTime, int>();
    data[runningDate] = runningSum / lookback;

    while (daily.ContainsKey(runningDate.AddDays(-lookback)))
    {
      runningSum += daily[runningDate.AddDays(-lookback)];
      runningSum -= daily[runningDate];

      runningDate = runningDate.AddDays(-1);

      data[runningDate] = runningSum / lookback;
    }

    return data;
  }
}
