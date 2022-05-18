namespace AirAlerts.Stats;

internal class ClientProvider
{
  internal async Task<WTelegram.Client> Get()
  {
    WTelegram.Client client = new WTelegram.Client(Config);

    await client.LoginUserIfNeeded();
    return client;

    static string Config(string what)
    {
      switch (what)
      {
        case "api_id":
          Console.Write("api_key: ");
          return Console.ReadLine();
        case "api_hash":
          Console.Write("api_hash: ");
          return Console.ReadLine();
        case "phone_number":
          Console.Write("phone_number: ");
          return Console.ReadLine();
        case "verification_code": 
          Console.Write("verification_code: "); 
          return Console.ReadLine();
        default:
          // let WTelegramClient decide the default config
          return null;
      }
    }
  }
}
