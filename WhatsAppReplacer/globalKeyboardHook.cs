using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WhatsAppReplacer
{
    class globalKeyboardHook
    {
        #region Constant, Structure and Delegate Definitions
        /// <summary>
        /// defines the callback type for the hook
        /// </summary>
        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);

        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
        #endregion

        #region Instance Variables
        /// <summary>
        /// The collections of keys to watch for
        /// </summary>
        public List<Keys> HookedKeys = new List<Keys>();
        /// <summary>
        /// Handle to the hook, need this to unhook and call the next hook
        /// </summary>
        IntPtr hhook = IntPtr.Zero;
        #endregion

        #region Events
        /// <summary>
        /// Occurs when one of the hooked keys is pressed
        /// </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when one of the hooked keys is released
        /// </summary>
        public event KeyEventHandler KeyUp;
        #endregion

        #region Constructors and Destructors
        /// <summary>
        /// Initializes a new instance of the <see cref="globalKeyboardHook"/> class and installs the keyboard hook.
        /// </summary>
        public globalKeyboardHook()
        {
            hook();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="globalKeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
        /// </summary>
        ~globalKeyboardHook()
        {
            unhook();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Installs the global hook
        /// </summary>
        public void hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, hookProc, hInstance, 0);
        }

        /// <summary>
        /// Uninstalls the global hook
        /// </summary>
        public void unhook()
        {
            UnhookWindowsHookEx(hhook);
        }

        /// <summary>
        /// The callback for the keyboard hook
        /// </summary>
        /// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
        /// <param name="wParam">The event type</param>
        /// <param name="lParam">The keyhook event information</param>
        /// <returns></returns>
        public int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Keys key = (Keys)lParam.vkCode;
                if (HookedKeys.Contains(key))
                {
                    KeyEventArgs kea = new KeyEventArgs(key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        KeyDown(this, kea);
                    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    {
                        KeyUp(this, kea);
                    }
                    if (kea.Handled)
                        return 1;
                }
            }
            return CallNextHookEx(hhook, code, wParam, ref lParam);
        }
        #endregion

        #region DLL imports
        /// <summary>
        /// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
        /// </summary>
        /// <param name="idHook">The id of the event you want to hook</param>
        /// <param name="callback">The callback.</param>
        /// <param name="hInstance">The handle you want to attach the event to, can be null</param>
        /// <param name="threadId">The thread you want to attach the event to, can be null</param>
        /// <returns>a handle to the desired hook</returns>
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        /// <summary>
        /// Unhooks the windows hook.
        /// </summary>
        /// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
        /// <returns>True if successful, false otherwise</returns>
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        /// <summary>
        /// Calls the next hook.
        /// </summary>
        /// <param name="idHook">The hook id</param>
        /// <param name="nCode">The hook code</param>
        /// <param name="wParam">The wparam.</param>
        /// <param name="lParam">The lparam.</param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

        /// <summary>
        /// Loads the library.
        /// </summary>
        /// <param name="lpFileName">Name of the library</param>
        /// <returns>A handle to the library</returns>
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);
        #endregion
    }

    public static class HookManager
    {
        public static string activeAppName;
        public static void SubscribeToWindowEvents()
        {
            if (windowEventHook == IntPtr.Zero)
            {
                windowEventHook = SetWinEventHook(
                    EVENT_SYSTEM_FOREGROUND, // eventMin
                    EVENT_SYSTEM_FOREGROUND, // eventMax
                    IntPtr.Zero,             // hmodWinEventProc
                    WindowEventCallback,     // lpfnWinEventProc
                    0,                       // idProcess
                    0,                       // idThread
                    WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS);

                if (windowEventHook == IntPtr.Zero)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private static void WindowEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
        {
            uint processId;
            GetWindowThreadProcessId(hwnd, out processId);

            Process myProcess = Process.GetProcessById(int.Parse(processId.ToString()));

            //if (myProcess.ProcessName.ToLower().Equals("whatsapp"))
            activeAppName = myProcess.ProcessName.ToLower();

            Console.WriteLine("App {0}", myProcess.ProcessName);
        }

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private static IntPtr windowEventHook;

        private delegate void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, int dwflags);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int UnhookWinEvent(IntPtr hWinEventHook);

        private const int WINEVENT_INCONTEXT = 4;
        private const int WINEVENT_OUTOFCONTEXT = 0;
        private const int WINEVENT_SKIPOWNPROCESS = 2;
        private const int WINEVENT_SKIPOWNTHREAD = 1;

        private const int EVENT_SYSTEM_FOREGROUND = 3;
    }

    public static class EventLoop
    {
        public static void Run()
        {
            MSG msg;

            while (true)
            {
                if (PeekMessage(out msg, IntPtr.Zero, 0, 0, PM_REMOVE))
                {
                    if (msg.Message == WM_QUIT)
                        break;

                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr Hwnd;
            public uint Message;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public System.Drawing.Point Point;
        }

        const uint PM_NOREMOVE = 0;
        const uint PM_REMOVE = 1;

        const uint WM_QUIT = 0x0012;

        [DllImport("user32.dll")]
        private static extern bool PeekMessage(out MSG lpMsg, IntPtr hwnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
        [DllImport("user32.dll")]
        private static extern bool TranslateMessage(ref MSG lpMsg);
        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage(ref MSG lpMsg);
    }
}
