using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace frida_windows_package_manager.Services
{
    public class AppxAppModel
    {
        class NativeMethods
        {
            public enum PackageFilters :int {
                PACKAGE_FILTER_HEAD = 0x00000010
            }

            [DllImport("kernel32.dll")]
            public static extern int FindPackagesByPackageFamily(
                [MarshalAs(UnmanagedType.LPWStr)]
                string PackageFamilyName,
                PackageFilters PackageFilters,
                ref int packageFullNamesCount,
                IntPtr packageFullNames,
                ref int bufferLength,
                IntPtr buffer,
                IntPtr packageProperties);
            public static int ERROR_SUCCESS = 0;
        }

        public static string PackageFullNameFromFamilyName(string familyName)
        {
            int packageFullNamesCount = 1;
            IntPtr packageFullNames = Marshal.AllocCoTaskMem(IntPtr.Size * 8);

            IntPtr buffer = Marshal.AllocCoTaskMem(4096); ; // big buffer
            int bufferLength = 4096; // big buffer

            var ret = NativeMethods.FindPackagesByPackageFamily(
                familyName, NativeMethods.PackageFilters.PACKAGE_FILTER_HEAD,
                ref packageFullNamesCount, packageFullNames, ref bufferLength, buffer, IntPtr.Zero);

            if (NativeMethods.ERROR_SUCCESS == ret)
            {
                return Marshal.PtrToStringUni(Marshal.ReadIntPtr(packageFullNames));
            }
            throw new ApplicationException("Couldn't call FindPackagesByPackageFamily: " + ret);
        }
    }
}
