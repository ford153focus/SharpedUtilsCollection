// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Text;

namespace SharpedUtilsCollection;

public class StringUtils
{
    /**
        <summary>Convert UTF8-string to UTF16</summary>
        */
    public static string EightToSixteen(string u8Str)
    {
        //UTF8 bytes
        byte[] u8Bytes = Encoding.UTF8.GetBytes(u8Str);

        //Converting to Unicode from UTF8 bytes
        byte[] u16Bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, u8Bytes);

        //Getting string from Unicode bytes
        string u16Str = Encoding.Unicode.GetString(u16Bytes);

        return u16Str;
    }

    /**
        <summary>Convert cp1251-string to UTF8</summary>
        */
    public static string CyrToUnicode(string sourceString)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        byte[] sourceBytes = Encoding.GetEncoding(1251).GetBytes(sourceString);
        return Encoding.UTF8.GetString(sourceBytes);
    }

    private static int ComputeLevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source)) return 0;
        if (string.IsNullOrEmpty(target)) return 0;
            
        if (source == target) return source.Length;

        int sourceWordCount = source.Length;
        int targetWordCount = target.Length;

        // Step 1
        if (sourceWordCount == 0)
            return targetWordCount;

        if (targetWordCount == 0)
            return sourceWordCount;

        int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

        // Step 2
        for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
        for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

        for (int i = 1; i <= sourceWordCount; i++)
        {
            for (int j = 1; j <= targetWordCount; j++)
            {
                // Step 3
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                // Step 4
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }

        return distance[sourceWordCount, targetWordCount];
    }

    /// <url>https://social.technet.microsoft.com/wiki/contents/articles/26805.c-calculating-percentage-similarity-of-2-strings.aspx</url>
    public static double CalculateSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source)) return 0;
        if (string.IsNullOrEmpty(target)) return 0;
            
        if (source == target) return 1.0;

        int stepsToSame = ComputeLevenshteinDistance(source, target);
        double max = Math.Max(source.Length, target.Length);
        return 1.0 - stepsToSame/max;
    }
}