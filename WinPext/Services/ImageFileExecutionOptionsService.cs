using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace frida_windows_package_manager.Services
{
    class ImageFileExecutionOptionsService
    {
        private static string KeyName = @"Software\Microsoft\Windows NT\CurrentVersion\Image File Execution Options";

        public static void SetDebugger(string exeName, string debuggerCommandLine)
        {
            Registry.LocalMachine.OpenSubKey(KeyName, true).
                CreateSubKey(exeName).SetValue("Debugger", debuggerCommandLine);
        }

        public static void ClearDebugger(string exeName)
        {
            Registry.LocalMachine.OpenSubKey(KeyName, true).DeleteSubKey(exeName);
        }

        public static IEnumerable<KeyValuePair<string, string>> EnumerateDebuggers()
        {
            var ifeoKey = Registry.LocalMachine.OpenSubKey(KeyName);
            foreach (var app in ifeoKey.GetSubKeyNames())
            {
                yield return new KeyValuePair<string, string>(app,
                    (string)ifeoKey.OpenSubKey(app).GetValue("Debugger", ""));
            }
            yield break;
        }
    }
}
