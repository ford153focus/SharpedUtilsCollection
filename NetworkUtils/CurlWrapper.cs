// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SharpedUtilsCollection.NetworkUtils;

public static class CurlWrapper
{
    private static List<string> GetArguments(
        string url, 
        Dictionary<string,string>? headers=null,
        string proxy = "socks5h://127.0.0.1:9050"
    )
    {
        List<string> arguments = new();

        if (headers is not null)
        {
            var headerOptions = headers.Select(header => $"--header \"{header.Key}:{header.Value}\"");
            arguments.AddRange(headerOptions);
        }

        arguments.Add("--compressed");
        arguments.Add("--connect-timeout 15");
        arguments.Add("--continue-at -");
        arguments.Add("--http0.9");
        arguments.Add("--insecure");
        arguments.Add("--insecure");
        arguments.Add("--location");
        arguments.Add("--progress-bar");
        arguments.Add("--range 0-");
        arguments.Add("--remote-time");
        arguments.Add("--retry 15");
        arguments.Add("--retry-delay 3");
        // arguments.Add("--verbose");
        arguments.Add($"--proxy {proxy}");
        arguments.Add(url);
        return arguments;
    }
    
    public static async Task<string> DownloadPage(
        string url, 
        Dictionary<string,string>? headers=null,
        string proxy = "socks5h://127.0.0.1:9050"
    )
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) 
            throw new Exception("Unsupported platform");

        List<string> arguments = GetArguments(url, headers, proxy);

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.Arguments = string.Join(" ", arguments);
        startInfo.CreateNoWindow = true;
        startInfo.FileName = "/usr/bin/curl";
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        
        Process proc = new Process();
        proc.StartInfo = startInfo;
        proc.Start();
        string output = await proc.StandardOutput.ReadToEndAsync();
        proc.WaitForExit();
        return output;
    }

    /**
     * @param {int} timeOut Time out in seconds
     */
    public static void DownloadFile(
        string url, string path, 
        Dictionary<string,string>? headers=null,
        string proxy = "socks5h://127.0.0.1:9050"
    )
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) 
            throw new Exception("Unsupported platform");
        
        List<string> arguments = GetArguments(url, headers, proxy);
        arguments.Insert(arguments.Count-2, $"--output {path}");
        
        // ReSharper disable once UseObjectOrCollectionInitializer
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.Arguments = string.Join(" ", arguments);
        startInfo.CreateNoWindow = true;
        startInfo.FileName = "/usr/bin/curl";
        startInfo.UseShellExecute = true;
        
        Process proc = new Process();
        proc.StartInfo = startInfo;
        proc.Start();
        proc.WaitForExit();
    }
}
