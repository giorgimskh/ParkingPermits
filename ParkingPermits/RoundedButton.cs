using System.Drawing.Drawing2D;

namespace ParkingPermits
{
    // A Button with rounded corners and a soft colour fade when the mouse moves over it.
    // It uses the normal Button properties for its colours:
    //   BackColor                        - normal colour
    //   FlatAppearance.MouseOverBackColor - colour when the mouse is over it
    //   FlatAppearance.BorderColor       - outline
    //   ForeColor                        - text
    public class RoundedButton : Button
    {
        private const int CornerRadius = 8;

        private readonly System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer();
        private float hoverAmount;   // 0 = normal colour, 1 = full hover colour
        private bool isHovered;
        private bool isPressed;

        public RoundedButton()
        {
            // we paint the whole button ourselves, double-buffered so it does not flicker
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            FlatStyle = FlatStyle.Flat;
            Cursor = Cursors.Hand;

            fadeTimer.Interval = 15;
            fadeTimer.Tick += FadeTimer_Tick;
        }

        // moves hoverAmount a little towards 0 or 1 on every tick, so the colour changes smoothly
        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            float target = isHovered ? 1f : 0f;
            float step = 0.15f;

            if (Math.Abs(target - hoverAmount) <= step)
            {
                hoverAmount = target;
                fadeTimer.Stop();
            }
            else if (hoverAmount < target)
            {
                hoverAmount += step;
            }
            else
            {
                hoverAmount -= step;
            }
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            fadeTimer.Start();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            fadeTimer.Start();
        }

        protected override void OnMouseDown(MouseEventArgs mevent)
        {
            base.OnMouseDown(mevent);
            if (mevent.Button == MouseButtons.Left)
            {
                isPressed = true;
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            base.OnMouseUp(mevent);
            isPressed = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics graphics = pevent.Graphics;
            Color parentColor = Parent != null ? Parent.BackColor : SystemColors.Control;
            graphics.Clear(parentColor);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Color hoverColor = FlatAppearance.MouseOverBackColor.IsEmpty ? BackColor : FlatAppearance.MouseOverBackColor;
            Color fillColor = Blend(BackColor, hoverColor, hoverAmount);
            if (isPressed)
                fillColor = Blend(fillColor, Color.Black, 0.12f);
            if (!Enabled)
                fillColor = Blend(fillColor, parentColor, 0.5f);

            Rectangle bounds = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(bounds, CornerRadius))
            using (SolidBrush fillBrush = new SolidBrush(fillColor))
            {
                graphics.FillPath(fillBrush, path);

                if (!FlatAppearance.BorderColor.IsEmpty)
                {
                    using (Pen borderPen = new Pen(FlatAppearance.BorderColor))
                    {
                        graphics.DrawPath(borderPen, path);
                    }
                }
            }

            // a blue ring when the button is reached with the Tab key
            if (Focused && ShowFocusCues)
            {
                Rectangle ring = new Rectangle(2, 2, Width - 5, Height - 5);
                using (GraphicsPath ringPath = GraphicsHelper.CreateRoundedRectangle(ring, CornerRadius - 2))
                using (Pen ringPen = new Pen(Color.FromArgb(147, 197, 253), 2f))
                {
                    graphics.DrawPath(ringPen, ringPath);
                }
            }

            // the text moves down 1 pixel while the button is pressed
            Rectangle textBounds = ClientRectangle;
            if (isPressed)
                textBounds.Offset(0, 1);
            Color textColor = Enabled ? ForeColor : Blend(ForeColor, parentColor, 0.5f);
            TextRenderer.DrawText(graphics, Text, Font, textBounds, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        // a colour part of the way from one colour to another (amount 0..1)
        private static Color Blend(Color from, Color to, float amount)
        {
            int red = (int)(from.R + (to.R - from.R) * amount);
            int green = (int)(from.G + (to.G - from.G) * amount);
            int blue = (int)(from.B + (to.B - from.B) * amount);
            return Color.FromArgb(red, green, blue);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                fadeTimer.Dispose();
            base.Dispose(disposing);
        }
    }
}
