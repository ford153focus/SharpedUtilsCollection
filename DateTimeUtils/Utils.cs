// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;

namespace SharpedUtilsCollection.DateTimeUtils;

public class Utils
{
    public const int SecsInWeek = 604800;  // 7 * 24 * 60 * 60

    public static double UnixNow()
    {
        DateTime unixEpochBegin = new DateTime(1970, 1, 1);
        TimeSpan span = DateTime.UtcNow - unixEpochBegin;
        return span.TotalSeconds;
    }
}