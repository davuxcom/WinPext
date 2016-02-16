using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace frida_windows_package_manager.Services
{
	public static class WinRtPackageDebugSettingsService
	{
		[ComImport]
		[Guid("B1AEC16F-2383-4852-B0E9-8F0B1DC66B4D")]
		internal class PackageDebugSettings { }

		[ComImport]
		[Guid("F27C3930-8029-4AD1-94E3-3DBA417810C1")]
		[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		internal interface IPackageDebugSettings
		{
			int EnableDebugging(
                [MarshalAs(UnmanagedType.LPWStr)]
                string packageFullName,
				[MarshalAs(UnmanagedType.LPWStr)]
			    string debuggerCommandLine,
				IntPtr environment
			);

			int DisableDebugging(
				[MarshalAs(UnmanagedType.LPWStr)]
			    string packageFullName
			);

			// [...]
		}

		private static IPackageDebugSettings _packageDebugSettings;

		static WinRtPackageDebugSettingsService()
		{
			_packageDebugSettings = new PackageDebugSettings() as IPackageDebugSettings;
		}

		public static void SetDebugger(string packageFullName, string debuggerCommandLine)
		{
			_packageDebugSettings.EnableDebugging(packageFullName, debuggerCommandLine, IntPtr.Zero);
		}

		public static void ClearDebugger(string packageFullName)
		{
			_packageDebugSettings.DisableDebugging(packageFullName);
		}

		public static IEnumerable<KeyValuePair<string, string>> EnumerateDebuggers()
		{
			var winRtPackagesKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Classes\ActivatableClasses\Package");

			foreach (var winRtPackage in winRtPackagesKey.GetSubKeyNames())
			{
				var packageDebugKey = winRtPackagesKey.OpenSubKey(winRtPackage).OpenSubKey("DebugInformation");
				if (packageDebugKey != null)
				{
					var firstDebugInfoKeyName = packageDebugKey.GetSubKeyNames().FirstOrDefault();
					if (!string.IsNullOrWhiteSpace(firstDebugInfoKeyName))
					{
						var debugPath = (string)packageDebugKey.OpenSubKey(firstDebugInfoKeyName).GetValue("DebugPath", "");
						yield return new KeyValuePair<string, string>(winRtPackage, debugPath);
					}
				}
			}

			yield break;
		}
	}
}
