namespace AirAlerts.Stats;

internal record Message(long Id, DateTime Date, string Text);

internal record Event(DateTime Date, EventType Type, string Tag);

internal record Period(DateTime Start, DateTime End);

internal enum EventType { On, Off };
