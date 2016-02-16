using Microsoft.Win32;
using System.Diagnostics;

namespace frida_windows_package_manager
{
    public class FileTypeAssociationService
    {
        private static string _progId;
        public static string ProgId
        {
            get { return _progId; }
            set
            {
                if (_progId != value)
                {
                    _progId = value;
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\" + _progId + @"\shell\open\command", null,
                        "\"" + Process.GetCurrentProcess().MainModule.FileName + "\" \"%1\"");
                }
                
            }
        }

        public static void RegisterFileExtension(string ext) // .ext
        {
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Classes\" + ext, null, ProgId);
        }
    }
}
