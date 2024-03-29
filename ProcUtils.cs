﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SharpedUtilsCollection;

public static class ProcUtils
{
    /**
     * Execute command in terminal
     */
    public static void Exec(string cmd)
    {
        string[] segments = cmd.Split(' ');

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = segments[0],
            Arguments = String.Join(' ', segments[1..]),
            UseShellExecute = true,
            CreateNoWindow = true,
        };

        var process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
    }
}
