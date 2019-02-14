using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenSnip {
    public struct Screen {
        public static int left = SystemInformation.VirtualScreen.Left;
        public static int top = SystemInformation.VirtualScreen.Top;
        public static int width = SystemInformation.VirtualScreen.Width, height = SystemInformation.VirtualScreen.Height;
        public static int x = SystemInformation.VirtualScreen.X, y = SystemInformation.VirtualScreen.Y;

        public struct Selection {
            public Point p1, p2;
            public int x, y, w, h;

            public void Update() {
                x = Math.Min(p1.X, p2.X);
                y = Math.Min(p1.Y, p2.Y);
                w = Math.Max(p1.X, p2.X) - x;
                h = Math.Max(p1.Y, p2.Y) - y;
            }
        }
    }
}