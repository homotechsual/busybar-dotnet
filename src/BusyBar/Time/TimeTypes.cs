namespace Busy.Bar;

/// <summary>The device's current real-time-clock timestamp.</summary>
/// <param name="Timestamp">ISO 8601 formatted timestamp with timezone.</param>
public sealed record TimestampInfo(string Timestamp);

/// <summary>Requested real-time-clock timestamp.</summary>
/// <param name="Timestamp">ISO 8601 timestamp, e.g. <c>2025-10-02T14:30:45+02:00</c> for local time or <c>2025-10-02T14:30:45Z</c> for UTC. A timezone qualifier is required.</param>
public sealed record TimeSetTimestampParams(string Timestamp);

/// <summary>A time zone the device recognizes.</summary>
/// <param name="Name">Time zone name.</param>
/// <param name="Offset">Time zone offset from UTC.</param>
/// <param name="Abbr">Time zone abbreviation.</param>
public sealed record TimezoneInfo(string Name, string Offset, string Abbr);

/// <summary>Requested timezone.</summary>
/// <param name="Timezone">Timezone name; see <see cref="TimezoneListResponse"/> for the accepted names.</param>
public sealed record TimeSetTimezoneParams(string Timezone);

/// <summary>The list of time zones accepted when setting the device's timezone.</summary>
/// <param name="List">Supported time zones.</param>
public sealed record TimezoneListResponse(IReadOnlyList<TimezoneInfo> List);
