using System.Drawing.Drawing2D;

namespace ParkingPermits
{
    // The third part of MainForm: the "Parking map" page.
    // Every zone is drawn as a small car park with ParkingLot.SpotsPerZone spots:
    // a car for a paid permit, a dashed "reserved" box for one awaiting payment, or a free spot.
    public partial class MainForm
    {
        // every spot drawn on the last paint, and the permit in it (-1 = free), so the mouse can find them
        private readonly List<Rectangle> spotBounds = new List<Rectangle>();
        private readonly List<Permit> spotPermits = new List<Permit>();
        private int hoveredSpot = -1;

        private readonly Font zoneLetterFont = new Font("Segoe UI", 26F, FontStyle.Bold);
        private readonly Font spotLabelFont = new Font("Segoe UI Semibold", 7.5F, FontStyle.Bold);

        // a light car park: pale grey ground, white spot lines, amber lane line
        private static readonly Color Asphalt = Color.FromArgb(214, 224, 237);
        private static readonly Color SpotLine = Color.White;
        private static readonly Color LaneLine = Color.FromArgb(224, 161, 26);

        // the permits that hold a spot in a zone, in a fixed order (by plate) so cars do not jump around
        private List<Permit> GetPermitsInZone(string zone)
        {
            List<Permit> permits = new List<Permit>();
            foreach (Permit permit in allPermits)
            {
                if (permit.Zone == zone && permit.TakesSpot)
                    permits.Add(permit);
            }
            permits.Sort((first, second) => string.Compare(first.PlateNumber, second.PlateNumber, StringComparison.Ordinal));
            return permits;
        }

        private void pnlMapPage_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            int margin = ScaleForScreen(24);
            int gap = ScaleForScreen(16);
            spotBounds.Clear();
            spotPermits.Clear();

            // page title, summary and colour key
            int takenTotal = 0;
            foreach (string zone in ParkingLot.Zones)
                takenTotal += Math.Min(GetPermitsInZone(zone).Count, ParkingLot.SpotsPerZone);
            int capacity = ParkingLot.SpotsPerZone * ParkingLot.Zones.Length;

            TextRenderer.DrawText(graphics, "Parking map", cardTitleFont, new Point(margin, margin), Palette.Text);
            int subtitleTop = margin + cardTitleFont.Height + ScaleForScreen(2);
            TextRenderer.DrawText(graphics,
                ParkingLot.SpotsPerZone + " spots in each zone  ·  " + takenTotal + " of " + capacity + " spots taken  ·  " +
                (capacity - takenTotal) + " free", detailFont, new Point(margin, subtitleTop), Palette.Muted);
            DrawMapLegend(graphics, pnlMapPage.Width - margin, margin + ScaleForScreen(8));

            // one card per zone, side by side
            int top = subtitleTop + detailFont.Height + ScaleForScreen(18);
            int height = pnlMapPage.Height - top - margin;
            int zoneCount = ParkingLot.Zones.Length;
            int cardWidth = (pnlMapPage.Width - margin * 2 - gap * (zoneCount - 1)) / zoneCount;

            for (int i = 0; i < zoneCount; i++)
            {
                Rectangle card = new Rectangle(margin + i * (cardWidth + gap), top, cardWidth, height);
                DrawZoneCard(graphics, card, ParkingLot.Zones[i]);
            }
        }

        // colour key in the top right corner, drawn from right to left
        private void DrawMapLegend(Graphics graphics, int right, int y)
        {
            string[] names = { "Free", "Reserved (awaiting payment)", "Ending soon", "Parked" };
            Color[] colors = { Palette.Faint, Palette.Orange, Palette.Yellow, Palette.Green };

            int x = right;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            for (int i = 0; i < names.Length; i++)
            {
                int textWidth = TextRenderer.MeasureText(names[i], detailFont).Width;
                x -= textWidth;
                TextRenderer.DrawText(graphics, names[i], detailFont, new Point(x, y), Palette.Muted);
                x -= ScaleForScreen(18);
                Rectangle swatch = new Rectangle(x, y + (detailFont.Height - 12) / 2, 12, 12);
                using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(swatch, 3))
                using (SolidBrush brush = new SolidBrush(colors[i]))
                {
                    graphics.FillPath(brush, path);
                }
                x -= ScaleForScreen(22);
            }
        }

        // a zone: header with "taken / capacity" and a fill bar, then the car park drawing
        private void DrawZoneCard(Graphics graphics, Rectangle card, string zone)
        {
            GraphicsHelper.DrawRoundedCard(graphics, card, 12);
            int padding = ScaleForScreen(18);
            List<Permit> permits = GetPermitsInZone(zone);
            int taken = Math.Min(permits.Count, ParkingLot.SpotsPerZone);

            // header: "Zone A" on the left, "5 / 8 taken" on the right
            int y = card.Y + padding;
            Rectangle headerRow = new Rectangle(card.X + padding, y, card.Width - padding * 2, cardTitleFont.Height);
            TextRenderer.DrawText(graphics, "Zone " + zone, cardTitleFont, headerRow, Palette.Text,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            TextRenderer.DrawText(graphics, taken + " / " + ParkingLot.SpotsPerZone + " taken", detailValueFont, headerRow, Palette.Muted,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            y += cardTitleFont.Height + ScaleForScreen(8);

            // fill bar: green, yellow when nearly full, red when full
            float fullness = (float)taken / ParkingLot.SpotsPerZone;
            Color barColor = fullness >= 1f ? Palette.Red : (fullness >= 0.75f ? Palette.Yellow : Palette.Green);
            Rectangle track = new Rectangle(card.X + padding, y, card.Width - padding * 2, ScaleForScreen(6));
            DrawRoundedBar(graphics, track, Palette.Background);
            if (taken > 0)
                DrawRoundedBar(graphics, new Rectangle(track.X, track.Y, Math.Max(track.Height, (int)(track.Width * fullness)), track.Height), barColor);
            y += track.Height + ScaleForScreen(14);

            if (permits.Count > ParkingLot.SpotsPerZone)
            {
                TextRenderer.DrawText(graphics, (permits.Count - ParkingLot.SpotsPerZone) + " permit(s) over capacity",
                    detailFont, new Point(card.X + padding, y), Palette.Red);
                y += detailFont.Height + ScaleForScreen(6);
            }

            Rectangle lot = new Rectangle(card.X + padding, y, card.Width - padding * 2, card.Bottom - padding - y);
            DrawCarPark(graphics, lot, zone, permits);
        }

        // the car park: asphalt, two columns of spots facing a driving lane in the middle
        private void DrawCarPark(Graphics graphics, Rectangle lot, string zone, List<Permit> permits)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath lotPath = GraphicsHelper.CreateRoundedRectangle(lot, 10))
            using (SolidBrush asphaltBrush = new SolidBrush(Asphalt))
            {
                graphics.FillPath(asphaltBrush, lotPath);
            }

            int inset = ScaleForScreen(10);
            Rectangle inner = Rectangle.Inflate(lot, -inset, -inset);
            int laneWidth = inner.Width * 22 / 100;
            int spotDepth = (inner.Width - laneWidth) / 2;
            int rows = (ParkingLot.SpotsPerZone + 1) / 2;
            int spotHeight = inner.Height / rows;
            int laneLeft = inner.X + spotDepth;

            // driving lane: dashed yellow centre line and the zone letter at the top
            using (Pen lanePen = new Pen(LaneLine, 2f))
            {
                lanePen.DashStyle = DashStyle.Dash;
                int laneCenter = laneLeft + laneWidth / 2;
                graphics.DrawLine(lanePen, laneCenter, inner.Y + zoneLetterFont.Height + ScaleForScreen(4), laneCenter, inner.Bottom);
            }
            TextRenderer.DrawText(graphics, zone, zoneLetterFont,
                new Rectangle(laneLeft, inner.Y, laneWidth, zoneLetterFont.Height),
                Color.FromArgb(70, Palette.Muted), TextFormatFlags.HorizontalCenter | TextFormatFlags.Top | TextFormatFlags.NoPadding);

            // white spot lines: a back line and a line between every two spots
            using (Pen linePen = new Pen(SpotLine, 3f))
            {
                graphics.DrawLine(linePen, inner.X, inner.Y, inner.X, inner.Bottom);
                graphics.DrawLine(linePen, inner.Right, inner.Y, inner.Right, inner.Bottom);
                for (int row = 0; row <= rows; row++)
                {
                    int lineY = inner.Y + row * spotHeight;
                    graphics.DrawLine(linePen, inner.X, lineY, laneLeft, lineY);
                    graphics.DrawLine(linePen, laneLeft + laneWidth, lineY, inner.Right, lineY);
                }
            }

            // the spots: left column first (top to bottom), then the right column
            for (int spot = 0; spot < ParkingLot.SpotsPerZone; spot++)
            {
                bool leftColumn = spot < rows;
                int row = leftColumn ? spot : spot - rows;
                int x = leftColumn ? inner.X : laneLeft + laneWidth;
                Rectangle spotArea = new Rectangle(x, inner.Y + row * spotHeight, spotDepth, spotHeight);

                Permit permit = spot < permits.Count ? permits[spot] : null;
                DrawSpot(graphics, spotArea, permit, leftColumn);

                spotBounds.Add(spotArea);
                spotPermits.Add(permit);
                if (spotBounds.Count - 1 == hoveredSpot)
                {
                    using (SolidBrush hoverBrush = new SolidBrush(Color.FromArgb(40, Palette.Accent)))
                    {
                        graphics.FillRectangle(hoverBrush, Rectangle.Inflate(spotArea, -2, -2));
                    }
                }
            }
        }

        // one spot: a car with its plate, a dashed "reserved" box, or the word FREE
        private void DrawSpot(Graphics graphics, Rectangle spot, Permit permit, bool carFacesRight)
        {
            TextFormatFlags centered = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;

            if (permit == null)
            {
                TextRenderer.DrawText(graphics, "FREE", spotLabelFont, spot, Palette.Faint, centered);
                return;
            }

            int labelHeight = spotLabelFont.Height;
            Rectangle label = new Rectangle(spot.X, spot.Bottom - labelHeight - ScaleForScreen(4), spot.Width, labelHeight);

            if (!permit.IsPaid)
            {
                // reserved: soft peach box with a dashed orange edge and the word RESERVED
                Rectangle box = new Rectangle(spot.X + ScaleForScreen(8), spot.Y + ScaleForScreen(8),
                    spot.Width - ScaleForScreen(16), spot.Height - labelHeight - ScaleForScreen(16));
                using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(box, 6))
                using (SolidBrush fillBrush = new SolidBrush(Palette.OrangeSoft))
                using (Pen dashPen = new Pen(Palette.Orange, 2f))
                {
                    dashPen.DashStyle = DashStyle.Dash;
                    graphics.FillPath(fillBrush, path);
                    graphics.DrawPath(dashPen, path);
                }
                TextRenderer.DrawText(graphics, "RESERVED", spotLabelFont, box, Palette.OrangeText, centered);
                TextRenderer.DrawText(graphics, permit.PlateNumber, spotLabelFont, label, Palette.Text, centered);
                return;
            }

            // parked car, nose towards the driving lane, plate number under it
            int carLength = spot.Width * 84 / 100;
            int carWidth = Math.Min(spot.Height - labelHeight - ScaleForScreen(14), carLength * 58 / 100);
            int carX = carFacesRight ? spot.Right - carLength - ScaleForScreen(6) : spot.X + ScaleForScreen(6);
            int carY = spot.Y + (spot.Height - labelHeight - ScaleForScreen(4) - carWidth) / 2;
            Color carColor = permit.ExpiresSoon ? Palette.Yellow : Palette.Green;
            GraphicsHelper.DrawCar(graphics, new Rectangle(carX, carY, carLength, carWidth), carColor, carFacesRight);
            TextRenderer.DrawText(graphics, permit.PlateNumber, spotLabelFont, label, Palette.Text, centered);
        }

        // ---------- mouse on the map ----------

        private int FindSpotAt(Point location)
        {
            for (int i = 0; i < spotBounds.Count; i++)
            {
                if (spotBounds[i].Contains(location))
                    return i;
            }
            return -1;
        }

        private void pnlMapPage_MouseMove(object sender, MouseEventArgs e)
        {
            int spot = FindSpotAt(e.Location);
            if (spot == hoveredSpot)
                return;

            hoveredSpot = spot;
            pnlMapPage.Invalidate();

            Permit permit = spot >= 0 ? spotPermits[spot] : null;
            pnlMapPage.Cursor = permit != null ? Cursors.Hand : Cursors.Default;
            if (spot < 0)
            {
                tipSpot.Hide(pnlMapPage);
            }
            else if (permit == null)
            {
                tipSpot.Show("Free spot", pnlMapPage, e.X + 16, e.Y + 16);
            }
            else
            {
                string state = permit.IsPaid ? permit.DaysLeft + " days left" : "Reserved, awaiting payment";
                tipSpot.Show(permit.PlateNumber + "  ·  " + permit.OwnerName + "\n" + permit.PermitType + " permit  ·  " + state +
                    "\nClick to open", pnlMapPage, e.X + 16, e.Y + 16);
            }
        }

        private void pnlMapPage_MouseLeave(object sender, EventArgs e)
        {
            hoveredSpot = -1;
            tipSpot.Hide(pnlMapPage);
            pnlMapPage.Invalidate();
        }

        // clicking a car or reserved spot opens that permit on the Permits page
        private void pnlMapPage_MouseClick(object sender, MouseEventArgs e)
        {
            int spot = FindSpotAt(e.Location);
            if (spot >= 0 && spotPermits[spot] != null)
            {
                tipSpot.Hide(pnlMapPage);
                OpenPermitOnPermitsPage(spotPermits[spot].Id);
            }
        }
    }
}
