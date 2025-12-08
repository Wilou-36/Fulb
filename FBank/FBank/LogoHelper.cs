using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace FBank
{
    internal static class LogoHelper
    {
        public static void AddLogoToForm(Form form, Point? location = null)
        {
            if (form == null) return;

            try
            {
                var logo = CreateFulbankLogo(220, 60);
                var logoPictureBox = new PictureBox
                {
                    Size = new Size(logo.Width, logo.Height),
                    Location = location ?? new Point(30, 30),
                    SizeMode = PictureBoxSizeMode.Normal,
                    BackColor = Color.Transparent,
                    Image = logo
                };

                var found = form.Controls.Find("sidePanel", true);
                if (found != null && found.Length > 0 && found[0] is Panel pnl)
                {
                    pnl.Controls.Add(logoPictureBox);
                    logoPictureBox.BringToFront();
                }
                else
                {
                    form.Controls.Add(logoPictureBox);
                    logoPictureBox.BringToFront();
                }
            }
            catch
            {
                // Ne pas casser l'UI si le logo échoue
            }
        }

        public static Bitmap CreateFulbankLogo(int width, int height)
        {
            const int radius = 12;
            var bmp = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.Clear(Color.Transparent);

                var rect = new Rectangle(0, 0, width, height);

                using (var path = new GraphicsPath())
                {
                    int r = radius;
                    path.AddArc(rect.X, rect.Y, r, r, 180, 90);
                    path.AddArc(rect.X + rect.Width - r, rect.Y, r, r, 270, 90);
                    path.AddArc(rect.X + rect.Width - r, rect.Y + rect.Height - r, r, r, 0, 90);
                    path.AddArc(rect.X, rect.Y + rect.Height - r, r, r, 90, 90);
                    path.CloseFigure();

                    using (var gradientBrush = new LinearGradientBrush(rect, Color.FromArgb(33, 150, 243), Color.FromArgb(21, 101, 192), 45f))
                    {
                        g.FillPath(gradientBrush, path);
                    }

                    using (var pen = new Pen(Color.FromArgb(20, 0, 0, 0), 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }

                string text = "Fulbank";
                float baseFontSize = Math.Min(height * 0.6f, 28f);
                string fontName = GetPreferredFontName("Rockwell", "Segoe UI");

                float chosenSize = 8f;
                for (float fs = baseFontSize; fs >= 8f; fs -= 0.5f)
                {
                    using (var testFont = new Font(fontName, fs, FontStyle.Bold, GraphicsUnit.Pixel))
                    {
                        var measured = g.MeasureString(text, testFont);
                        if (measured.Width <= width * 0.85f && measured.Height <= height * 0.85f)
                        {
                            chosenSize = fs;
                            break;
                        }
                    }
                }

                using (var textFont = new Font(fontName, chosenSize, FontStyle.Bold, GraphicsUnit.Pixel))
                using (var textBrush = new SolidBrush(Color.White))
                {
                    var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    g.DrawString(text, textFont, textBrush, new RectangleF(0, 0, width, height), sf);
                }
            }

            return bmp;
        }

        private static string GetPreferredFontName(string preferred, string fallback)
        {
            try
            {
                using (var ifc = new InstalledFontCollection())
                {
                    foreach (var f in ifc.Families)
                    {
                        if (string.Equals(f.Name, preferred, StringComparison.OrdinalIgnoreCase))
                            return preferred;
                    }
                }
            }
            catch
            {
                // ignore
            }

            return fallback;
        }
    }
}