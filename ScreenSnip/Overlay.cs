using System;
using System.Drawing;
using System.Windows.Forms;

namespace ScreenSnip {
    class Overlay {
        private static Form form;
        private static Panel box;
        private static Label info;
        
        private static bool selecting = false;
        private static Screen.Selection selection;

        public static void Init() {
            form = new Form();

            MouseMoveMessageFilter mouseMoveFilter = new MouseMoveMessageFilter();
            mouseMoveFilter.TargetForm = form;
            Application.AddMessageFilter(mouseMoveFilter);

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

            form.MouseClick += overlayMouseClick;
            form.KeyDown += overlayKeyDown;
            info.Paint += infoDraw;

            Application.Run(form);
        }

        private static void infoDraw(object sender, PaintEventArgs e) {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            e.Graphics.DrawString(selection.w + " x " + selection.h, info.Font, SystemBrushes.GrayText, new Point(0, 0));
        }

        class MouseMoveMessageFilter : IMessageFilter {
            public Form TargetForm { get; set; }
            public bool PreFilterMessage(ref Message m) {
                if (m.Msg == 512 || m.Msg == 160) {
                    if (selecting) {
                        selection.p2 = new Point(Control.MousePosition.X, Control.MousePosition.Y);
                        selection.Update();
                        box.Left = Screen.left * -1 + selection.x;
                        box.Top = Screen.top * -1 + selection.y;
                        box.Size = new Size(selection.w, selection.h);
                    }
                    info.Invalidate();
                }
                return false;
            }
        }

        private static void overlayKeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) Application.Exit();
        }

        private static void overlayMouseClick(object sender, MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                if (!selecting) {
                    selecting = true;
                    box.Visible = true;
                    selection.p1 = new Point(Control.MousePosition.X, Control.MousePosition.Y);
                } else {
                    selection.p2 = new Point(Control.MousePosition.X, Control.MousePosition.Y);
                    selection.Update();
                    box.Visible = false;
                    ScreenSnip.Snip(selection.x, selection.y, selection.w, selection.h);
                    Application.Exit();
                }
            } else Application.Exit();
        }
    }
}
