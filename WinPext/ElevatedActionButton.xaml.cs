using System.Windows.Controls;
using System.Windows;
using frida_windows_package_manager.Models;

namespace frida_windows_package_manager
{
    public partial class ElevatedActionButton : Button
    {
        public ElevatedActionButton()
        {
            InitializeComponent();

            Loaded += ElevatedActionButton_Loaded;
            DataContextChanged += ElevatedActionButton_DataContextChanged;
            
        }

        private void ElevatedActionButton_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var pkg = DataContext as Package;

            if (pkg != null && pkg.IsExe)
            {
                Shield.Visibility = Visibility.Visible;
                Shield.Source = UserAccountControlService.GetShield();
            }

            var installWindow = DataContext as InstallWindow;
            if (installWindow != null && installWindow.Package.IsExe)
            {
                Shield.Visibility = Visibility.Visible;
                Shield.Source = UserAccountControlService.GetShield();
            }
        }

        private void ElevatedActionButton_Loaded(object sender, RoutedEventArgs e)
        {


        }
    }
}
