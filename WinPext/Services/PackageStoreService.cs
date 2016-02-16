using frida_windows_package_manager.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frida_windows_package_manager.Services
{
    class PackageStoreService
    {
        private static string KeyName = @"Software\Frida-PkgMgr\extensions";
        private static string PackagePathName = "PackagePath";
        public static void Add(Package package)
        {
            Registry.CurrentUser.CreateSubKey(KeyName).
                CreateSubKey(package.ExtendsFriendlyName).SetValue(PackagePathName, package.PackagePath);
        }

        public static void Delete(Package package)
        {
            Registry.CurrentUser.CreateSubKey(KeyName).DeleteSubKey(package.ExtendsFriendlyName);
        }

        public static IEnumerable<Package> Enumerate()
        {
            var ifeoKey = Registry.CurrentUser.OpenSubKey(KeyName);
            foreach (var app in ifeoKey.GetSubKeyNames())
            {
                var packagePath = (string)ifeoKey.OpenSubKey(app).GetValue(PackagePathName, "");
                if (Package.CanLoad(packagePath))
                {
                    yield return Package.Load(packagePath);
                }
            }
            yield break;
        }
    }
}
