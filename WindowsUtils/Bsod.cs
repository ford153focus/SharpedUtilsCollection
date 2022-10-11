// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Runtime.InteropServices;

namespace SharpedUtilsCollection.WindowsUtils
{
    public class Bsod
    {
        [DllImport("ntdll.dll")]
        private static extern uint RtlAdjustPrivilege(int privilege, bool bEnablePrivilege, bool isThreadPrivilege, out bool previousValue);

        [DllImport("ntdll.dll")]
        private static extern uint NtRaiseHardError(uint errorStatus, uint numberOfParameters, uint unicodeStringParameterMask, IntPtr parameters, uint validResponseOption, out uint response);

        public static void Invoke()
        {
            // ReSharper disable once JoinDeclarationAndInitializer
            uint __;
            __ = RtlAdjustPrivilege(19, true, false, out _);
            __ = NtRaiseHardError(0xc0000022, 0, 0, IntPtr.Zero, 6, out _);
        }
    }
}
