using frida_windows_package_manager.Models;
using frida_windows_package_manager.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace frida_windows_package_manager
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                // exec "<path-to-package>"
                var arg0 = e.Args[0].ToLower();
                var isDebug = arg0 == "execdebug";
                
                if (arg0 == "exec" || isDebug)
                {
                    FridaService.RunProcessWithPackage(e.Args[2], e.Args.Skip(3).ToArray(), isDebug || GlobalKeyboard.IsDebugKeyDown(), Package.Load(e.Args[1]));
                    // Stay resident. Detaching frida when the target quits will tear down this process.
                    return;
                }

				// attach "<path-to-package>"
				if (arg0 == "attach")
				{
                    isDebug = true;
					FridaService.AttachToProcessWithPackage(uint.Parse(e.Args[3]), "", isDebug || GlobalKeyboard.IsDebugKeyDown(), Package.Load(e.Args[1]));
					// Stay resident. Detaching frida when the target quits will tear down this process.
					return;
				}

                // <DeploymentAction> "<path-to-package>"
                foreach (PackageDeploymentService.DeploymentAction operation in Enum.GetValues(typeof(PackageDeploymentService.DeploymentAction)))
                {
                    if (operation.ToString().ToUpper() == e.Args[0].ToUpper())
                    {
                        PackageDeploymentService.DeployPackageElevated(Package.Load(e.Args[1]), operation);
                        Environment.Exit(0);
                    }
                }

                if (arg0 == "/register")
                {
                    // re-register appx apps, since the registration goes away after logout.
                    foreach (var pkg in PackageStoreService.Enumerate())
                    {
                        if (!pkg.IsExe)
                        {
                            PackageDeploymentService.DeployPackageUnElevated(pkg, PackageDeploymentService.DeploymentAction.Install_Package);
                        }
                    }
                    return;
                }

                // Default to filetype association launch for package
                // <path-to-package>
                new InstallWindow(e.Args[0]).Show();
            }
            else
            {
                // Package Manager UI
                new MainWindow().Show();
            }
        }
    }
}
