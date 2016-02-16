using frida_windows_package_manager.Models;
using System;
using System.Diagnostics;

namespace frida_windows_package_manager.Services
{
	public static class PackageDebugSettingsService
	{
		private static void ValidatePackage(Package package)
		{
			if (!string.IsNullOrWhiteSpace(package.Manifest.Exe) && !string.IsNullOrWhiteSpace(package.Manifest.AppxPackageFamilyName))
			{
				throw new ArgumentException("Package App and PackageName values cannnot both be set.", "package");
			}

            if (string.IsNullOrWhiteSpace(package.Manifest.Exe) && string.IsNullOrWhiteSpace(package.Manifest.AppxPackageFamilyName))
			{
				throw new ArgumentException("Package App and PackageName values cannnot both be null.", "package");
			}
		}

        public static bool PackageOperationNeedsElevation(Package package)
        {
            return package.IsExe;
        }

		public static void SetDebuggerForPackage(Package package)
		{
			ValidatePackage(package);

			if (package.IsExe)
			{
                ImageFileExecutionOptionsService.SetDebugger(package.Manifest.Exe, GenerateCommandLineForPackage(package));
			}
            else
			{
                WinRtPackageDebugSettingsService.SetDebugger(AppxAppModel.PackageFullNameFromFamilyName(package.Manifest.AppxPackageFamilyName), GenerateCommandLineForPackage(package));
			}
		}

        private static string GenerateCommandLineForPackage(Package package)
        {
            if (!string.IsNullOrWhiteSpace(package.Manifest.Exe))
            {
                return string.Format("\"{0}\" Exec \"{1}\"", Process.GetCurrentProcess().MainModule.FileName, package.PackagePath);
            }
            else
            {
                return string.Format("\"{0}\" Attach \"{1}\"", Process.GetCurrentProcess().MainModule.FileName, package.PackagePath);
            }
        }

		public static void ClearDebuggerForPackage(Package package)
		{
			ValidatePackage(package);

			if (package.IsExe)
			{
				ImageFileExecutionOptionsService.ClearDebugger(package.Manifest.Exe);
			}
            else
			{
                WinRtPackageDebugSettingsService.ClearDebugger(AppxAppModel.PackageFullNameFromFamilyName(package.Manifest.AppxPackageFamilyName));
			}
		}
	}
}
