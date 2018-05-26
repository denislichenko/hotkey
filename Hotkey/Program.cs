using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hotkey
{
    static class Program
    {
        const int KEYEVENTF_EXTENDEDKEY = 0x1;
        const int KEУEVENTF_KEYUP = 0x2;

        private const int WH_KEYBOARD_LL = 13; 
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static bool lastKeyWasLetter = false;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            _hookID = SetHook(_proc);
            Application.Run();

            UnhookWindowsHookEx(_hookID);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                Keys key = (Keys)Marshal.ReadInt32(lParam);

                switch(key)
                {
                    case Keys.NumPad9:
                        TogglePageUp(); break;
                    case Keys.NumPad3:
                        TogglePageDown(); break;
                    case Keys.NumPad7:
                        ToggleHome(); break;
                    case Keys.NumPad1:
                        ToggleEnd(); break;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static void TogglePageUp()
        {
            keybd_event(0x21, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event(0x21, 0x45, KEYEVENTF_EXTENDEDKEY | KEУEVENTF_KEYUP, (UIntPtr)0);
        }

        private static void TogglePageDown()
        {
            keybd_event(0x22, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event(0x22, 0x45, KEYEVENTF_EXTENDEDKEY | KEУEVENTF_KEYUP, (UIntPtr)0);
        }

        private static void ToggleEnd()
        {
            keybd_event(0x23, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event(0x23, 0x45, KEYEVENTF_EXTENDEDKEY | KEУEVENTF_KEYUP, (UIntPtr)0);
        }

        private static void ToggleHome()
        {
            keybd_event(0x24, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);
            keybd_event(0x24, 0x45, KEYEVENTF_EXTENDEDKEY | KEУEVENTF_KEYUP, (UIntPtr)0);
        }
    }
}
