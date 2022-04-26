namespace WarspearBot.Helpers.WinApi
{
    internal static partial class WinApi
    {
        public static readonly float AbsoluteModX = 65535.0f / GetSystemMetrics(0);
        public static readonly float AbsoluteModY = 65535.0f / GetSystemMetrics(1);
    }
}