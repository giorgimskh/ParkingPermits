namespace ParkingPermits
{
    // A Panel that is double-buffered and repaints when resized,
    // so hand-drawn content does not flicker or smear
    public class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
        }
    }

    // A card with rounded corners and a thin border; controls are placed on top of it.
    // It is white unless the form gives it other colours with SetCardColors.
    public class CardPanel : BufferedPanel
    {
        private const int CornerRadius = 12;
        private Color fillColor = Palette.Card;
        private Color borderColor = Palette.Border;

        public void SetCardColors(Color fill, Color border)
        {
            fillColor = fill;
            borderColor = border;
            BackColor = fill;   // buttons on the card clear their corners with this colour
            Invalidate();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // the corners outside the rounded card show the window's background colour
            Color outsideColor = Parent != null ? Parent.BackColor : Palette.Background;
            e.Graphics.Clear(outsideColor);
            GraphicsHelper.DrawRoundedCard(e.Graphics, new Rectangle(0, 0, Width - 1, Height - 1), CornerRadius,
                fillColor, borderColor);
        }
    }
}
