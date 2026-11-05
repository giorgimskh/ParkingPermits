using System.Drawing.Drawing2D;

namespace ParkingPermits
{
    // The loading screen shown for about two seconds before the main window opens:
    // it fades in, a ring turns around the "P" logo while a bar fills up, then it fades out.
    public partial class SplashForm : Form
    {
        private const int FadeInTime = 250;      // milliseconds
        private const int LoadingStart = 300;
        private const int LoadingEnd = 1700;
        private const int FadeOutStart = 1850;
        private const int TotalTime = 2100;

        private readonly Font logoFont = new Font("Segoe UI", 34F, FontStyle.Bold);
        private readonly Font titleFont = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
        private readonly Font subtitleFont = new Font("Segoe UI", 10F);
        private readonly Font loadingFont = new Font("Segoe UI", 9F);

        private DateTime startTime;
        private int elapsed;   // milliseconds since the splash appeared

        public SplashForm()
        {
            InitializeComponent();
        }

        private void SplashForm_Load(object sender, EventArgs e)
        {
            // rounded corners for the borderless window
            using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(
                new Rectangle(0, 0, ClientSize.Width, ClientSize.Height), ScaleForScreen(16)))
            {
                Region = new Region(path);
            }

            startTime = DateTime.Now;
            tmrAnimation.Start();
        }

        // moves the animation on: fade in, fill the bar, fade out, then close
        private void tmrAnimation_Tick(object sender, EventArgs e)
        {
            elapsed = (int)(DateTime.Now - startTime).TotalMilliseconds;

            if (elapsed < FadeInTime)
                Opacity = elapsed / (double)FadeInTime;
            else if (elapsed < FadeOutStart)
                Opacity = 1;
            else if (elapsed < TotalTime)
                Opacity = 1 - (elapsed - FadeOutStart) / (double)(TotalTime - FadeOutStart);
            else
            {
                tmrAnimation.Stop();
                Close();
                return;
            }
            Invalidate();
        }

        // how full the loading bar is, from 0 to 1; it slows down near the end
        private float GetProgress()
        {
            float linear = (elapsed - LoadingStart) / (float)(LoadingEnd - LoadingStart);
            linear = Math.Max(0f, Math.Min(1f, linear));
            return 1f - (1f - linear) * (1f - linear);
        }

        private void SplashForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int width = ClientSize.Width;
            int height = ClientSize.Height;
            int centerX = width / 2;

            // background: ocean blue, deeper towards the bottom right, with two soft circles
            using (LinearGradientBrush background = new LinearGradientBrush(ClientRectangle,
                Palette.Header, Color.FromArgb(16, 52, 104), LinearGradientMode.ForwardDiagonal))
            {
                graphics.FillRectangle(background, ClientRectangle);
            }
            using (SolidBrush circleBrush = new SolidBrush(Color.FromArgb(14, Color.White)))
            {
                graphics.FillEllipse(circleBrush, width - ScaleForScreen(170), -ScaleForScreen(110), ScaleForScreen(300), ScaleForScreen(300));
                graphics.FillEllipse(circleBrush, -ScaleForScreen(120), height - ScaleForScreen(150), ScaleForScreen(280), ScaleForScreen(280));
            }

            // the logo with a turning ring around it
            int logoSize = ScaleForScreen(76);
            Point logoCenter = new Point(centerX, ScaleForScreen(96));
            int ringRadius = ScaleForScreen(56);
            Rectangle ring = new Rectangle(logoCenter.X - ringRadius, logoCenter.Y - ringRadius, ringRadius * 2, ringRadius * 2);
            using (Pen trackPen = new Pen(Color.FromArgb(45, Color.White), ScaleForScreen(3)))
            using (Pen arcPen = new Pen(Color.White, ScaleForScreen(3)))
            {
                arcPen.StartCap = LineCap.Round;
                arcPen.EndCap = LineCap.Round;
                graphics.DrawEllipse(trackPen, ring);
                graphics.DrawArc(arcPen, ring, elapsed * 0.36f - 90f, 100f);
            }

            Rectangle logo = new Rectangle(logoCenter.X - logoSize / 2, logoCenter.Y - logoSize / 2, logoSize, logoSize);
            using (GraphicsPath shadowPath = GraphicsHelper.CreateRoundedRectangle(
                new Rectangle(logo.X, logo.Y + ScaleForScreen(4), logo.Width, logo.Height), ScaleForScreen(14)))
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(50, 6, 24, 54)))
            {
                graphics.FillPath(shadowBrush, shadowPath);
            }
            GraphicsHelper.DrawParkingSign(graphics, logo, logoFont);

            // name and subtitle
            int y = logoCenter.Y + ringRadius + ScaleForScreen(18);
            TextFormatFlags centered = TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.NoPadding;
            TextRenderer.DrawText(graphics, "Parking Permits", titleFont,
                new Rectangle(0, y, width, titleFont.Height), Palette.HeaderText, centered);
            y += titleFont.Height + ScaleForScreen(2);
            TextRenderer.DrawText(graphics, "Campus parking permit register", subtitleFont,
                new Rectangle(0, y, width, subtitleFont.Height), Palette.HeaderMuted, centered);

            // loading bar and its caption
            int barWidth = ScaleForScreen(260);
            int barHeight = ScaleForScreen(5);
            Rectangle bar = new Rectangle(centerX - barWidth / 2, height - ScaleForScreen(56), barWidth, barHeight);
            using (GraphicsPath trackPath = GraphicsHelper.CreateRoundedRectangle(bar, barHeight / 2))
            using (SolidBrush trackBrush = new SolidBrush(Color.FromArgb(50, Color.White)))
            {
                graphics.FillPath(trackBrush, trackPath);
            }
            int fillWidth = (int)(barWidth * GetProgress());
            if (fillWidth >= barHeight)
            {
                using (GraphicsPath fillPath = GraphicsHelper.CreateRoundedRectangle(
                    new Rectangle(bar.X, bar.Y, fillWidth, barHeight), barHeight / 2))
                using (SolidBrush fillBrush = new SolidBrush(Color.White))
                {
                    graphics.FillPath(fillBrush, fillPath);
                }
            }

            string caption = elapsed < LoadingEnd ? "Loading…" : "Ready";
            TextRenderer.DrawText(graphics, caption, loadingFont,
                new Rectangle(0, bar.Bottom + ScaleForScreen(10), width, loadingFont.Height), Palette.HeaderMuted, centered);
        }

        // converts a size meant for a normal (100%) screen to the current screen scaling
        private int ScaleForScreen(int value)
        {
            return (int)Math.Round(value * DeviceDpi / 96f);
        }
    }
}
