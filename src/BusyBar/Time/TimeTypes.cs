namespace Busy.Bar;

public sealed record TimestampInfo(string Timestamp);

public sealed record TimeSetTimestampParams(string Timestamp);

public sealed record TimezoneInfo(string Name, string Offset, string Abbr);

public sealed record TimeSetTimezoneParams(string Timezone);

public sealed record TimezoneListResponse(IReadOnlyList<TimezoneInfo> List);
