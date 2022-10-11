// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;

namespace SharpedUtilsCollection.DateTime
{
    public class DateTimeUtils
    {
        public const int SecsInWeek = 7 * 24 * 60 * 60;

        public static double UnixNow()
        {
            TimeSpan timeSpan = System.DateTime.UtcNow - new System.DateTime(1970, 1, 1);
            return timeSpan.TotalSeconds;
        }
    }
}
