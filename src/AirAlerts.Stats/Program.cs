using AirAlerts.Stats;

var client = await new ClientProvider().Get();
var dataProvider = new DataProvider(client);
var messages = dataProvider.Get();

var regionalEvents = await DataProcessor.GetEvents(messages);
var compactedEvents = DataProcessor.Compact(regionalEvents);
var periods = DataProcessor.GetPeriods(compactedEvents);

Directory.CreateDirectory("plots");
var plotter = new Plotter();

var daily = DataProcessor.GetDailyStats(periods);
plotter.SaveGraphics("plots/daily.png", "Air Alerts Duration, hrs", daily);

foreach (var lookback in new[] {7, 14, 30})
{
  var dailyLookback = DataProcessor.GetDailyLookback(daily, lookback);

  plotter.SaveGraphics(
    $"plots/daily-{lookback}.png", 
    $"Air Alerts Duration {lookback}D lookback, hrs",
    dailyLookback);
}
