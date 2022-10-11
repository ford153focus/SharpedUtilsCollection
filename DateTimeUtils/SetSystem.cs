// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpedUtilsCollection.DateTimeUtils
{
    public class SystemDateTime
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct SystemTime
        {
            public short wYear;
            public short wMonth;
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            // ReSharper disable once MemberCanBePrivate.Local
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            // ReSharper disable once FieldCanBeMadeReadOnly.Local
            // ReSharper disable once MemberCanBePrivate.Local
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetSystemTime(ref SystemTime st);

        public static void Set(DateTime dateTime)
        {
            SystemTime systemTime = new SystemTime
            {
                wYear = (short) dateTime.Year, // must be short
                wMonth = (short) dateTime.Month,
                wDay = (short) dateTime.Day,
                wHour = (short) dateTime.Hour,
                wMinute = (short) dateTime.Minute,
                wSecond = (short) dateTime.Second
            };

            SetSystemTime(ref systemTime); // invoke this method.
        }
    }

    public class SystemTimeZone
    {
        public static void Set(string timeZoneId)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "tzutil.exe",
                Arguments = $"/s \"{timeZoneId}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var process = Process.Start(processStartInfo);
            if (process == null) return;
            process.WaitForExit();
            TimeZoneInfo.ClearCachedData();
        }
    }
}
