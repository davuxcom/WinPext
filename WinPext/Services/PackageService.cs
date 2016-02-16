using frida_windows_package_manager.Models;
using frida_windows_package_manager.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace frida_windows_package_manager
{
	public class PackageService
    {
        public static IEnumerable<Package> EnumeratePackages()
        {
            foreach (var entry in ImageFileExecutionOptionsService.EnumerateDebuggers())
            {
				var packagePath = GetPackagePath(entry.Value);
                if (!string.IsNullOrWhiteSpace(packagePath) &&
                    Package.CanLoad(packagePath))
                {
                    yield return Package.Load(packagePath);
                }
			}

			foreach (var entry in WinRtPackageDebugSettingsService.EnumerateDebuggers())
			{
				var packagePath = GetPackagePath(entry.Value);
				if (!string.IsNullOrWhiteSpace(packagePath))
					yield return Package.Load(packagePath);
			}

			yield break;
        }

		private static string GetPackagePath(string entryValue)
		{
			var exePath = Process.GetCurrentProcess().MainModule.FileName;

			if (entryValue.ToLower().Contains(exePath.ToLower()))
			{
				// Format: "<path-to-this.exe>" RUN "<path-to-package>"
				var lastQuoteIndex = entryValue.LastIndexOf("\"", entryValue.Length - 2);
				var PackagePath = entryValue.Substring(lastQuoteIndex + 1, entryValue.Length - lastQuoteIndex - 2);
				return PackagePath;
			}

			return string.Empty;
		}

        public static bool IsInstalled(Package package)
        {
            return EnumeratePackages().Where(pkg => pkg.PackagePath == package.PackagePath).Any();
        }

        public static bool IsConflictingWithInstalledPackage(Package package)
        {
            return EnumeratePackages().Where(pkg => pkg.ExtendsFriendlyName == package.ExtendsFriendlyName &&
            pkg.PackagePath != package.PackagePath).Any();
        }
    }
}
