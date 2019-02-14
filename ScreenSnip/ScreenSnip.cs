﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ScreenSnip {
    class ScreenSnip {
        private static Image screenshot;
        private static Bitmap final_image;
        private static string dir;

        public static void Snip(int x, int y, int w, int h) {
            takeScreenshot();

            Rectangle rect = new Rectangle(Screen.left * -1 + x, Screen.top * -1 + y, w, h);
            Bitmap originalImage = new Bitmap(screenshot, screenshot.Width, screenshot.Height);

            final_image = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(final_image);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.DrawImage(originalImage, 0, 0, rect, GraphicsUnit.Pixel);

            save();
        }

        private static void takeScreenshot() {
            Bitmap printscreen = new Bitmap(Screen.width, Screen.height);
            Graphics graphics = Graphics.FromImage(printscreen as Image);
            graphics.CopyFromScreen(Screen.left, Screen.top, 0, 0, printscreen.Size);

            using (MemoryStream s = new MemoryStream()) {
                printscreen.Save(s, ImageFormat.Bmp);
                screenshot = Image.FromStream(s);
            }
        }

        private static void save() {
            var path = getPath();
            final_image.Save(path, ImageFormat.Png);
            System.Diagnostics.Process.Start("explorer.exe", @dir);
        }

        private static string getPath() {
            dir = readPath();
            if(dir.Length == 0 || !Directory.Exists(dir)) dir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\Screenshots\\";
            Directory.CreateDirectory(dir);

            var name = DateTime.Now.ToString().Replace(".", "-").Replace(" ", "_").Replace(":", "-");
            var path = dir + "\\" + name + ".png";
            int i = 2;
            while (File.Exists(path)) {
                path = dir + "\\" + name + "_" + i + ".png";
                i++;
            }
            return path;
        }

        private static string readPath() {
            var path = AppDomain.CurrentDomain.BaseDirectory + "config.cfg";
            if (File.Exists(path)) {
                foreach (var line in File.ReadLines(path)) {
                    if (line.StartsWith("path")) {
                        var pair = line.Split('=');
                        if(pair.Length == 2) {
                            var value = pair[1];
                            if(value != "") {
                                if (!value.EndsWith("\\")) value += "\\";
                                return value;
                            }
                        }
                    }
                }
            }
            return "";
        }
    }
}
