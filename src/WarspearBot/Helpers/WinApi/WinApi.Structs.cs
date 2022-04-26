using System;
using System.Runtime.InteropServices;

namespace WarspearBot.Helpers.WinApi
{
    internal static partial class WinApi
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct Input
        {
            public SendInputType Type;
            public InputUnion Union;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            public MouseInput MouseInput;

            [FieldOffset(0)]
            public KeyboardInput KeyboardInput;

            [FieldOffset(0)]
            public HardwareInput Hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public int X;
            public int Y;
            public uint MouseData;
            public MouseEventFlags Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort VirtKeys;
            public ushort WScan;
            public KeyboardInputFlags Flags;
            public uint Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HardwareInput
        {
            public int Message;
            public short LParam;
            public short WParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;
        }
    }
}