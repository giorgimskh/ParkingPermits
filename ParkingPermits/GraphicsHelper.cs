using System.Drawing.Drawing2D;

namespace ParkingPermits
{
    // The colours used by all windows, in one place.
    // "Ocean blue": a blue top bar, a soft blue-grey background, white cards,
    // navy text, and clear status colours (green, amber, orange, rose).
    public static class Palette
    {
        public static readonly Color Header = Color.FromArgb(30, 90, 168);        // top bar
        public static readonly Color HeaderText = Color.White;
        public static readonly Color HeaderMuted = Color.FromArgb(200, 218, 240);  // inactive tabs, subtitle

        public static readonly Color Background = Color.FromArgb(227, 235, 245);   // soft blue-grey
        public static readonly Color Card = Color.White;
        public static readonly Color Border = Color.FromArgb(210, 221, 235);
        public static readonly Color FieldBorder = Color.FromArgb(190, 204, 222);
        public static readonly Color Text = Color.FromArgb(27, 42, 65);           // navy
        public static readonly Color Muted = Color.FromArgb(91, 107, 130);
        public static readonly Color Faint = Color.FromArgb(150, 163, 182);
        public static readonly Color Accent = Color.FromArgb(47, 111, 214);       // ocean blue
        public static readonly Color AccentSoft = Color.FromArgb(211, 227, 250);
        public static readonly Color PlateBlue = Color.FromArgb(29, 78, 216);     // the real blue of a Georgian plate

        // one clear colour per status (stripes, car colours, chart parts)
        public static readonly Color Green = Color.FromArgb(34, 163, 95);
        public static readonly Color Yellow = Color.FromArgb(224, 161, 26);
        public static readonly Color Orange = Color.FromArgb(238, 122, 52);
        public static readonly Color Red = Color.FromArgb(222, 77, 98);

        // light background + dark text of the same colour, for the status pills
        public static readonly Color GreenSoft = Color.FromArgb(220, 245, 230);
        public static readonly Color GreenText = Color.FromArgb(23, 112, 63);
        public static readonly Color YellowSoft = Color.FromArgb(253, 241, 201);
        public static readonly Color YellowText = Color.FromArgb(138, 90, 6);
        public static readonly Color OrangeSoft = Color.FromArgb(255, 231, 214);
        public static readonly Color OrangeText = Color.FromArgb(168, 70, 15);
        public static readonly Color RedSoft = Color.FromArgb(253, 225, 230);
        public static readonly Color RedText = Color.FromArgb(163, 32, 62);

        // row backgrounds for each status, and a deeper shade for when the row is selected
        public static readonly Color RowExpired = Color.FromArgb(253, 225, 230);
        public static readonly Color RowExpiredSelected = Color.FromArgb(250, 200, 210);
        public static readonly Color RowAwaiting = Color.FromArgb(255, 231, 214);
        public static readonly Color RowAwaitingSelected = Color.FromArgb(253, 210, 178);
        public static readonly Color RowExpiresSoon = Color.FromArgb(253, 241, 201);
        public static readonly Color RowExpiresSoonSelected = Color.FromArgb(251, 229, 150);
        public static readonly Color RowValidSelected = Color.FromArgb(211, 227, 250);
    }

    // The colours of the "Permit details" panel only.
    // It looks like a parking pass: a teal band on top with the plate and owner,
    // and a soft mint body, so it stands apart from the blue list next to it.
    public static class DetailsPalette
    {
        public static readonly Color BandTop = Color.FromArgb(17, 128, 120);      // teal
        public static readonly Color BandBottom = Color.FromArgb(12, 99, 94);     // deeper teal
        public static readonly Color BandText = Color.White;
        public static readonly Color BandMuted = Color.FromArgb(196, 236, 230);

        public static readonly Color Body = Color.FromArgb(240, 250, 248);        // soft mint
        public static readonly Color Border = Color.FromArgb(186, 224, 217);
        public static readonly Color Divider = Color.FromArgb(206, 233, 227);
        public static readonly Color Text = Color.FromArgb(18, 56, 62);           // deep teal-navy
        public static readonly Color Muted = Color.FromArgb(78, 118, 116);
        public static readonly Color RingTrack = Color.FromArgb(206, 233, 227);
    }

    // the small line icons drawn on the summary cards and in the search box
    public enum LineIcon
    {
        List,
        Check,
        Clock,
        Card,
        Cross,
        Search
    }

    // Small drawing methods shared by the forms
    public static class GraphicsHelper
    {
        // a rectangle with rounded corners, as a path that can be filled or drawn
        public static GraphicsPath CreateRoundedRectangle(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        // a white card with rounded corners and a thin border
        public static void DrawRoundedCard(Graphics graphics, Rectangle bounds, int radius)
        {
            DrawRoundedCard(graphics, bounds, radius, Palette.Card, Palette.Border);
        }

        // the same card in other colours
        public static void DrawRoundedCard(Graphics graphics, Rectangle bounds, int radius, Color fillColor, Color borderColor)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRectangle(bounds, radius))
            using (SolidBrush cardBrush = new SolidBrush(fillColor))
            using (Pen borderPen = new Pen(borderColor))
            {
                graphics.FillPath(cardBrush, path);
                graphics.DrawPath(borderPen, path);
            }
        }

        // a white input field with rounded corners (the control sits on top of it)
        public static void DrawField(Graphics graphics, Rectangle bounds)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRectangle(bounds, 8))
            using (SolidBrush fillBrush = new SolidBrush(Color.White))
            using (Pen borderPen = new Pen(Palette.FieldBorder))
            {
                graphics.FillPath(fillBrush, path);
                graphics.DrawPath(borderPen, path);
            }
        }

        // a 1-pixel frame just outside a control, used around the dialog's ComboBoxes
        public static void DrawControlBorder(Graphics graphics, Control control)
        {
            using (Pen borderPen = new Pen(Palette.FieldBorder))
            {
                graphics.DrawRectangle(borderPen, control.Left - 1, control.Top - 1, control.Width + 1, control.Height + 1);
            }
        }

        // a simple line icon, drawn in an 18 x 18 box
        public static void DrawIcon(Graphics graphics, LineIcon icon, Rectangle box, Color color)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int x = box.X;
            int y = box.Y;
            using (Pen pen = new Pen(color, 2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                switch (icon)
                {
                    case LineIcon.List:
                        graphics.DrawLine(pen, x + 2, y + 4, x + 16, y + 4);
                        graphics.DrawLine(pen, x + 2, y + 9, x + 16, y + 9);
                        graphics.DrawLine(pen, x + 2, y + 14, x + 11, y + 14);
                        break;
                    case LineIcon.Check:
                        graphics.DrawLines(pen, new[] { new Point(x + 2, y + 9), new Point(x + 7, y + 14), new Point(x + 16, y + 4) });
                        break;
                    case LineIcon.Clock:
                        graphics.DrawEllipse(pen, x + 1, y + 1, 16, 16);
                        graphics.DrawLines(pen, new[] { new Point(x + 9, y + 5), new Point(x + 9, y + 9), new Point(x + 12, y + 11) });
                        break;
                    case LineIcon.Card:
                        graphics.DrawRectangle(pen, x + 1, y + 3, 16, 12);
                        graphics.DrawLine(pen, x + 1, y + 7, x + 17, y + 7);
                        break;
                    case LineIcon.Cross:
                        graphics.DrawLine(pen, x + 4, y + 4, x + 14, y + 14);
                        graphics.DrawLine(pen, x + 14, y + 4, x + 4, y + 14);
                        break;
                    case LineIcon.Search:
                        graphics.DrawEllipse(pen, x + 1, y + 1, 11, 11);
                        graphics.DrawLine(pen, x + 11, y + 11, x + 16, y + 16);
                        break;
                }
            }
        }

        // a rounded "pill" with a word in it, for example a status inside a grid cell
        public static void DrawBadge(Graphics graphics, Rectangle cell, string text, Font font, Color backColor, Color textColor)
        {
            DrawBadge(graphics, cell, text, font, backColor, textColor, Color.Empty);
        }

        // the same badge with a thin outline (borderColor), so it stands out on a coloured row
        public static void DrawBadge(Graphics graphics, Rectangle cell, string text, Font font, Color backColor, Color textColor, Color borderColor)
        {
            Size textSize = TextRenderer.MeasureText(graphics, text, font, Size.Empty, TextFormatFlags.NoPadding);
            int width = Math.Min(textSize.Width + 20, cell.Width - 16);
            int height = Math.Max(22, textSize.Height + 8);
            Rectangle badge = new Rectangle(cell.X + 8, cell.Y + (cell.Height - height) / 2, width, height);

            SmoothingMode oldMode = graphics.SmoothingMode;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRectangle(badge, height / 2))
            using (SolidBrush brush = new SolidBrush(backColor))
            {
                graphics.FillPath(brush, path);
                if (!borderColor.IsEmpty)
                {
                    using (Pen borderPen = new Pen(borderColor))
                    {
                        graphics.DrawPath(borderPen, path);
                    }
                }
            }
            graphics.SmoothingMode = oldMode;

            TextRenderer.DrawText(graphics, text, font, badge, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        // a car number plate: white, black edge, blue "GE" strip on the left
        public static void DrawLicensePlate(Graphics graphics, Rectangle bounds, string plateNumber, Font font, bool faded)
        {
            Color ink = faded ? Palette.Faint : Palette.Text;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (GraphicsPath path = CreateRoundedRectangle(bounds, 7))
            using (SolidBrush plateBrush = new SolidBrush(Color.White))
            using (SolidBrush stripBrush = new SolidBrush(faded ? Palette.Faint : Palette.PlateBlue))
            using (Pen edgePen = new Pen(ink, 2f))
            using (Font countryFont = new Font("Segoe UI", 7.5F, FontStyle.Bold))
            {
                graphics.FillPath(plateBrush, path);

                int stripWidth = Math.Max(22, bounds.Height / 2);
                graphics.SetClip(path);
                graphics.FillRectangle(stripBrush, bounds.X, bounds.Y, stripWidth, bounds.Height);
                graphics.ResetClip();

                graphics.DrawPath(edgePen, path);

                Rectangle countryArea = new Rectangle(bounds.X, bounds.Bottom - bounds.Height / 2, stripWidth, bounds.Height / 2);
                TextRenderer.DrawText(graphics, "GE", countryFont, countryArea, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

                Rectangle numberArea = new Rectangle(bounds.X + stripWidth, bounds.Y, bounds.Width - stripWidth, bounds.Height);
                TextRenderer.DrawText(graphics, plateNumber, font, numberArea, ink,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }

        // a circle with a coloured arc; fraction 0..1 is how much of the circle is coloured
        public static void DrawProgressRing(Graphics graphics, Point center, int radius, int thickness, float fraction, Color color)
        {
            DrawProgressRing(graphics, center, radius, thickness, fraction, color, Palette.Border);
        }

        // the same ring with a chosen colour for the empty part
        public static void DrawProgressRing(Graphics graphics, Point center, int radius, int thickness, float fraction, Color color, Color trackColor)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle circle = new Rectangle(center.X - radius, center.Y - radius, radius * 2, radius * 2);

            using (Pen trackPen = new Pen(trackColor, thickness))
            {
                graphics.DrawEllipse(trackPen, circle);
            }
            if (fraction > 0)
            {
                using (Pen arcPen = new Pen(color, thickness))
                {
                    arcPen.StartCap = LineCap.Round;
                    arcPen.EndCap = LineCap.Round;
                    graphics.DrawArc(arcPen, circle, -90, 360f * fraction);
                }
            }
        }

        // A car seen from above, lying left-right inside "body".
        // facingRight says which end is the front (windscreen and headlights).
        public static void DrawCar(Graphics graphics, Rectangle body, Color color, bool facingRight)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            float length = body.Width;
            float width = body.Height;

            // a part of the car, given as fractions: along the car from the back (0) to the front (1),
            // and across the car from one side (0) to the other (1)
            RectangleF Part(float from, float to, float top, float bottom)
            {
                float x = facingRight ? body.X + from * length : body.Right - to * length;
                return new RectangleF(x, body.Y + top * width, (to - from) * length, (bottom - top) * width);
            }

            Color darker = Color.FromArgb(color.R * 7 / 10, color.G * 7 / 10, color.B * 7 / 10);

            using (SolidBrush tyreBrush = new SolidBrush(Color.FromArgb(17, 24, 39)))
            using (SolidBrush bodyBrush = new SolidBrush(color))
            using (Pen outlinePen = new Pen(darker, 1.5f))
            using (SolidBrush glassBrush = new SolidBrush(Color.FromArgb(220, 30, 41, 59)))
            using (SolidBrush roofBrush = new SolidBrush(Color.FromArgb(45, 255, 255, 255)))
            using (SolidBrush lightBrush = new SolidBrush(Color.FromArgb(254, 240, 138)))
            using (SolidBrush rearLightBrush = new SolidBrush(Color.FromArgb(248, 113, 113)))
            {
                // tyres stick out a little on both sides
                foreach (float position in new[] { 0.16f, 0.70f })
                {
                    graphics.FillRectangle(tyreBrush, Part(position, position + 0.15f, -0.08f, 0.10f));
                    graphics.FillRectangle(tyreBrush, Part(position, position + 0.15f, 0.90f, 1.08f));
                }

                // body
                using (GraphicsPath bodyPath = CreateRoundedRectangle(body, Math.Max(3, body.Height / 3)))
                {
                    graphics.FillPath(bodyBrush, bodyPath);
                    graphics.DrawPath(outlinePen, bodyPath);
                }

                // rear window, roof and windscreen
                graphics.FillRectangle(glassBrush, Part(0.12f, 0.24f, 0.16f, 0.84f));
                graphics.FillRectangle(roofBrush, Part(0.26f, 0.60f, 0.18f, 0.82f));
                graphics.FillRectangle(glassBrush, Part(0.61f, 0.77f, 0.12f, 0.88f));

                // headlights at the front, tail lights at the back
                graphics.FillRectangle(lightBrush, Part(0.95f, 0.99f, 0.10f, 0.26f));
                graphics.FillRectangle(lightBrush, Part(0.95f, 0.99f, 0.74f, 0.90f));
                graphics.FillRectangle(rearLightBrush, Part(0.01f, 0.04f, 0.10f, 0.24f));
                graphics.FillRectangle(rearLightBrush, Part(0.01f, 0.04f, 0.76f, 0.90f));
            }
        }

        // the "P" parking sign used in the window headers: a white square with a blue P,
        // so it stands out on the blue top bar
        public static void DrawParkingSign(Graphics graphics, Rectangle bounds, Font font)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = CreateRoundedRectangle(bounds, 8))
            using (SolidBrush brush = new SolidBrush(Color.White))
            {
                graphics.FillPath(brush, path);
            }
            TextRenderer.DrawText(graphics, "P", font, bounds, Palette.Header,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }
}
