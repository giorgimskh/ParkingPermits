using System.Drawing.Drawing2D;

namespace ParkingPermits
{
    // The second half of MainForm: the page tabs in the top bar (Permits / Overview),
    // the status tabs above the grid, and the drawing of the Overview page.
    public partial class MainForm
    {
        // ---------- state ----------

        private static readonly string[] pageNames = { "Permits", "Overview", "Parking map" };
        private int currentPage;          // 0 = Permits, 1 = Overview, 2 = Parking map
        private int hoveredPage = -1;     // page tab under the mouse, -1 = none

        private static readonly string[] statusTabNames = { "All", "Active", "Ending soon", "Awaiting payment", "Expired" };
        private int selectedStatusTab;    // index into statusTabNames
        private int hoveredStatusTab = -1;

        // rows of the "Needs attention" list on the Overview page, so a click can find its permit
        private readonly List<Rectangle> attentionRowBounds = new List<Rectangle>();
        private readonly List<int> attentionRowIds = new List<int>();

        private readonly Font navFont = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
        private readonly Font tabFont = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
        private readonly Font pillFont = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
        private readonly Font cardTitleFont = new Font("Segoe UI Semibold", 11.5F, FontStyle.Bold);

        // ---------- status tabs above the grid ----------

        private static Color GetStatusTabColor(int tab)
        {
            Color[] colors = { Palette.Accent, Palette.Green, Palette.Yellow, Palette.Orange, Palette.Red };
            return colors[tab];
        }

        // which permits belong on each status tab
        private static bool MatchesStatusTab(Permit permit, int tab)
        {
            switch (tab)
            {
                case 1: return permit.IsActive;
                case 2: return permit.ExpiresSoon;
                case 3: return !permit.IsExpired && !permit.IsPaid;
                case 4: return permit.IsExpired;
                default: return true;
            }
        }

        // how many of the current search results belong on a tab (the number in its pill)
        private int CountInStatusTab(int tab)
        {
            int count = 0;
            foreach (Permit permit in searchResults)
            {
                if (MatchesStatusTab(permit, tab))
                    count++;
            }
            return count;
        }

        // where each status tab is drawn: name + count pill, side by side
        private Rectangle[] GetStatusTabBounds()
        {
            Rectangle[] bounds = new Rectangle[statusTabNames.Length];
            int x = 0;
            for (int i = 0; i < statusTabNames.Length; i++)
            {
                int textWidth = TextRenderer.MeasureText(statusTabNames[i], tabFont).Width;
                int pillWidth = TextRenderer.MeasureText(CountInStatusTab(i).ToString(), pillFont).Width + ScaleForScreen(12);
                int width = ScaleForScreen(12) + textWidth + ScaleForScreen(6) + pillWidth + ScaleForScreen(12);
                bounds[i] = new Rectangle(x, 0, width, pnlTabs.Height);
                x += width + ScaleForScreen(4);
            }
            return bounds;
        }

        private void pnlTabs_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // the long thin line under all tabs
            using (Pen linePen = new Pen(Palette.Border))
            {
                graphics.DrawLine(linePen, 0, pnlTabs.Height - 1, pnlTabs.Width, pnlTabs.Height - 1);
            }

            Rectangle[] bounds = GetStatusTabBounds();
            for (int i = 0; i < bounds.Length; i++)
            {
                Rectangle tab = bounds[i];
                bool isSelected = i == selectedStatusTab;
                bool isHovered = i == hoveredStatusTab;
                Color tabColor = GetStatusTabColor(i);

                // tab name
                int textWidth = TextRenderer.MeasureText(statusTabNames[i], tabFont).Width;
                Rectangle textArea = new Rectangle(tab.X + ScaleForScreen(12), tab.Y, textWidth, tab.Height - 3);
                Color textColor = isSelected || isHovered ? Palette.Text : Palette.Muted;
                TextRenderer.DrawText(graphics, statusTabNames[i], tabFont, textArea, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

                // count pill: tinted in the tab's colour when selected, grey otherwise
                string count = CountInStatusTab(i).ToString();
                int pillWidth = TextRenderer.MeasureText(count, pillFont).Width + ScaleForScreen(12);
                int pillHeight = pillFont.Height + ScaleForScreen(4);
                Rectangle pill = new Rectangle(textArea.Right + ScaleForScreen(6), (tab.Height - 3 - pillHeight) / 2, pillWidth, pillHeight);
                Color pillBack = isSelected ? Color.FromArgb(40, tabColor) : Palette.Background;
                Color pillText = isSelected ? tabColor : Palette.Muted;
                using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(pill, pillHeight / 2))
                using (SolidBrush pillBrush = new SolidBrush(pillBack))
                {
                    graphics.FillPath(pillBrush, path);
                }
                TextRenderer.DrawText(graphics, count, pillFont, pill, pillText,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

                // underline of the selected tab
                if (isSelected)
                {
                    using (SolidBrush lineBrush = new SolidBrush(tabColor))
                    {
                        graphics.FillRectangle(lineBrush, tab.X + ScaleForScreen(6), tab.Height - 3, tab.Width - ScaleForScreen(12), 3);
                    }
                }
            }
        }

        private int FindStatusTabAt(Point location)
        {
            Rectangle[] bounds = GetStatusTabBounds();
            for (int i = 0; i < bounds.Length; i++)
            {
                if (bounds[i].Contains(location))
                    return i;
            }
            return -1;
        }

        private void pnlTabs_MouseMove(object sender, MouseEventArgs e)
        {
            int tab = FindStatusTabAt(e.Location);
            if (tab != hoveredStatusTab)
            {
                hoveredStatusTab = tab;
                pnlTabs.Invalidate();
            }
        }

        private void pnlTabs_MouseLeave(object sender, EventArgs e)
        {
            hoveredStatusTab = -1;
            pnlTabs.Invalidate();
        }

        private void pnlTabs_MouseClick(object sender, MouseEventArgs e)
        {
            int tab = FindStatusTabAt(e.Location);
            if (tab >= 0 && tab != selectedStatusTab)
            {
                selectedStatusTab = tab;
                ShowSelectedStatusTab();
                pnlTabs.Invalidate();
            }
        }

        // ---------- page tabs in the top bar ----------

        private Rectangle[] GetPageTabBounds()
        {
            Rectangle[] bounds = new Rectangle[pageNames.Length];
            int x = Math.Max(lblTitle.Right, lblSubtitle.Right) + ScaleForScreen(48);
            for (int i = 0; i < pageNames.Length; i++)
            {
                int width = TextRenderer.MeasureText(pageNames[i], navFont).Width + ScaleForScreen(32);
                bounds[i] = new Rectangle(x, 0, width, pnlHeader.Height);
                x += width;
            }
            return bounds;
        }

        private void DrawPageTabs(Graphics graphics)
        {
            Rectangle[] bounds = GetPageTabBounds();
            for (int i = 0; i < bounds.Length; i++)
            {
                bool isSelected = i == currentPage;
                bool isHovered = i == hoveredPage;
                // on the blue bar: white for the open page and on hover, pale blue for the others
                Color textColor = isSelected || isHovered ? Palette.HeaderText : Palette.HeaderMuted;
                TextRenderer.DrawText(graphics, pageNames[i], navFont, bounds[i], textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                if (isSelected)
                {
                    using (SolidBrush lineBrush = new SolidBrush(Palette.HeaderText))
                    {
                        graphics.FillRectangle(lineBrush, bounds[i].X + ScaleForScreen(10), bounds[i].Bottom - 3,
                            bounds[i].Width - ScaleForScreen(20), 3);
                    }
                }
            }
        }

        private int FindPageTabAt(Point location)
        {
            Rectangle[] bounds = GetPageTabBounds();
            for (int i = 0; i < bounds.Length; i++)
            {
                if (bounds[i].Contains(location))
                    return i;
            }
            return -1;
        }

        private void pnlHeader_MouseMove(object sender, MouseEventArgs e)
        {
            int page = FindPageTabAt(e.Location);
            pnlHeader.Cursor = page >= 0 ? Cursors.Hand : Cursors.Default;
            if (page != hoveredPage)
            {
                hoveredPage = page;
                pnlHeader.Invalidate();
            }
        }

        private void pnlHeader_MouseLeave(object sender, EventArgs e)
        {
            hoveredPage = -1;
            pnlHeader.Invalidate();
        }

        private void pnlHeader_MouseClick(object sender, MouseEventArgs e)
        {
            int page = FindPageTabAt(e.Location);
            if (page >= 0)
                ShowPage(page);
        }

        private void ShowPage(int page)
        {
            currentPage = page;
            pnlPermitsPage.Visible = page == 0;
            pnlOverviewPage.Visible = page == 1;
            pnlMapPage.Visible = page == 2;
            pnlHeader.Invalidate();
            pnlOverviewPage.Invalidate();
            pnlMapPage.Invalidate();
        }

        // switches to the Permits page and selects one permit in the grid
        // (search, type filter and status tab are cleared first, so the permit is surely in the list)
        private void OpenPermitOnPermitsPage(int permitId)
        {
            selectedStatusTab = 0;
            cmbPermitType.SelectedIndex = 0;
            if (txtSearch.Text.Length > 0)
                txtSearch.Text = "";   // this reloads the grid
            else
                ShowSelectedStatusTab();
            pnlTabs.Invalidate();

            ShowPage(0);
            SelectPermitInGrid(permitId);
        }

        // ---------- Overview page ----------

        private void pnlOverviewPage_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            int margin = ScaleForScreen(24);
            int gap = ScaleForScreen(16);
            int width = pnlOverviewPage.Width - margin * 2;

            // row 1: the five summary cards
            int cardsHeight = ScaleForScreen(84);
            DrawSummaryCards(graphics, new Rectangle(margin, margin, width, cardsHeight));

            // row 2: three cards that fill the rest of the page
            int top = margin + cardsHeight + gap;
            int height = pnlOverviewPage.Height - top - margin;
            int leftWidth = (width - gap * 2) * 30 / 100;
            int rightWidth = (width - gap * 2) * 34 / 100;
            int middleWidth = width - gap * 2 - leftWidth - rightWidth;

            Rectangle statusCard = new Rectangle(margin, top, leftWidth, height);
            Rectangle breakdownCard = new Rectangle(statusCard.Right + gap, top, middleWidth, height);
            Rectangle attentionCard = new Rectangle(breakdownCard.Right + gap, top, rightWidth, height);

            DrawStatusChart(graphics, statusCard);
            DrawBreakdown(graphics, breakdownCard);
            DrawNeedsAttention(graphics, attentionCard);
        }

        // white card with a title and a small grey subtitle; returns where the content may start
        private int DrawOverviewCard(Graphics graphics, Rectangle card, string title, string subtitle)
        {
            GraphicsHelper.DrawRoundedCard(graphics, card, 12);
            int padding = ScaleForScreen(20);
            int y = card.Y + padding;
            TextRenderer.DrawText(graphics, title, cardTitleFont, new Point(card.X + padding, y), Palette.Text);
            y += cardTitleFont.Height + ScaleForScreen(2);
            TextRenderer.DrawText(graphics, subtitle, detailFont, new Point(card.X + padding, y), Palette.Muted);
            return y + detailFont.Height + ScaleForScreen(16);
        }

        // donut chart: how the permits split into the four statuses
        private void DrawStatusChart(Graphics graphics, Rectangle card)
        {
            int y = DrawOverviewCard(graphics, card, "Status breakdown", "All permits by current status");
            int padding = ScaleForScreen(20);

            string[] names = { "Valid", "Ending soon", "Awaiting payment", "Expired" };
            int[] counts = { activeCount - expiringSoonCount, expiringSoonCount, awaitingPaymentCount, expiredCount };
            Color[] colors = { Palette.Green, Palette.Yellow, Palette.Orange, Palette.Red };

            int rowHeight = ScaleForScreen(30);
            int legendHeight = rowHeight * names.Length;
            int size = Math.Min(card.Width - padding * 4, card.Bottom - padding - legendHeight - ScaleForScreen(20) - y);
            size = Math.Min(size, ScaleForScreen(200));
            int thickness = ScaleForScreen(26);

            if (size > ScaleForScreen(80))
            {
                Rectangle ring = new Rectangle(card.X + (card.Width - size) / 2 + thickness / 2, y + thickness / 2,
                    size - thickness, size - thickness);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen trackPen = new Pen(Palette.Border, thickness))
                {
                    graphics.DrawEllipse(trackPen, ring);
                }

                // one arc per status, going round from the top
                float start = -90;
                for (int i = 0; i < counts.Length; i++)
                {
                    if (totalCount == 0 || counts[i] == 0)
                        continue;
                    float sweep = 360f * counts[i] / totalCount;
                    using (Pen arcPen = new Pen(colors[i], thickness))
                    {
                        // a small gap between the arcs, unless one status is the whole ring
                        float drawSweep = sweep >= 360f ? 360f : sweep - 1.5f;
                        graphics.DrawArc(arcPen, ring, start, drawSweep);
                    }
                    start += sweep;
                }

                // total in the middle
                int numberHeight = ringNumberFont.Height;
                int textTop = ring.Y + ring.Height / 2 - (numberHeight + detailFont.Height) / 2;
                TextRenderer.DrawText(graphics, totalCount.ToString(), ringNumberFont,
                    new Rectangle(ring.X, textTop, ring.Width, numberHeight), Palette.Text,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                TextRenderer.DrawText(graphics, "permits", detailFont,
                    new Rectangle(ring.X, textTop + numberHeight, ring.Width, detailFont.Height), Palette.Muted,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                y += size + ScaleForScreen(20);
            }

            // legend: colour, name, count and percentage
            for (int i = 0; i < names.Length; i++)
            {
                Rectangle row = new Rectangle(card.X + padding, y + i * rowHeight, card.Width - padding * 2, rowHeight);
                Rectangle swatch = new Rectangle(row.X, row.Y + (row.Height - 10) / 2, 10, 10);
                using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(swatch, 3))
                using (SolidBrush brush = new SolidBrush(colors[i]))
                {
                    graphics.FillPath(brush, path);
                }

                Rectangle textRow = new Rectangle(row.X + 18, row.Y, row.Width - 18, row.Height);
                TextRenderer.DrawText(graphics, names[i], detailFont, textRow, Palette.Text,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

                int percent = totalCount == 0 ? 0 : (int)Math.Round(100.0 * counts[i] / totalCount);
                string numbers = counts[i] + "   (" + percent + "%)";
                TextRenderer.DrawText(graphics, numbers, detailValueFont, textRow, Palette.Text,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }

        // bars: permits per type (split into status colours) and per zone
        private void DrawBreakdown(Graphics graphics, Rectangle card)
        {
            int y = DrawOverviewCard(graphics, card, "Permits by type and zone", "How the permits are spread out");
            int padding = ScaleForScreen(20);
            int barWidth = card.Width - padding * 2;
            int barHeight = ScaleForScreen(12);
            Color[] statusColors = { Palette.Green, Palette.Yellow, Palette.Orange, Palette.Red };

            // --- by type: one stacked bar per type ---
            string[] types = { "Student", "Staff", "Guest" };
            foreach (string type in types)
            {
                int[] counts = new int[4];   // valid, ending soon, awaiting payment, expired
                int typeTotal = 0;
                foreach (Permit permit in allPermits)
                {
                    if (permit.PermitType != type)
                        continue;
                    typeTotal++;
                    if (permit.IsExpired) counts[3]++;
                    else if (!permit.IsPaid) counts[2]++;
                    else if (permit.ExpiresSoon) counts[1]++;
                    else counts[0]++;
                }

                Rectangle labelRow = new Rectangle(card.X + padding, y, barWidth, detailFont.Height);
                TextRenderer.DrawText(graphics, type, detailFont, labelRow, Palette.Text,
                    TextFormatFlags.Left | TextFormatFlags.NoPadding);
                TextRenderer.DrawText(graphics, typeTotal + " permits", detailValueFont, labelRow, Palette.Text,
                    TextFormatFlags.Right | TextFormatFlags.NoPadding);
                y += detailFont.Height + ScaleForScreen(6);

                Rectangle bar = new Rectangle(card.X + padding, y, barWidth, barHeight);
                DrawStackedBar(graphics, bar, counts, statusColors, typeTotal);
                y += barHeight + ScaleForScreen(18);
            }

            // --- by zone: spots taken out of the zone's capacity ---
            y += ScaleForScreen(6);
            TextRenderer.DrawText(graphics, "SPOTS TAKEN BY ZONE", sectionFont, new Point(card.X + padding, y), Palette.Muted);
            y += sectionFont.Height + ScaleForScreen(10);

            string[] zones = ParkingLot.Zones;
            int[] zoneCounts = new int[zones.Length];
            for (int i = 0; i < zones.Length; i++)
            {
                foreach (Permit permit in allPermits)
                {
                    if (permit.Zone == zones[i] && permit.TakesSpot)
                        zoneCounts[i]++;
                }
            }

            int labelWidth = ScaleForScreen(60);
            int countWidth = ScaleForScreen(46);
            int rowHeight = ScaleForScreen(30);
            for (int i = 0; i < zones.Length; i++)
            {
                if (y + rowHeight > card.Bottom - padding)
                    break;   // window too short for more rows

                Rectangle row = new Rectangle(card.X + padding, y, barWidth, rowHeight);
                TextRenderer.DrawText(graphics, "Zone " + zones[i], detailFont,
                    new Rectangle(row.X, row.Y, labelWidth, row.Height), Palette.Text,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

                int trackWidth = row.Width - labelWidth - countWidth;
                Rectangle track = new Rectangle(row.X + labelWidth, row.Y + (row.Height - barHeight) / 2, trackWidth, barHeight);
                int taken = Math.Min(zoneCounts[i], ParkingLot.SpotsPerZone);
                int fillWidth = Math.Max(barHeight, trackWidth * taken / ParkingLot.SpotsPerZone);
                Color fillColor = taken >= ParkingLot.SpotsPerZone ? Palette.Red : Palette.Accent;
                DrawRoundedBar(graphics, track, Palette.Background);
                if (taken > 0)
                    DrawRoundedBar(graphics, new Rectangle(track.X, track.Y, fillWidth, barHeight), fillColor);

                TextRenderer.DrawText(graphics, zoneCounts[i] + " / " + ParkingLot.SpotsPerZone, detailValueFont,
                    new Rectangle(row.Right - countWidth, row.Y, countWidth, row.Height), Palette.Text,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                y += rowHeight;
            }
        }

        private static void DrawRoundedBar(Graphics graphics, Rectangle bar, Color color)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(bar, bar.Height / 2))
            using (SolidBrush brush = new SolidBrush(color))
            {
                graphics.FillPath(brush, path);
            }
        }

        // a rounded bar split into coloured parts, one per status
        private static void DrawStackedBar(Graphics graphics, Rectangle bar, int[] counts, Color[] colors, int total)
        {
            DrawRoundedBar(graphics, bar, Palette.Background);
            if (total == 0)
                return;

            using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(bar, bar.Height / 2))
            {
                graphics.SetClip(path);
                int x = bar.X;
                for (int i = 0; i < counts.Length; i++)
                {
                    int partWidth = (int)Math.Round((double)bar.Width * counts[i] / total);
                    if (i == counts.Length - 1)
                        partWidth = bar.Right - x;   // last part fills any rounding gap
                    using (SolidBrush brush = new SolidBrush(colors[i]))
                    {
                        graphics.FillRectangle(brush, x, bar.Y, partWidth, bar.Height);
                    }
                    x += partWidth;
                }
                graphics.ResetClip();
            }
        }

        // list of permits that need action: ending within 30 days, or not paid yet
        private void DrawNeedsAttention(Graphics graphics, Rectangle card)
        {
            int y = DrawOverviewCard(graphics, card, "Needs attention", "Ending within 30 days or awaiting payment");
            int padding = ScaleForScreen(20);
            attentionRowBounds.Clear();
            attentionRowIds.Clear();

            List<Permit> endingSoon = new List<Permit>();
            List<Permit> awaitingPayment = new List<Permit>();
            foreach (Permit permit in allPermits)
            {
                if (permit.ExpiresSoon)
                    endingSoon.Add(permit);
                else if (!permit.IsExpired && !permit.IsPaid)
                    awaitingPayment.Add(permit);
            }
            endingSoon.Sort((first, second) => first.DaysLeft.CompareTo(second.DaysLeft));

            List<Permit> items = new List<Permit>(endingSoon);
            items.AddRange(awaitingPayment);

            if (items.Count == 0)
            {
                TextRenderer.DrawText(graphics, "All permits are in order.", detailFont,
                    new Point(card.X + padding, y), Palette.Muted);
                return;
            }

            int rowHeight = detailValueFont.Height + detailFont.Height + ScaleForScreen(18);
            int bottom = card.Bottom - padding - detailFont.Height - ScaleForScreen(6);
            int shown = 0;

            using (Pen linePen = new Pen(Palette.Border))
            {
                foreach (Permit permit in items)
                {
                    if (y + rowHeight > bottom)
                        break;

                    Rectangle row = new Rectangle(card.X + padding, y, card.Width - padding * 2, rowHeight);
                    graphics.DrawLine(linePen, row.X, row.Y, row.Right, row.Y);

                    // plate and owner on the left
                    int textTop = row.Y + ScaleForScreen(8);
                    TextRenderer.DrawText(graphics, permit.PlateNumber, detailValueFont, new Point(row.X, textTop), Palette.Text);
                    TextRenderer.DrawText(graphics, permit.OwnerName + "  ·  " + permit.PermitType, detailFont,
                        new Point(row.X, textTop + detailValueFont.Height), Palette.Muted);

                    // a pill on the right: days left, or "Unpaid"
                    string pillText = permit.ExpiresSoon ? permit.DaysLeft + (permit.DaysLeft == 1 ? " day" : " days") : "Unpaid";
                    GetStatusPillColors(permit, out Color pillBack, out Color pillTextColor);
                    int pillWidth = TextRenderer.MeasureText(pillText, badgeFont).Width + ScaleForScreen(20);
                    GraphicsHelper.DrawBadge(graphics, new Rectangle(row.Right - pillWidth - 8, row.Y, pillWidth + 16, row.Height),
                        pillText, badgeFont, pillBack, pillTextColor);

                    attentionRowBounds.Add(row);
                    attentionRowIds.Add(permit.Id);
                    y += rowHeight;
                    shown++;
                }
            }

            if (shown < items.Count)
            {
                TextRenderer.DrawText(graphics, "+ " + (items.Count - shown) + " more", detailFont,
                    new Point(card.X + padding, y + ScaleForScreen(6)), Palette.Muted);
            }
        }

        private int FindAttentionRowAt(Point location)
        {
            for (int i = 0; i < attentionRowBounds.Count; i++)
            {
                if (attentionRowBounds[i].Contains(location))
                    return attentionRowIds[i];
            }
            return -1;
        }

        private void pnlOverviewPage_MouseMove(object sender, MouseEventArgs e)
        {
            pnlOverviewPage.Cursor = FindAttentionRowAt(e.Location) >= 0 ? Cursors.Hand : Cursors.Default;
        }

        // clicking a permit in "Needs attention" opens it on the Permits page
        private void pnlOverviewPage_MouseClick(object sender, MouseEventArgs e)
        {
            int permitId = FindAttentionRowAt(e.Location);
            if (permitId >= 0)
                OpenPermitOnPermitsPage(permitId);
        }
    }
}
