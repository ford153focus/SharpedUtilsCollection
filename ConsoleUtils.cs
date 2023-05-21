// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Text;

namespace SharpedUtilsCollection;

public class ConsoleUtils
{
    private static readonly object ConsoleLock = new object();

    public static void WriteError(string message)
    {
        lock (ConsoleLock)
        {
            Console.OutputEncoding = Encoding.Default;
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(message);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
        }
    }

    public static void WriteSuccess(string message)
    {
        lock (ConsoleLock)
        {
            Console.OutputEncoding = Encoding.Default;
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(message);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
        }
    }

    public static void WriteWarning(string message)
    {
        lock (ConsoleLock)
        {
            Console.OutputEncoding = Encoding.Default;
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(message);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
