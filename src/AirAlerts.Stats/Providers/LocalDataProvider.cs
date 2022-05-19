using System.Text.Json;

namespace AirAlerts.Stats;

internal interface ILocalDataProvider
{
  IList<Message>? GetLocal();
  void SaveLocal(IEnumerable<Message> messages);
}

internal class LocalDataProvider : ILocalDataProvider
{
  private const string _file = "data/data.json";

  public IList<Message>? GetLocal()
  {
    try
    {
      var text = File.ReadAllText(_file);
      return JsonSerializer.Deserialize<List<Message>>(text);
    }
    catch (Exception)
    {
      return new List<Message>();
    }
  }

  public void SaveLocal(IEnumerable<Message> messages)
  {
    Directory.CreateDirectory("data");

    try
    {
      var text = JsonSerializer.Serialize(messages);
      File.WriteAllText(_file, text);
    }
    catch
    {
    }
  }
}
