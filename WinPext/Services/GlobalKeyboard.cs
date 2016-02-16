using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace frida_windows_package_manager.Services
{
    class GlobalKeyboard
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private static int VK_LCONTROL = 0xA2;
        //private static int VK_LSHIFT = 0xA0;

        public static bool IsDebugKeyDown()
        {
            return (GetAsyncKeyState(VK_LCONTROL) & 0x8000) > 0;
        }
    }
}
