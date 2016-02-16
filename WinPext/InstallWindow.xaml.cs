using frida_windows_package_manager.Models;
using System;
using System.Linq;
using System.Windows;

namespace frida_windows_package_manager
{
    public partial class InstallWindow : Window
    {
        public Package Package { get; private set; }
        public string TitleText { get; private set; }
        public string ButtonText { get; private set; }
        public bool IsInstalled { get; private set; }
        public bool NeedsElevation { get; private set; }

        public InstallWindow(string packagePath)
        {
            InitializeComponent();

            Package = Package.Load(packagePath);
            IsInstalled = PackageService.IsInstalled(Package);
            NeedsElevation = Package.IsExe;
            var IsConflict = PackageService.IsConflictingWithInstalledPackage(Package);
            TitleText = IsInstalled ? "Remove extension" : "Install extension";
            ButtonText = IsInstalled ? "Remove" :
                (IsConflict ? "Overwrite existing extension" : "Install");

            DataContext = this;
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (PackageDeploymentService.DeployPackageUnElevated(Package, IsInstalled ? PackageDeploymentService.DeploymentAction.Remove_Package : PackageDeploymentService.DeploymentAction.Install_Package))
            {
                Close();
            }
        }
    }
}
