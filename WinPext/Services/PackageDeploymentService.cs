using System.Diagnostics;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using frida_windows_package_manager.Services;
using frida_windows_package_manager.Models;

namespace frida_windows_package_manager
{
    public class PackageDeploymentService
    {
        public enum DeploymentAction
        {
            Install_Package,
            Remove_Package,
        }

        public static void DeployPackageElevated(Package package, DeploymentAction action)
        {
            switch (action)
            {
                case DeploymentAction.Install_Package:
					PackageDebugSettingsService.SetDebuggerForPackage(package);
                    PackageStoreService.Add(package);
                    break;

                case DeploymentAction.Remove_Package:
					PackageDebugSettingsService.ClearDebuggerForPackage(package);
                    PackageStoreService.Delete(package);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        public static bool DeployPackageUnElevated(Package package, DeploymentAction action)
        {
            if (PackageDebugSettingsService.PackageOperationNeedsElevation(package))
            {
                try
                {
                    // Format: <path-to-this.exe> VERB "<path-to-package>"
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Process.GetCurrentProcess().MainModule.FileName,
                        Arguments = action.ToString().ToUpper() + " \"" + package.PackagePath + "\"",
                        Verb = "runas"
                    });
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Failed to start elevated worker instance:\n" + e.StackTrace);
                    return false;
                }
            }
            else
            {
                DeployPackageElevated(package, action);
                return true;
            }
        }
    }
}
