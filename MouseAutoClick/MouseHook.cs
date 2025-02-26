using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseAutoClick
{
    public class MouseHook
    {
        private LowLevelMouseProc _proc; 
        private IntPtr _hookID = IntPtr.Zero;
        private bool _isCapturing = false;

        public event EventHandler<MousePositionEventArgs> MousePositionCaptured;
        public event EventHandler<MouseSideButtonEventArgs> MouseSideButtonPressed;

        public bool IsCapturing
        {
            get { return _isCapturing; }
            set { _isCapturing = value; }
        }

        public MouseHook()
        {
            _proc = HookCallback; // 在构造函数中初始化
        }

        public void StartHook()
        {
            _hookID = SetHook(_proc);
        }

        public void StopHook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // 改为实例方法
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));

                // 检查坐标捕获
                if (wParam == (IntPtr)WM_LBUTTONDOWN && _isCapturing)
                {
                    MousePositionCaptured?.Invoke(this, new MousePositionEventArgs(hookStruct.pt.x, hookStruct.pt.y));
                }
                // 检查侧键 (XBUTTON1 和 XBUTTON2)
                else if (wParam == (IntPtr)WM_XBUTTONDOWN)
                {
                    int button = (int)(hookStruct.mouseData >> 16); // 高16位包含侧键信息
                    if (button == 1 || button == 2) // XBUTTON1 = 1, XBUTTON2 = 2
                    {
                        MouseSideButtonPressed?.Invoke(this, new MouseSideButtonEventArgs(button));
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        // Windows API 定义
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        // 添加新的常量
        private const int WM_XBUTTONDOWN = 0x020B;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn,
            IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        public delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
    }

    public class MousePositionEventArgs : EventArgs
    {
        public int X { get; }
        public int Y { get; }

        public MousePositionEventArgs(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // 新增事件参数类
    public class MouseSideButtonEventArgs : EventArgs
    {
        public int ButtonId { get; } // 1 表示 XBUTTON1, 2 表示 XBUTTON2

        public MouseSideButtonEventArgs(int buttonId)
        {
            ButtonId = buttonId;
        }
    }
}