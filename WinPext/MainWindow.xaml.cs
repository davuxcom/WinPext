using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using frida_windows_package_manager.Models;
using Microsoft.Win32;
using System.Diagnostics;

namespace frida_windows_package_manager
{
    public partial class MainWindow : Window
    {
        public IList<Package> Packages { get; private set; }

        public MainWindow()
        {
            


            InitializeComponent();

            RegisterFileTypeAssociations();
            RegisterForStartup();

            Packages = PackageService.EnumeratePackages().ToList();
            TitleText.Text = Packages.Any() ? "Installed Extensions" : "No Extensions Installed";
            DataContext = this;
        }

        private void UninstallPackageButton_Click(object sender, RoutedEventArgs e) 
        {
            var package = (Package)((Button)sender).DataContext;

            if (PackageDeploymentService.DeployPackageUnElevated(package, PackageDeploymentService.DeploymentAction.Remove_Package))
            {
                Packages.Remove(package);
                DataContext = null;
                DataContext = this;
            }
        }

        private void RegisterFileTypeAssociations()
        {
            FileTypeAssociationService.ProgId = "frida.pkgmgr.1";
            FileTypeAssociationService.RegisterFileExtension(".pext");
        }

        private void RegisterForStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            rk.SetValue("Frida-PkgMgr",
                "\"" + Process.GetCurrentProcess().MainModule.FileName + "\" /register");
        }
    }
}
