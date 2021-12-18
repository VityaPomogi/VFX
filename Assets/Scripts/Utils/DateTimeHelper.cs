using System;

public class DateTimeHelper
{
    private static readonly DateTime epochStart = new System.DateTime( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );

    public static DateTime GetDateTimeFromUnixTimeSeconds( long epochSeconds, bool isLocalTime )
    {
        DateTimeOffset _dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds( epochSeconds );
        if (isLocalTime == true)
        {
            return _dateTimeOffset.LocalDateTime;
        }

        return _dateTimeOffset.DateTime;
    }

    public static DateTime GetDateTimeFromUnixTimeMilliseconds( long epochMilliseconds, bool isLocalTime )
    {
        DateTimeOffset _dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds( epochMilliseconds );
        if (isLocalTime == true)
        {
            return _dateTimeOffset.LocalDateTime;
        }

        return _dateTimeOffset.DateTime;
    }

    public static long GetUnixTimeSecondsFromDateTime( DateTime targetDateTime )
    {
        return ( long )targetDateTime.Subtract( epochStart ).TotalSeconds;
    }

    public static long GetUnixTimeMillisecondsFromDateTime( DateTime targetDateTime )
    {
        return ( long )targetDateTime.Subtract( epochStart ).TotalMilliseconds;
    }
}
