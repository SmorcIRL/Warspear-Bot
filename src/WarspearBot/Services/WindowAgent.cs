using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using WarspearBot.Contracts;
using WarspearBot.Helpers.WinApi;
using WarspearBot.Models;

namespace WarspearBot.Services
{
    public partial class WindowAgent : IWindowAgent
    {
        public IntPtr GetWindowHandleByName(string name)
        {
            var processes = Process.GetProcessesByName(name);

            if (processes.Length == 0)
            {
                throw new Exception($"No \"{name}\" window found");
            }

            var process = processes.FirstOrDefault(x => x.MainWindowHandle != IntPtr.Zero);

            if (process == null)
            {
                throw new Exception("Found process does not have a window");
            }

            return process.MainWindowHandle;
        }

        public (int X, int Y) GetWindowLocation(IntPtr handle)
        {
            var rect = new WinApi.Rect();

            if (!WinApi.GetWindowRect(handle, ref rect))
            {
                ThrowLastError();
            }

            return (rect.Left, rect.Top);
        }

        public (int W, int H) GetWindowSize(IntPtr handle)
        {
            var rect = new WinApi.Rect();

            if (!WinApi.GetWindowRect(handle, ref rect))
            {
                ThrowLastError();
            }

            return (rect.Right - rect.Left, rect.Bottom - rect.Top);
        }

        public void MoveWindow(IntPtr handle, (int X, int Y) location)
        {
            if (!WinApi.SetWindowPos(handle, 0, location.X, location.Y, 0, 0, (int)WinApi.WindowPositionFlags.MOVE))
            {
                ThrowLastError();
            }
        }

        public void PinToTop(IntPtr handle)
        {
            if (!WinApi.SetWindowPos(handle, -1, 0, 0, 0, 0, (int)WinApi.WindowPositionFlags.PIN))
            {
                ThrowLastError();
            }
        }

        public Bitmap MakeScreenshot(Bitmap bitmap, (int X, int Y) leftTop)
        {
            using var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(new Point(leftTop.X, leftTop.Y), Point.Empty, bitmap.Size);

            return bitmap;
        }

        public void MouseClick((int X, int Y) location, MouseClick mouseClick)
        {
            WinApi.GetCursorPos(out var point);

            var inputs = new WinApi.Input[3];

            var x = CalculateAbsoluteX(location.X);
            var y = CalculateAbsoluteY(location.Y);

            const WinApi.MouseEventFlags absoluteMove = WinApi.MouseEventFlags.MOUSEEVENTF_MOVE | WinApi.MouseEventFlags.MOUSEEVENTF_ABSOLUTE;

            inputs[0] = new WinApi.Input();
            inputs[0].Type = WinApi.SendInputType.MOUSE;
            inputs[0].Union = new WinApi.InputUnion();
            inputs[0].Union.MouseInput.X = x;
            inputs[0].Union.MouseInput.Y = y;
            inputs[0].Union.MouseInput.Flags = absoluteMove | (mouseClick == Models.MouseClick.Left ? WinApi.MouseEventFlags.MOUSEEVENTF_LEFTDOWN : WinApi.MouseEventFlags.MOUSEEVENTF_RIGHTDOWN);

            inputs[1] = new WinApi.Input();
            inputs[1].Type = WinApi.SendInputType.MOUSE;
            inputs[1].Union = new WinApi.InputUnion();
            inputs[1].Union.MouseInput.X = x;
            inputs[1].Union.MouseInput.Y = y;
            inputs[1].Union.MouseInput.Flags = absoluteMove | (mouseClick == Models.MouseClick.Left ? WinApi.MouseEventFlags.MOUSEEVENTF_LEFTUP : WinApi.MouseEventFlags.MOUSEEVENTF_RIGHTUP);

            inputs[2] = new WinApi.Input();
            inputs[2].Type = WinApi.SendInputType.MOUSE;
            inputs[2].Union = new WinApi.InputUnion();
            inputs[2].Union.MouseInput.X = CalculateAbsoluteX(point.X);
            inputs[2].Union.MouseInput.Y = CalculateAbsoluteY(point.Y);
            inputs[2].Union.MouseInput.Flags = absoluteMove;

            if (WinApi.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(inputs[0])) == 0)
            {
                ThrowLastError();
            }
        }
    }

    public partial class WindowAgent
    {
        private static void ThrowLastError()
        {
            var error = Marshal.GetLastWin32Error();
            throw new Win32Exception(error);
        }

        private static int CalculateAbsoluteX(int x)
        {
            return (int)Math.Round(x * WinApi.AbsoluteModX, MidpointRounding.AwayFromZero);
        }

        private static int CalculateAbsoluteY(int y)
        {
            return (int)Math.Round(y * WinApi.AbsoluteModY, MidpointRounding.AwayFromZero);
        }
    }
}