using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenSnip {
    class OverlayForm : Form {
        public OverlayForm() {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }
    }

    class Overlay {
        private static Form form;
        private static Panel box;
        private static Label info;
        private static Timer timer;

        private static Point mousePos;
        private static bool selecting = false;
        private static Screen.Selection selection;

        public static void Init() {
            form = new OverlayForm();

            MouseMessageFilter mouseFilter = new MouseMessageFilter();
            mouseFilter.TargetForm = form;
            Application.AddMessageFilter(mouseFilter);

            form.FormBorderStyle = FormBorderStyle.None;
            form.TransparencyKey = Color.Turquoise;
            form.BackColor = Color.Turquoise;

            form.StartPosition = FormStartPosition.Manual;
            form.Location = new Point(Screen.left, Screen.top);
            form.Size = new Size(Screen.width, Screen.height);

            box = new Panel();
            box.Visible = false;
            box.BorderStyle = BorderStyle.FixedSingle;

            info = new Label();
            box.Controls.Add(info);
            form.Controls.Add(box);

            timer = new Timer();
            timer.Interval = 12;
            timer.Tick += timerTick;
            timer.Start();

            form.KeyDown += overlayKeyDown;
            info.Paint += infoDraw;
            Application.Run(form);
        }

        private static void timerTick(object sender, EventArgs e) {
            if(mousePos.X != Control.MousePosition.X || mousePos.Y != Control.MousePosition.Y) {
                mousePos.X = Control.MousePosition.X;
                mousePos.Y = Control.MousePosition.Y;
                if (selecting) {
                    selection.p2 = new Point(Control.MousePosition.X, Control.MousePosition.Y);
                    selection.Update();
                    box.Visible = false;
                    box.Left = Screen.left * -1 + selection.x;
                    box.Top = Screen.top * -1 + selection.y;
                    box.Width = selection.w + 1;
                    box.Height = selection.h + 1;
                    box.Visible = true;
                    if (selection.p2.X < selection.p1.X) info.Top = selection.h - 17;
                    else info.Top = 1;
                }
                info.Invalidate();
            }
        }

        private static void infoDraw(object sender, PaintEventArgs e) {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            e.Graphics.DrawString(selection.w + " x " + selection.h, info.Font, SystemBrushes.GrayText, new Point(0, 0));
        }

        class MouseMessageFilter : IMessageFilter {
            public Form TargetForm { get; set; }

            public bool PreFilterMessage(ref Message m) {
                if (m.Msg == 513 || m.Msg == 162) { // left click
                    if (!selecting) {
                        selecting = true;
                        selection.p1 = new Point(Control.MousePosition.X, Control.MousePosition.Y);
                    } else {
                        selection.p2 = new Point(Control.MousePosition.X, Control.MousePosition.Y);
                        selection.Update();
                        form.Visible = false;
                        if(selection.w > 0 && selection.h > 0) ScreenSnip.Snip(selection.x, selection.y, selection.w, selection.h);
                        Application.Exit();
                    }
                } else if (m.Msg == 517 || m.Msg == 165) Application.Exit(); // right click

                return false;
            }
        }

        private static void overlayKeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) Application.Exit();
        }
    }
}
