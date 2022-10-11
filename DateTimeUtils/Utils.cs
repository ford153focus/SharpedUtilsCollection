// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;

namespace SharpedUtilsCollection.DateTimeUtils
{
    public class Utils
    {
        public const int SecsInWeek = 7 * 24 * 60 * 60;

        public static double UnixNow()
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return timeSpan.TotalSeconds;
        }
    }
}
