using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Readme
{
    class HotKey
    {
        // Boss Key
        // If Function Execute Success, The Return Value is not 0.
        // If Function Execute Failure, The Return Value is 0. 
        // We Can Get Fault Message By Call Function GetLastError
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(
                                                    IntPtr hWnd,                // The Defined Form Handle
                                                    int id,                     // The Defined HotKey ID£¨It Could not Be Same With Other ID£©
                                                    KeyModifiers fsModifiers,   // Define HotKey Be Useful with Alt¡¢Ctrl¡¢Shift¡¢Windows
                                                    Keys vk                     // Define HotKey's Content
                                                 );
        [DllImport("user32.dll", SetLastError = true)]

        public static extern bool UnregisterHotKey(
                                                    IntPtr hWnd,                // Form Handle of HotKey Which Will Be Cancel
                                                    int id                      // ID of HotKey Which Will Be Cancel
                                                  );
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Ctrl = 2,
            Shift = 4,
            WindowsKey = 8
        }
    }
}
