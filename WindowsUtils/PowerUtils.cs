// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SharpedUtilsCollection.WindowsUtils;

// via https://www.codeproject.com/Tips/480049/Shut-Down-Restart-Log-off-Lock-Hibernate-or-Sleep
public class PowerUtils
{
    [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

    [DllImport("user32")]
    private static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

    [DllImport("user32")]
    private static extern void LockWorkStation();

    public static void ShutDown()
    {
        Process.Start("shutdown", "/s /t 0");
    }

    public static void Reboot()
    {
        Process.Start("shutdown", "/r /t 0");
    }

    public static void LockSession()
    {
        LockWorkStation();
    }

    public static void LogOut()
    {
        ExitWindowsEx(0, 0);
    }

    public static void Hibernate()
    {
        SetSuspendState(true, true, true);
    }

    public static void Sleep()
    {
        SetSuspendState(false, true, true);
    }
}