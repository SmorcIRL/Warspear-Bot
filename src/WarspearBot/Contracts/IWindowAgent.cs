using System;
using System.Drawing;
using WarspearBot.Models;

namespace WarspearBot.Contracts
{
    public interface IWindowAgent
    {
        IntPtr GetWindowHandleByName(string name);

        (int X, int Y) GetWindowLocation(IntPtr handle);

        (int W, int H) GetWindowSize(IntPtr handle);

        void MoveWindow(IntPtr handle, (int X, int Y) location);

        void PinToTop(IntPtr handle);

        Bitmap MakeScreenshot(Bitmap bitmap, (int X, int Y) leftTop);

        void MouseClick((int X, int Y) location, MouseClick mouseClick);
    }
}