using AirAlerts.Stats;
using NSubstitute;
using Xunit;

namespace AirAlerts.Tests;

public class DataProviderTests
{
  [Fact]
  public async Task DataProvider_Get()
  {
    // A
    var telegramDataProvider = Substitute.For<ITelegramDataProvider>();
    var localDataProvider = Substitute.For<ILocalDataProvider>();

    var sut = new DataProvider(telegramDataProvider, localDataProvider);

    var telegramMessages = new List<Message>
    {
      new Message(4, new DateTime(2022, 05, 01), ""),
      new Message(5, new DateTime(2022, 05, 02), ""),
      new Message(6, new DateTime(2022, 05, 03), ""),
    };

    telegramDataProvider
      .Fetch(Arg.Any<DateTime>())
      .ReturnsForAnyArgs(
        Task.FromResult((IEnumerable<Message>)telegramMessages));

    var localMessages = new List<Message>
    {
      new Message(3, new DateTime(2022, 04, 30), ""),
      new Message(4, new DateTime(2022, 05, 01), ""),
      new Message(5, new DateTime(2022, 05, 02), ""),
    };

    localDataProvider.GetLocal().ReturnsForAnyArgs(localMessages);

    // A
    var messages = (await sut.Get()).ToList();

    // A
    Assert.Equal(4, messages.Count);

    for (var id = 3; id <= 6; id++)
    {
      var message = messages.SingleOrDefault(m => m.Id == id);
      Assert.NotNull(message);
    }
  }
}
