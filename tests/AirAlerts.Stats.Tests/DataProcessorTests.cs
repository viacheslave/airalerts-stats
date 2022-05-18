using AirAlerts.Stats;
using Xunit;

namespace AirAlerts.Tests;

public class DataProcessorTests
{
  private readonly IReadOnlyList<Event> _events = new List<Event>
  {
    // May 1st, 00:01 off 
    // May 1st, 00:02 off
    new Event(new DateTime(2022, 05, 01, 00, 01, 00), EventType.Off, ""),
    new Event(new DateTime(2022, 05, 01, 00, 02, 00), EventType.Off, ""),

    // May 1st, 00:03 - 01:00 // 58
    new Event(new DateTime(2022, 05, 01, 00, 03, 00), EventType.On, ""),
    new Event(new DateTime(2022, 05, 01, 01, 00, 00), EventType.Off, ""),

    // May 1st, 12:00 - 13:00 // 61
    new Event(new DateTime(2022, 05, 01, 12, 00, 00), EventType.On, ""),
    new Event(new DateTime(2022, 05, 01, 13, 00, 00), EventType.Off, ""),

    // May 1st, 15:00 - 17:00 // 121
    new Event(new DateTime(2022, 05, 01, 15, 00, 00), EventType.On, ""),
    new Event(new DateTime(2022, 05, 01, 16, 00, 00), EventType.On, ""),
    new Event(new DateTime(2022, 05, 01, 17, 00, 00), EventType.Off, ""),

    // May 1st, 23:59 - 00:00 // 1
    // May 2nd, 00:00 - 00:01 // 2
    new Event(new DateTime(2022, 05, 01, 23, 59, 00), EventType.On, ""),
    new Event(new DateTime(2022, 05, 02, 00, 01, 00), EventType.Off, ""),

    // May 2nd, 23:59 - 00:00 // 1
    // May 3rd, whole day     // 1440
    // May 4th, 00:00 - 00:01 // 2 
    new Event(new DateTime(2022, 05, 02, 23, 59, 00), EventType.On, ""),
    new Event(new DateTime(2022, 05, 04, 00, 01, 00), EventType.Off, ""),

    // May 5th, nothing

    // May 6th, 04:00 - 04:55 // 56
    new Event(new DateTime(2022, 05, 06, 04, 00, 00), EventType.On, ""),
    new Event(new DateTime(2022, 05, 06, 04, 30, 00), EventType.Off, ""),
    new Event(new DateTime(2022, 05, 06, 04, 45, 00), EventType.Off, ""),
    new Event(new DateTime(2022, 05, 06, 04, 55, 00), EventType.Off, ""),

    // On - irrelevant
    new Event(new DateTime(2022, 05, 06, 05, 00, 00), EventType.On, ""),
  };

  [Fact]
  public void GetDaily_Tests()
  {
    var expected = new Dictionary<DateTime, int>
    {
      [new DateTime(2022, 05, 01)] = 58 + 61 + 121 + 1,
      [new DateTime(2022, 05, 02)] = 3,
      [new DateTime(2022, 05, 03)] = 1440,
      [new DateTime(2022, 05, 04)] = 2,
      [new DateTime(2022, 05, 05)] = 0,
      [new DateTime(2022, 05, 06)] = 56,
    };

    var compactedEvents = DataProcessor.Compact(_events);
    var periods = DataProcessor.GetPeriods(compactedEvents);

    var daily = DataProcessor.GetDailyStats(periods);

    Assert.Equal(expected.Count, daily.Count);

    foreach (var k in expected.Keys)
    {
      Assert.Equal(daily[k], expected[k]);
    }
  }

  [Fact]
  public void GetDailyLookback3_Tests()
  {
    var expected = new Dictionary<DateTime, int>
    {
      [new DateTime(2022, 05, 03)] = (1440 + 3 + 58 + 61 + 121 + 1) / 3,
      [new DateTime(2022, 05, 04)] = (2 + 1440 + 3) / 3,
      [new DateTime(2022, 05, 05)] = (0 + 2 + 1440) / 3,
      [new DateTime(2022, 05, 06)] = (56 + 0 + 2) / 3,
    };

    var compactedEvents = DataProcessor.Compact(_events);
    var periods = DataProcessor.GetPeriods(compactedEvents);

    var daily = DataProcessor.GetDailyStats(periods);
    var daily3 = DataProcessor.GetDailyLookback(daily, 3);

    Assert.Equal(expected.Count, daily3.Count);

    foreach (var k in expected.Keys)
    {
      Assert.Equal(daily3[k], expected[k]);
    }
  }
}
