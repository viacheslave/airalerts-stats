namespace AirAlerts.Stats;

internal class Plotter
{
  internal void SaveGraphics(string file, string title, IDictionary<DateTime, int> daily)
  {
    double[] dataX = daily.Keys.Select(k => k.ToOADate()).ToArray();
    double[] dataY = daily.Values.Select(v => 1d * v / 60).ToArray();

    var plt = new ScottPlot.Plot(800, 600);
    plt.XAxis.DateTimeFormat(true);

    plt.SetAxisLimitsX(daily.Keys.Min().ToOADate(), daily.Keys.Max().ToOADate());
    plt.SetAxisLimitsY(0, 24);

    plt.Title(title);
    plt.AddScatter(dataX, dataY);
    plt.SaveFig(file);
  }
}