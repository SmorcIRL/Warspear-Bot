using System;

namespace WarspearBot.Helpers.WinApi
{
    internal static partial class WinApi
    {
        [Flags]
        public enum KeyboardInputFlags : uint
        {
            KEY_DOWN = 0x0,
            EXTENDED_KEY = 0x0001,
            KEY_UP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004,
        }

        [Flags]
        public enum MouseEventFlags : uint
        {
            MOUSEEVENTF_MOVE = 0x0001,
            MOUSEEVENTF_LEFTDOWN = 0x0002,
            MOUSEEVENTF_LEFTUP = 0x0004,
            MOUSEEVENTF_RIGHTDOWN = 0x0008,
            MOUSEEVENTF_RIGHTUP = 0x0010,
            MOUSEEVENTF_MIDDLEDOWN = 0x0020,
            MOUSEEVENTF_MIDDLEUP = 0x0040,
            MOUSEEVENTF_XDOWN = 0x0080,
            MOUSEEVENTF_XUP = 0x0100,
            MOUSEEVENTF_WHEEL = 0x0800,
            MOUSEEVENTF_VIRTUALDESK = 0x4000,
            MOUSEEVENTF_ABSOLUTE = 0x8000,
        }

        [Flags]
        public enum WindowPositionFlags
        {
            NO_SIZE = 0x1,
            NO_MOVE = 0x2,
            NO_Z_ORDER = 0x4,
            SHOW_WINDOW = 0x0040,
            MOVE = NO_SIZE | NO_Z_ORDER,
            RESIZE = NO_MOVE | NO_Z_ORDER,
            PIN = NO_MOVE | NO_SIZE | SHOW_WINDOW,
        }

        public enum SendInputType : uint
        {
            MOUSE = 0,
            KEYBOARD = 1,
            HARDWARE = 2,
        }
    }
}