using System.Drawing.Drawing2D;
using System.Text;

namespace ParkingPermits
{
    public partial class MainForm : Form
    {
        private readonly PermitRepository repository = new PermitRepository();

        // allPermits    - every permit in the table (for the Overview page and the totals)
        // searchResults - the permits that match the search box, type filter and sort
        // shownPermits  - searchResults narrowed down by the selected status tab; this is what the grid shows
        private List<Permit> allPermits = new List<Permit>();
        private List<Permit> searchResults = new List<Permit>();
        private List<Permit> shownPermits = new List<Permit>();

        // numbers for the summary cards, counted over all permits in LoadPermits
        private int totalCount;
        private int activeCount;
        private int expiringSoonCount;
        private int expiredCount;
        private int awaitingPaymentCount;

        // fonts for the drawing code, created once and reused on every repaint
        private readonly Font signFont = new Font("Segoe UI", 18F, FontStyle.Bold);
        private readonly Font tileValueFont = new Font("Segoe UI Semibold", 19F, FontStyle.Bold);
        private readonly Font tileCaptionFont = new Font("Segoe UI", 9F);
        private readonly Font badgeFont = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
        private readonly Font bigBadgeFont = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        private readonly Font emptyFont = new Font("Segoe UI", 11F);
        private readonly Font sectionFont = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
        private readonly Font plateFont = new Font("Segoe UI", 19F, FontStyle.Bold);
        private readonly Font ownerFont = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
        private readonly Font detailFont = new Font("Segoe UI", 9.75F);
        private readonly Font detailValueFont = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
        private readonly Font ringNumberFont = new Font("Segoe UI Semibold", 22F, FontStyle.Bold);

        public MainForm()
        {
            InitializeComponent();
            dgvPermits.AutoGenerateColumns = false;
            // the details panel has its own teal "parking pass" colours
            pnlDetails.SetCardColors(DetailsPalette.Body, DetailsPalette.Border);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // the status notice in the top bar gets rounded corners
            // (done here, after Windows has scaled the controls for the screen)
            using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(
                new Rectangle(0, 0, lblStatus.Width, lblStatus.Height), 8))
            {
                lblStatus.Region = new Region(path);
            }

            cmbSort.SelectedIndex = 0;
            // choosing "All types" fires SelectedIndexChanged, which loads the grid
            cmbPermitType.SelectedIndex = 0;
            // start with the grid focused, so the search box shows its hint text
            ActiveControl = dgvPermits;
        }

        private void LoadPermits()
        {
            string searchText = txtSearch.Text.Trim();
            // index 0 is "All types": send an empty string, not the words "All types"
            string permitType = cmbPermitType.SelectedIndex > 0 ? cmbPermitType.Text : "";

            try
            {
                searchResults = repository.Search(searchText, permitType, cmbSort.SelectedIndex);
                allPermits = repository.Search("", "", 0);
            }
            catch (Exception ex)
            {
                ShowDatabaseError("Could not read from the database. Make sure database.sql has been run.", ex);
                return;
            }

            CountTotals();
            ShowSelectedStatusTab();
            pnlTabs.Invalidate();
            pnlOverviewPage.Invalidate();
            pnlMapPage.Invalidate();
        }

        // fills the grid with the search results that belong to the selected status tab
        private void ShowSelectedStatusTab()
        {
            // remember which permit was selected, so it stays selected after the grid is refilled
            Permit previouslySelected = GetSelectedPermit();

            shownPermits = new List<Permit>();
            foreach (Permit permit in searchResults)
            {
                if (MatchesStatusTab(permit, selectedStatusTab))
                    shownPermits.Add(permit);
            }

            dgvPermits.DataSource = null;
            dgvPermits.DataSource = shownPermits;
            if (previouslySelected != null)
                SelectPermitInGrid(previouslySelected.Id);
            pnlDetails.Invalidate();

            if (shownPermits.Count == 0)
                lblCount.Text = "No permits match your search.";
            else
                lblCount.Text = "Showing " + shownPermits.Count + " of " + totalCount + " permits";
        }

        private void CountTotals()
        {
            totalCount = allPermits.Count;
            activeCount = 0;
            expiringSoonCount = 0;
            expiredCount = 0;
            awaitingPaymentCount = 0;

            // every permit is exactly one of: active, expired, awaiting payment
            foreach (Permit permit in allPermits)
            {
                if (permit.IsExpired)
                    expiredCount++;
                else if (!permit.IsPaid)
                    awaitingPaymentCount++;
                else
                    activeCount++;

                if (permit.ExpiresSoon)
                    expiringSoonCount++;
            }
        }

        private Permit GetSelectedPermit()
        {
            if (dgvPermits.CurrentRow == null)
                return null;
            return dgvPermits.CurrentRow.DataBoundItem as Permit;
        }

        // selects the grid row of a permit (by Id or by plate number); false if it is not in the grid
        private bool SelectPermitInGrid(int permitId)
        {
            return SelectPermitInGrid(permit => permit.Id == permitId);
        }

        private bool SelectPermitInGrid(string plateNumber)
        {
            return SelectPermitInGrid(permit => permit.PlateNumber == plateNumber);
        }

        private bool SelectPermitInGrid(Func<Permit, bool> isWanted)
        {
            foreach (DataGridViewRow row in dgvPermits.Rows)
            {
                Permit permit = row.DataBoundItem as Permit;
                if (permit != null && isWanted(permit))
                {
                    dgvPermits.CurrentCell = row.Cells[colPlateNumber.Index];
                    return true;
                }
            }
            return false;
        }

        private void ShowDatabaseError(string message, Exception ex)
        {
            MessageBox.Show(message + "\n\n" + ex.Message, "Database error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // shows a green notice in the top bar; the timer hides it again after 3 seconds
        private void ShowStatus(string message)
        {
            lblStatus.Text = "✓  " + message;
            lblStatus.Visible = true;
            tmrStatus.Stop();
            tmrStatus.Start();
        }

        private void tmrStatus_Tick(object sender, EventArgs e)
        {
            tmrStatus.Stop();
            lblStatus.Visible = false;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadPermits();
        }

        private void cmbPermitType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPermits();
        }

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadPermits();
        }

        private void dgvPermits_SelectionChanged(object sender, EventArgs e)
        {
            pnlDetails.Invalidate();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string addedPlate = null;
            using (PermitForm form = new PermitForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        repository.Add(form.CurrentPermit);
                        addedPlate = form.CurrentPermit.PlateNumber;
                        ShowStatus("Permit " + addedPlate + " added.");
                    }
                    catch (Exception ex)
                    {
                        ShowDatabaseError("The permit could not be added.", ex);
                    }
                }
            }
            LoadPermits();

            // select the new permit, so it is visible in the grid and in the details panel
            if (addedPlate != null)
                SelectPermitInGrid(addedPlate);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditSelectedPermit();
        }

        private void dgvPermits_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // e.RowIndex is -1 when the header is double-clicked
            if (e.RowIndex >= 0)
                EditSelectedPermit();
        }

        private void EditSelectedPermit()
        {
            Permit selectedPermit = GetSelectedPermit();
            if (selectedPermit == null)
            {
                MessageBox.Show("Select a permit to edit first.", "No permit selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (PermitForm form = new PermitForm(selectedPermit))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        repository.Update(form.CurrentPermit);
                        ShowStatus("Permit " + form.CurrentPermit.PlateNumber + " updated.");
                    }
                    catch (Exception ex)
                    {
                        ShowDatabaseError("The permit could not be updated.", ex);
                    }
                }
            }
            LoadPermits();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            Permit selectedPermit = GetSelectedPermit();
            if (selectedPermit == null)
            {
                MessageBox.Show("Select a permit to delete first.", "No permit selected",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult answer = MessageBox.Show(
                "Delete the permit for " + selectedPermit.PlateNumber + " (" + selectedPermit.OwnerName + ")?",
                "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (answer == DialogResult.Yes)
            {
                try
                {
                    repository.Delete(selectedPermit.Id);
                    ShowStatus("Permit " + selectedPermit.PlateNumber + " deleted.");
                }
                catch (Exception ex)
                {
                    ShowDatabaseError("The permit could not be deleted.", ex);
                }
                LoadPermits();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (shownPermits.Count == 0)
            {
                MessageBox.Show("There are no rows to export.", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "CSV files (*.csv)|*.csv";
                dialog.FileName = "permits.csv";
                if (dialog.ShowDialog() != DialogResult.OK)
                    return;

                List<string> lines = new List<string>();
                lines.Add("Plate number,Owner,Permit type,Zone,Valid until,Days left,Payment,Status");
                foreach (Permit permit in shownPermits)
                {
                    lines.Add(string.Join(",",
                        CsvField(permit.PlateNumber),
                        CsvField(permit.OwnerName),
                        CsvField(permit.PermitType),
                        CsvField(permit.Zone),
                        permit.ValidUntil.ToString("yyyy-MM-dd"),
                        permit.DaysLeft,
                        permit.Payment,
                        permit.Validity));
                }

                try
                {
                    File.WriteAllLines(dialog.FileName, lines, Encoding.UTF8);
                    ShowStatus(shownPermits.Count + " permits exported to " + Path.GetFileName(dialog.FileName) + ".");
                }
                catch (IOException ex)
                {
                    MessageBox.Show("The file could not be saved. Is it open in Excel?\n\n" + ex.Message,
                        "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // wraps a value in quotes so commas and quotes inside it do not break the CSV
        private static string CsvField(string value)
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        // ---------- drawing ----------

        // The strong colour that stands for a permit's status:
        // red = expired, orange = awaiting payment, yellow = expires soon, green = valid.
        internal static Color GetStatusColor(Permit permit)
        {
            if (permit.IsExpired)
                return Palette.Red;
            if (!permit.IsPaid)
                return Palette.Orange;
            if (permit.ExpiresSoon)
                return Palette.Yellow;
            return Palette.Green;
        }

        // The light background and dark text used for a permit's status pill
        internal static void GetStatusPillColors(Permit permit, out Color backColor, out Color textColor)
        {
            if (permit.IsExpired)
            {
                backColor = Palette.RedSoft;
                textColor = Palette.RedText;
            }
            else if (!permit.IsPaid)
            {
                backColor = Palette.OrangeSoft;
                textColor = Palette.OrangeText;
            }
            else if (permit.ExpiresSoon)
            {
                backColor = Palette.YellowSoft;
                textColor = Palette.YellowText;
            }
            else
            {
                backColor = Palette.GreenSoft;
                textColor = Palette.GreenText;
            }
        }

        // blue top bar: the "P" sign and the page tabs
        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {
            int signSize = ScaleForScreen(34);
            GraphicsHelper.DrawParkingSign(e.Graphics,
                new Rectangle(ScaleForScreen(24), (pnlHeader.Height - signSize) / 2, signSize, signSize), signFont);
            DrawPageTabs(e.Graphics);
        }

        // five summary cards side by side, each with a round icon, a number and a caption
        private void DrawSummaryCards(Graphics graphics, Rectangle area)
        {
            string[] captions = { "Total permits", "Active", "Ending within 30 days", "Awaiting payment", "Expired" };
            int[] values = { totalCount, activeCount, expiringSoonCount, awaitingPaymentCount, expiredCount };
            Color[] colors = { Palette.Accent, Palette.Green, Palette.Yellow, Palette.Orange, Palette.Red };
            LineIcon[] icons = { LineIcon.List, LineIcon.Check, LineIcon.Clock, LineIcon.Card, LineIcon.Cross };

            int gap = ScaleForScreen(16);
            int tileWidth = (area.Width - gap * (values.Length - 1)) / values.Length;

            for (int i = 0; i < values.Length; i++)
            {
                Rectangle tile = new Rectangle(area.X + i * (tileWidth + gap), area.Y, tileWidth - 1, area.Height - 1);
                GraphicsHelper.DrawRoundedCard(graphics, tile, 12);

                // a soft circle in the card's colour with the icon in the middle
                int circleSize = ScaleForScreen(44);
                Rectangle circle = new Rectangle(tile.X + ScaleForScreen(18), tile.Y + (tile.Height - circleSize) / 2, circleSize, circleSize);
                using (SolidBrush circleBrush = new SolidBrush(Color.FromArgb(32, colors[i])))
                {
                    graphics.FillEllipse(circleBrush, circle);
                }
                GraphicsHelper.DrawIcon(graphics, icons[i],
                    new Rectangle(circle.X + (circleSize - 18) / 2, circle.Y + (circleSize - 18) / 2, 18, 18), colors[i]);

                // the number and caption, centred vertically next to the circle
                int textLeft = circle.Right + ScaleForScreen(14);
                int textTop = tile.Y + (tile.Height - tileValueFont.Height - tileCaptionFont.Height) / 2;
                TextRenderer.DrawText(graphics, values[i].ToString(), tileValueFont,
                    new Point(textLeft, textTop), Palette.Text);
                TextRenderer.DrawText(graphics, captions[i], tileCaptionFont,
                    new Rectangle(textLeft + 2, textTop + tileValueFont.Height, tile.Right - textLeft - 8, tileCaptionFont.Height), Palette.Muted,
                    TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
            }
        }

        // rounded input fields behind the search box and the two drop-down boxes
        private void pnlList_Paint(object sender, PaintEventArgs e)
        {
            // the search field lines up with the drop-down fields and has a magnifier on the left
            int fieldTop = cmbPermitType.Top - 6;
            int fieldHeight = cmbPermitType.Height + 12;
            Rectangle searchField = new Rectangle(txtSearch.Left - 34, fieldTop, txtSearch.Width + 46, fieldHeight);
            GraphicsHelper.DrawField(e.Graphics, searchField);
            GraphicsHelper.DrawIcon(e.Graphics, LineIcon.Search,
                new Rectangle(txtSearch.Left - 26, fieldTop + (fieldHeight - 18) / 2, 18, 18), Palette.Faint);

            GraphicsHelper.DrawField(e.Graphics, Rectangle.Inflate(cmbPermitType.Bounds, 6, 6));
            GraphicsHelper.DrawField(e.Graphics, Rectangle.Inflate(cmbSort.Bounds, 6, 6));
        }

        // converts a size meant for a normal (100%) screen to the current screen scaling
        private int ScaleForScreen(int value)
        {
            return (int)Math.Round(value * DeviceDpi / 96f);
        }

        // The right-hand panel: everything about the selected permit.
        // Each part is placed below the one before it, using the real font heights,
        // so the layout stays tidy at any screen scaling.
        private void pnlDetails_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            int margin = ScaleForScreen(24);
            int contentWidth = pnlDetails.Width - margin * 2;
            TextFormatFlags leftText = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
            TextFormatFlags centeredText = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;

            Permit permit = GetSelectedPermit();

            // the teal band at the top: its height depends on what is written on it
            int labelTop = ScaleForScreen(18);
            int plateHeight = ScaleForScreen(60);
            int bandHeight = permit == null
                ? labelTop + sectionFont.Height + ScaleForScreen(18)
                : labelTop + sectionFont.Height + ScaleForScreen(12) + plateHeight + ScaleForScreen(14)
                  + ownerFont.Height + ScaleForScreen(2) + detailFont.Height + ScaleForScreen(18);
            DrawDetailsBand(graphics, bandHeight);

            int y = labelTop;
            TextRenderer.DrawText(graphics, "PERMIT DETAILS", sectionFont,
                new Rectangle(margin, y, contentWidth, sectionFont.Height), DetailsPalette.BandMuted, leftText);
            y += sectionFont.Height + ScaleForScreen(12);

            if (permit == null)
            {
                Rectangle emptyArea = new Rectangle(margin, bandHeight, contentWidth, btnEdit.Top - bandHeight - ScaleForScreen(20));
                TextRenderer.DrawText(graphics, "Select a permit in the list\nto see its details here.", emptyFont,
                    emptyArea, DetailsPalette.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                return;
            }

            Color statusColor = GetStatusColor(permit);

            // number plate (white, so it stands out on the teal band)
            GraphicsHelper.DrawLicensePlate(graphics, new Rectangle(margin, y, contentWidth, plateHeight), permit.PlateNumber, plateFont, false);
            y += plateHeight + ScaleForScreen(14);

            // owner, then type and zone
            TextRenderer.DrawText(graphics, permit.OwnerName, ownerFont,
                new Rectangle(margin, y, contentWidth, ownerFont.Height), DetailsPalette.BandText, leftText);
            y += ownerFont.Height + ScaleForScreen(2);
            TextRenderer.DrawText(graphics, permit.PermitType + " permit   ·   Zone " + permit.Zone, detailFont,
                new Rectangle(margin, y, contentWidth, detailFont.Height), DetailsPalette.BandMuted, leftText);
            y = bandHeight + ScaleForScreen(16);

            // status badge (DrawBadge starts the pill 8 pixels inside the area it is given)
            int badgeHeight = bigBadgeFont.Height + ScaleForScreen(10);
            GetStatusPillColors(permit, out Color pillBack, out Color pillText);
            GraphicsHelper.DrawBadge(graphics, new Rectangle(margin - 8, y, contentWidth, badgeHeight),
                permit.Validity, bigBadgeFont, pillBack, pillText, statusColor);
            int badgeBottom = y + badgeHeight;

            // key facts: three rows sitting just above the Edit / Delete buttons
            int rowHeight = detailFont.Height + ScaleForScreen(14);
            int factsTop = btnEdit.Top - ScaleForScreen(20) - rowHeight * 3;
            DrawInfoRow(graphics, factsTop, rowHeight, "Valid until", permit.ValidUntil.ToString("dd MMM yyyy"), DetailsPalette.Text);
            DrawInfoRow(graphics, factsTop + rowHeight, rowHeight, "Payment", permit.Payment,
                permit.IsPaid ? Palette.GreenText : Palette.OrangeText);
            DrawInfoRow(graphics, factsTop + rowHeight * 2, rowHeight, "Permit type", permit.PermitType, DetailsPalette.Text);

            // ring in the space between the badge and the facts:
            // how much of a year is left (a full ring = a year or more)
            int space = factsTop - badgeBottom;
            int radius = Math.Min(ScaleForScreen(60), space / 2 - ScaleForScreen(14));
            if (radius < ScaleForScreen(30))
                return;   // window too short for the ring

            Point center = new Point(pnlDetails.Width / 2, badgeBottom + space / 2);
            float fraction = permit.IsActive ? Math.Min(permit.DaysLeft / 365f, 1f) : 0f;
            GraphicsHelper.DrawProgressRing(graphics, center, radius, ScaleForScreen(10), fraction, statusColor, DetailsPalette.RingTrack);

            int numberHeight = ringNumberFont.Height;
            int labelHeight = detailFont.Height;
            int textTop = center.Y - (numberHeight + labelHeight) / 2;
            TextRenderer.DrawText(graphics, permit.DaysLeft.ToString(), ringNumberFont,
                new Rectangle(center.X - radius, textTop, radius * 2, numberHeight), DetailsPalette.Text, centeredText);
            TextRenderer.DrawText(graphics, permit.IsActive ? "days left" : "not active", detailFont,
                new Rectangle(center.X - radius, textTop + numberHeight, radius * 2, labelHeight), DetailsPalette.Muted, centeredText);
        }

        // the teal band across the top of the details card, following its rounded top corners
        private void DrawDetailsBand(Graphics graphics, int bandHeight)
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle card = new Rectangle(0, 0, pnlDetails.Width - 1, pnlDetails.Height - 1);
            Rectangle band = new Rectangle(0, 0, pnlDetails.Width, bandHeight);
            using (GraphicsPath cardPath = GraphicsHelper.CreateRoundedRectangle(card, 12))
            using (LinearGradientBrush bandBrush = new LinearGradientBrush(band,
                DetailsPalette.BandTop, DetailsPalette.BandBottom, LinearGradientMode.Vertical))
            {
                graphics.SetClip(cardPath);
                graphics.FillRectangle(bandBrush, band);
                graphics.ResetClip();
            }
        }

        // one "label ........ value" line in the details panel, with a thin line above it
        private void DrawInfoRow(Graphics graphics, int y, int height, string label, string value, Color valueColor)
        {
            int margin = ScaleForScreen(24);
            using (Pen linePen = new Pen(DetailsPalette.Divider))
            {
                graphics.DrawLine(linePen, margin, y, pnlDetails.Width - margin, y);
            }
            Rectangle row = new Rectangle(margin, y, pnlDetails.Width - margin * 2, height);
            TextRenderer.DrawText(graphics, label, detailFont, row, DetailsPalette.Muted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            TextRenderer.DrawText(graphics, value, detailValueFont, row, valueColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        // the visual rule: each row is tinted with the colour of its status
        private void dgvPermits_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvPermits.Rows.Count)
                return;

            Permit permit = dgvPermits.Rows[e.RowIndex].DataBoundItem as Permit;
            if (permit == null)
                return;

            if (permit.IsExpired)
            {
                e.CellStyle.BackColor = Palette.RowExpired;
                e.CellStyle.SelectionBackColor = Palette.RowExpiredSelected;
            }
            else if (!permit.IsPaid)
            {
                e.CellStyle.BackColor = Palette.RowAwaiting;
                e.CellStyle.SelectionBackColor = Palette.RowAwaitingSelected;
            }
            else if (permit.ExpiresSoon)
            {
                e.CellStyle.BackColor = Palette.RowExpiresSoon;
                e.CellStyle.SelectionBackColor = Palette.RowExpiresSoonSelected;
            }
            else
            {
                e.CellStyle.SelectionBackColor = Palette.RowValidSelected;
            }

            // a permit that cannot be used shows its 0 days left in grey
            if (!permit.IsActive && dgvPermits.Columns[e.ColumnIndex] == colDaysLeft)
                e.CellStyle.ForeColor = Palette.Muted;
        }

        // Extra drawing on top of the grid:
        //  - a thick status-coloured stripe at the start of every row (in the Plate cell)
        //  - the Status cell as a solid coloured badge
        //  - the Payment cell as an outlined badge
        private void dgvPermits_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;
            if (e.ColumnIndex != colPlateNumber.Index && e.ColumnIndex != colPayment.Index && e.ColumnIndex != colValidity.Index)
                return;

            Permit permit = dgvPermits.Rows[e.RowIndex].DataBoundItem as Permit;
            if (permit == null)
                return;

            Color statusColor = GetStatusColor(permit);

            if (e.ColumnIndex == colPlateNumber.Index)
            {
                // the grid draws the cell as usual, then the stripe goes over its left edge
                e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                using (SolidBrush stripeBrush = new SolidBrush(statusColor))
                {
                    e.Graphics.FillRectangle(stripeBrush, e.CellBounds.X, e.CellBounds.Y, ScaleForScreen(6), e.CellBounds.Height - 1);
                }
                e.Handled = true;
                return;
            }

            // let the grid paint the background and line, then draw the badge on top
            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.SelectionBackground | DataGridViewPaintParts.Border);

            if (e.ColumnIndex == colValidity.Index)
            {
                GetStatusPillColors(permit, out Color pillBack, out Color pillText);
                GraphicsHelper.DrawBadge(e.Graphics, e.CellBounds, permit.Validity, badgeFont, pillBack, pillText, statusColor);
            }
            else if (permit.IsPaid)
            {
                GraphicsHelper.DrawBadge(e.Graphics, e.CellBounds, permit.Payment, badgeFont,
                    Palette.GreenSoft, Palette.GreenText, Palette.Green);
            }
            else
            {
                GraphicsHelper.DrawBadge(e.Graphics, e.CellBounds, permit.Payment, badgeFont,
                    Color.White, Palette.OrangeText, Palette.Orange);
            }
            e.Handled = true;
        }

        // when nothing matches, write a message in the middle of the empty grid
        private void dgvPermits_Paint(object sender, PaintEventArgs e)
        {
            if (dgvPermits.Rows.Count > 0)
                return;

            Rectangle area = new Rectangle(0, dgvPermits.ColumnHeadersHeight,
                dgvPermits.Width, dgvPermits.Height - dgvPermits.ColumnHeadersHeight);
            TextRenderer.DrawText(e.Graphics, "No permits match your search.", emptyFont, area, Palette.Muted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // the colour key under the grid
        private void pnlLegend_Paint(object sender, PaintEventArgs e)
        {
            string[] names = { "Valid", "Expires soon", "Awaiting payment", "Expired" };
            Color[] colors = { Palette.Green, Palette.Yellow, Palette.Orange, Palette.Red };

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int x = 0;
            for (int i = 0; i < names.Length; i++)
            {
                Rectangle swatch = new Rectangle(x, (pnlLegend.Height - 12) / 2, 12, 12);
                using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(swatch, 3))
                using (SolidBrush brush = new SolidBrush(colors[i]))
                {
                    e.Graphics.FillPath(brush, path);
                }

                Size textSize = TextRenderer.MeasureText(names[i], pnlLegend.Font);
                TextRenderer.DrawText(e.Graphics, names[i], pnlLegend.Font,
                    new Point(x + 18, (pnlLegend.Height - textSize.Height) / 2), Palette.Muted);
                x += 18 + textSize.Width + 18;
            }
        }
    }
}
