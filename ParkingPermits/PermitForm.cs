using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;

namespace ParkingPermits
{
    public partial class PermitForm : Form
    {
        // Georgian plate format, for example AB-123-CD
        private static readonly Regex platePattern = new Regex(@"^[A-Z]{2}-\d{3}-[A-Z]{2}$");

        // full name in English letters: at least a first and a last name, for example Nino Beridze
        // (a hyphen or apostrophe inside a name is allowed: Anna-Maria, O'Brien)
        private static readonly Regex fullNamePattern = new Regex(@"^[A-Za-z]+(['-][A-Za-z]+)*( [A-Za-z]+(['-][A-Za-z]+)*)+$");

        private readonly PermitRepository repository = new PermitRepository();
        private int permitId;
        private bool wasPaid;

        // spots in the chosen zone taken by other permits; -1 when it could not be counted
        private int takenInZone = -1;

        // fonts for the drawing code, created once and reused on every repaint
        private readonly Font signFont = new Font("Segoe UI", 17F, FontStyle.Bold);
        private readonly Font brandFont = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
        private readonly Font smallFont = new Font("Segoe UI", 9F);
        private readonly Font sectionFont = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
        private readonly Font hintFont = new Font("Segoe UI", 8.5F);
        private readonly Font keyFont = new Font("Segoe UI Semibold", 7.5F, FontStyle.Bold);
        private readonly Font plateFont = new Font("Segoe UI", 16F, FontStyle.Bold);
        private readonly Font ownerFont = new Font("Segoe UI Semibold", 13F, FontStyle.Bold);
        private readonly Font detailFont = new Font("Segoe UI", 9.5F);
        private readonly Font detailValueFont = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        private readonly Font badgeFont = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);

        public Permit CurrentPermit { get; private set; }

        // Add: an empty dialog
        public PermitForm()
        {
            InitializeComponent();
            Text = "Add permit";
            lblHeading.Text = "New parking permit";
            lblHint.Text = "Fill in the details. The pass on the left updates as you type.";
            btnSave.Text = "Add permit";

            // the date box sits inside a slightly smaller panel, which hides its square border,
            // so the rounded field drawn around it is the only frame
            pnlDateField.Size = new Size(dtpValidUntil.Width - 4, dtpValidUntil.Height - 4);

            cmbPermitType.SelectedIndex = 0;
            cmbZone.SelectedIndex = 0;
            dtpValidUntil.Value = DateTime.Today.AddMonths(6);
            UpdatePaymentCard();
        }

        // Edit: : this() runs the constructor above first, so the controls exist
        public PermitForm(Permit permit) : this()
        {
            Text = "Edit permit";
            lblHeading.Text = "Edit permit";
            lblHint.Text = "Change what you need, then press Save changes.";
            btnSave.Text = "Save changes";
            permitId = permit.Id;
            txtPlateNumber.Text = permit.PlateNumber;
            txtOwnerName.Text = permit.OwnerName;
            cmbPermitType.SelectedItem = permit.PermitType;
            cmbZone.SelectedItem = permit.Zone;
            dtpValidUntil.Value = permit.ValidUntil;
            chkIsPaid.Checked = permit.IsPaid;

            // a paid permit cannot be made unpaid again (the database refuses it too)
            wasPaid = permit.IsPaid;
            if (permit.IsPaid)
                chkIsPaid.Enabled = false;

            // count the zone again, now that this permit's own Id is known
            UpdateZoneSpots();
            UpdatePaymentCard();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string plateNumber = txtPlateNumber.Text.Trim().ToUpper();
            string ownerName = txtOwnerName.Text.Trim();

            if (string.IsNullOrWhiteSpace(plateNumber))
            {
                ShowError("Enter the plate number.", txtPlateNumber);
                return;
            }
            if (!platePattern.IsMatch(plateNumber))
            {
                ShowError("The plate number must look like AB-123-CD: two letters, three digits, two letters.", txtPlateNumber);
                return;
            }
            if (string.IsNullOrWhiteSpace(ownerName))
            {
                ShowError("Enter the owner's name.", txtOwnerName);
                return;
            }
            if (!fullNamePattern.IsMatch(ownerName))
            {
                ShowError("Enter the owner's full name in English letters: first name and last name, for example Nino Beridze.", txtOwnerName);
                return;
            }
            if (cmbPermitType.SelectedIndex < 0)
            {
                ShowError("Choose a permit type.", cmbPermitType);
                return;
            }
            if (cmbZone.SelectedIndex < 0)
            {
                ShowError("Choose a zone.", cmbZone);
                return;
            }
            // a new permit must still be usable; an existing one may be kept with its old (past) date
            if (permitId == 0 && dtpValidUntil.Value.Date < DateTime.Today)
            {
                ShowError("A new permit cannot end in the past.", dtpValidUntil);
                return;
            }
            if (dtpValidUntil.Value.Date > DateTime.Today.AddYears(2))
            {
                ShowError("A permit cannot be valid for more than two years from today.", dtpValidUntil);
                return;
            }
            if (repository.PlateExists(plateNumber, permitId))
            {
                ShowError("Another permit already has the plate number " + plateNumber + ".", txtPlateNumber);
                return;
            }
            // a permit that has not expired needs a free spot in its zone
            if (dtpValidUntil.Value.Date >= DateTime.Today)
            {
                string zone = cmbZone.Text;
                int takenSpots = repository.CountTakenSpots(zone, permitId);
                if (takenSpots >= ParkingLot.SpotsPerZone)
                {
                    ShowError("Zone " + zone + " is full (" + takenSpots + " of " + ParkingLot.SpotsPerZone +
                        " spots taken). Choose another zone.", cmbZone);
                    return;
                }
            }

            CurrentPermit = new Permit();
            CurrentPermit.Id = permitId;
            CurrentPermit.PlateNumber = plateNumber;
            CurrentPermit.OwnerName = ownerName;
            CurrentPermit.PermitType = cmbPermitType.Text;
            CurrentPermit.Zone = cmbZone.Text;
            CurrentPermit.ValidUntil = dtpValidUntil.Value.Date;
            CurrentPermit.IsPaid = chkIsPaid.Checked;

            DialogResult = DialogResult.OK;
        }

        // shows what is wrong and keeps the dialog open with what the user typed
        private void ShowError(string message, Control control)
        {
            MessageBox.Show(message, "Check the permit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            control.Focus();
        }

        // ---------- keeping the preview and hints up to date ----------

        // any field changed: redraw the pass preview and the hints under the fields
        private void Field_ValueChanged(object sender, EventArgs e)
        {
            pnlPreview.Invalidate();
            Invalidate();
        }

        // a field got or lost the focus: redraw it with or without the blue focus frame
        private void Field_FocusChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void cmbZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateZoneSpots();
            Field_ValueChanged(sender, e);
        }

        private void chkIsPaid_CheckedChanged(object sender, EventArgs e)
        {
            UpdatePaymentCard();
            Field_ValueChanged(sender, e);
        }

        // counts how many spots of the chosen zone other permits already take
        private void UpdateZoneSpots()
        {
            takenInZone = -1;
            if (cmbZone.SelectedIndex < 0)
                return;
            try
            {
                takenInZone = repository.CountTakenSpots(cmbZone.Text, permitId);
            }
            catch (Exception)
            {
                // no database: the dialog still works, it just does not show the free spots
                takenInZone = -1;
            }
        }

        // the payment card turns green when the fee is paid; the check box takes the card's colour
        private void UpdatePaymentCard()
        {
            chkIsPaid.BackColor = chkIsPaid.Checked ? PaidCardColor : UnpaidCardColor;
        }

        private static readonly Color PaidCardColor = Color.FromArgb(236, 250, 242);
        private static readonly Color UnpaidCardColor = Color.FromArgb(246, 249, 253);

        // the three quick buttons under the date: valid for a period starting today
        private void btnOneMonth_Click(object sender, EventArgs e)
        {
            dtpValidUntil.Value = DateTime.Today.AddMonths(1);
        }

        private void btnSixMonths_Click(object sender, EventArgs e)
        {
            dtpValidUntil.Value = DateTime.Today.AddMonths(6);
        }

        private void btnOneYear_Click(object sender, EventArgs e)
        {
            dtpValidUntil.Value = DateTime.Today.AddYears(1);
        }

        // the permit as it would be saved right now, for the preview (not checked yet)
        private Permit BuildPreviewPermit()
        {
            Permit permit = new Permit();
            permit.Id = permitId;
            permit.PlateNumber = txtPlateNumber.Text.Trim().ToUpper();
            permit.OwnerName = txtOwnerName.Text.Trim();
            permit.PermitType = cmbPermitType.Text;
            permit.Zone = cmbZone.Text;
            permit.ValidUntil = dtpValidUntil.Value.Date;
            permit.IsPaid = chkIsPaid.Checked;
            return permit;
        }

        // converts a size meant for a normal (100%) screen to the current screen scaling
        private int ScaleForScreen(int value)
        {
            return (int)Math.Round(value * DeviceDpi / 96f);
        }

        // ---------- drawing: the live pass preview on the left ----------

        private void pnlPreview_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int width = pnlPreview.Width;
            int height = pnlPreview.Height;
            int margin = ScaleForScreen(28);

            // background: ocean blue, a little deeper at the bottom, with two soft circles
            using (LinearGradientBrush background = new LinearGradientBrush(pnlPreview.ClientRectangle,
                Palette.Header, Color.FromArgb(20, 62, 122), LinearGradientMode.Vertical))
            {
                graphics.FillRectangle(background, pnlPreview.ClientRectangle);
            }
            using (SolidBrush circleBrush = new SolidBrush(Color.FromArgb(16, Color.White)))
            {
                graphics.FillEllipse(circleBrush, width - ScaleForScreen(130), -ScaleForScreen(70), ScaleForScreen(240), ScaleForScreen(240));
                graphics.FillEllipse(circleBrush, -ScaleForScreen(90), height - ScaleForScreen(200), ScaleForScreen(260), ScaleForScreen(260));
            }

            // brand: the "P" sign, the app name and "Live preview"
            int signSize = ScaleForScreen(36);
            GraphicsHelper.DrawParkingSign(graphics, new Rectangle(margin, margin, signSize, signSize), signFont);
            int brandLeft = margin + signSize + ScaleForScreen(12);
            int brandTop = margin + (signSize - brandFont.Height - smallFont.Height) / 2;
            TextRenderer.DrawText(graphics, "Parking Permits", brandFont, new Point(brandLeft, brandTop), Palette.HeaderText);
            TextRenderer.DrawText(graphics, "Live preview", smallFont, new Point(brandLeft, brandTop + brandFont.Height), Palette.HeaderMuted);

            Permit permit = BuildPreviewPermit();
            Rectangle card = DrawPassCard(graphics, permit, new Rectangle(margin, margin + signSize + ScaleForScreen(32), width - margin * 2, 0));

            // a short note under the card
            TextRenderer.DrawText(graphics, "This is how the permit will look\nin the list after you save it.", smallFont,
                new Rectangle(margin, card.Bottom + ScaleForScreen(18), width - margin * 2, smallFont.Height * 2 + 4),
                Palette.HeaderMuted, TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding);
        }

        // The parking pass: a teal band with the plate and the owner, and a mint body
        // with the status and the key facts. Returns the card's bounds.
        private Rectangle DrawPassCard(Graphics graphics, Permit permit, Rectangle area)
        {
            int padding = ScaleForScreen(18);
            int plateHeight = ScaleForScreen(50);
            int rowHeight = detailFont.Height + ScaleForScreen(14);
            int badgeHeight = badgeFont.Height + ScaleForScreen(12);

            int bandHeight = padding + sectionFont.Height + ScaleForScreen(10) + plateHeight + ScaleForScreen(14)
                + ownerFont.Height + ScaleForScreen(2) + detailFont.Height + padding;
            int bodyHeight = ScaleForScreen(14) + badgeHeight + ScaleForScreen(10) + rowHeight * 3 + ScaleForScreen(6);
            Rectangle card = new Rectangle(area.X, area.Y, area.Width, bandHeight + bodyHeight);
            int radius = ScaleForScreen(14);

            // soft shadow, then the mint card, then the teal band clipped to the card's rounded top
            using (GraphicsPath shadowPath = GraphicsHelper.CreateRoundedRectangle(
                new Rectangle(card.X, card.Y + ScaleForScreen(6), card.Width, card.Height), radius))
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(45, 8, 30, 60)))
            {
                graphics.FillPath(shadowBrush, shadowPath);
            }
            using (GraphicsPath cardPath = GraphicsHelper.CreateRoundedRectangle(card, radius))
            using (SolidBrush bodyBrush = new SolidBrush(DetailsPalette.Body))
            {
                graphics.FillPath(bodyBrush, cardPath);
                Rectangle band = new Rectangle(card.X, card.Y, card.Width, bandHeight);
                using (LinearGradientBrush bandBrush = new LinearGradientBrush(band,
                    DetailsPalette.BandTop, DetailsPalette.BandBottom, LinearGradientMode.Vertical))
                {
                    graphics.SetClip(cardPath);
                    graphics.FillRectangle(bandBrush, band);
                    graphics.ResetClip();
                }
            }

            TextFormatFlags leftText = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding;
            TextFormatFlags rightText = TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;
            int left = card.X + padding;
            int innerWidth = card.Width - padding * 2;

            // band: label and permit number, plate, owner, type and zone
            int y = card.Y + padding;
            Rectangle labelRow = new Rectangle(left, y, innerWidth, sectionFont.Height);
            TextRenderer.DrawText(graphics, "PARKING PASS", sectionFont, labelRow, DetailsPalette.BandMuted, leftText);
            TextRenderer.DrawText(graphics, permitId == 0 ? "NEW" : "No. " + permitId.ToString("000"), sectionFont,
                labelRow, DetailsPalette.BandMuted, rightText);
            y += sectionFont.Height + ScaleForScreen(10);

            bool noPlate = permit.PlateNumber.Length == 0;
            GraphicsHelper.DrawLicensePlate(graphics, new Rectangle(left, y, innerWidth, plateHeight),
                noPlate ? "AB-123-CD" : permit.PlateNumber, plateFont, noPlate);
            y += plateHeight + ScaleForScreen(14);

            bool noOwner = permit.OwnerName.Length == 0;
            TextRenderer.DrawText(graphics, noOwner ? "Owner's full name" : permit.OwnerName, ownerFont,
                new Rectangle(left, y, innerWidth, ownerFont.Height), noOwner ? DetailsPalette.BandMuted : DetailsPalette.BandText, leftText);
            y += ownerFont.Height + ScaleForScreen(2);
            TextRenderer.DrawText(graphics, permit.PermitType + " permit   ·   Zone " + permit.Zone, detailFont,
                new Rectangle(left, y, innerWidth, detailFont.Height), DetailsPalette.BandMuted, leftText);

            // body: status pill, then three facts
            y = card.Y + bandHeight + ScaleForScreen(14);
            MainForm.GetStatusPillColors(permit, out Color pillBack, out Color pillText);
            GraphicsHelper.DrawBadge(graphics, new Rectangle(left - 8, y, innerWidth, badgeHeight),
                permit.Validity, badgeFont, pillBack, pillText, MainForm.GetStatusColor(permit));
            y += badgeHeight + ScaleForScreen(10);

            DrawPassRow(graphics, left, y, innerWidth, rowHeight, "Valid until", permit.ValidUntil.ToString("dd MMM yyyy"), DetailsPalette.Text);
            DrawPassRow(graphics, left, y + rowHeight, innerWidth, rowHeight, "Payment", permit.Payment,
                permit.IsPaid ? Palette.GreenText : Palette.OrangeText);
            DrawPassRow(graphics, left, y + rowHeight * 2, innerWidth, rowHeight, "Days left",
                permit.IsActive ? permit.DaysLeft.ToString() : "0  (not active)", DetailsPalette.Text);

            return card;
        }

        // one "label ........ value" line on the pass, with a thin line above it
        private void DrawPassRow(Graphics graphics, int x, int y, int width, int height, string label, string value, Color valueColor)
        {
            using (Pen linePen = new Pen(DetailsPalette.Divider))
            {
                graphics.DrawLine(linePen, x, y, x + width, y);
            }
            Rectangle row = new Rectangle(x, y, width, height);
            TextRenderer.DrawText(graphics, label, detailFont, row, DetailsPalette.Muted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            TextRenderer.DrawText(graphics, value, detailValueFont, row, valueColor,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        // ---------- drawing: the form on the right ----------

        private void PermitForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int fieldHeight = cmbPermitType.Height + ScaleForScreen(12);

            // the three numbered sections
            DrawSectionTitle(graphics, 1, "VEHICLE AND OWNER", lblPlateNumber.Top);
            DrawSectionTitle(graphics, 2, "PERMIT", lblPermitType.Top);
            DrawSectionTitle(graphics, 3, "VALIDITY AND PAYMENT", lblValidUntil.Top);

            // rounded fields behind the controls, all the same height
            Rectangle plateField = TextField(txtPlateNumber, fieldHeight);
            Rectangle ownerField = TextField(txtOwnerName, fieldHeight);
            Rectangle typeField = Rectangle.Inflate(cmbPermitType.Bounds, ScaleForScreen(6), (fieldHeight - cmbPermitType.Height) / 2);
            Rectangle zoneField = Rectangle.Inflate(cmbZone.Bounds, ScaleForScreen(6), (fieldHeight - cmbZone.Height) / 2);
            Rectangle dateField = new Rectangle(typeField.X, pnlDateField.Top - (fieldHeight - pnlDateField.Height) / 2,
                typeField.Width, fieldHeight);

            DrawInputField(graphics, plateField, txtPlateNumber.Focused);
            DrawInputField(graphics, ownerField, txtOwnerName.Focused);
            DrawInputField(graphics, typeField, cmbPermitType.Focused);
            DrawInputField(graphics, zoneField, cmbZone.Focused);
            DrawInputField(graphics, dateField, dtpValidUntil.Focused);

            // hints under the fields, green when the value is right
            string plate = txtPlateNumber.Text.Trim().ToUpper();
            if (plate.Length == 0)
                DrawHint(graphics, plateField, "Format: AB-123-CD", Palette.Muted);
            else if (platePattern.IsMatch(plate))
                DrawHint(graphics, plateField, "✓  Valid plate number", Palette.GreenText);
            else
                DrawHint(graphics, plateField, "Use two letters, three digits, two letters", Palette.OrangeText);

            string owner = txtOwnerName.Text.Trim();
            if (owner.Length == 0)
                DrawHint(graphics, ownerField, "First and last name, English letters", Palette.Muted);
            else if (fullNamePattern.IsMatch(owner))
                DrawHint(graphics, ownerField, "✓  Looks good", Palette.GreenText);
            else
                DrawHint(graphics, ownerField, "Enter a first and a last name in English", Palette.OrangeText);

            DrawHint(graphics, typeField, GetPermitTypeHint(cmbPermitType.Text), Palette.Muted);
            DrawZoneSpots(graphics, zoneField);
            DrawDaysUntil(graphics, dateField);
            DrawPaymentCard(graphics, new Rectangle(zoneField.X, lblValidUntil.Top,
                ownerField.Right - zoneField.X, btnOneYear.Bottom - lblValidUntil.Top));
        }

        // a borderless TextBox gets a field of the given height around it
        private Rectangle TextField(TextBox textBox, int fieldHeight)
        {
            int sidePadding = ScaleForScreen(12);
            return new Rectangle(textBox.Left - sidePadding, textBox.Top - (fieldHeight - textBox.Height) / 2,
                textBox.Width + sidePadding * 2, fieldHeight);
        }

        // a white rounded field; the field with the focus gets a blue frame and a soft glow
        private void DrawInputField(Graphics graphics, Rectangle field, bool focused)
        {
            int radius = ScaleForScreen(8);
            if (focused)
            {
                using (GraphicsPath glowPath = GraphicsHelper.CreateRoundedRectangle(Rectangle.Inflate(field, 3, 3), radius + 3))
                using (SolidBrush glowBrush = new SolidBrush(Palette.AccentSoft))
                {
                    graphics.FillPath(glowBrush, glowPath);
                }
            }
            using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(field, radius))
            using (SolidBrush fillBrush = new SolidBrush(Color.White))
            using (Pen borderPen = new Pen(focused ? Palette.Accent : Palette.FieldBorder, focused ? 1.5f : 1f))
            {
                graphics.FillPath(fillBrush, path);
                graphics.DrawPath(borderPen, path);
            }
        }

        // a numbered circle, the section name and a thin line to the right edge
        private void DrawSectionTitle(Graphics graphics, int number, string title, int labelTop)
        {
            int circleSize = ScaleForScreen(20);
            int top = labelTop - ScaleForScreen(34);
            int left = lblPlateNumber.Left - 1;
            int right = TextField(txtOwnerName, 0).Right;

            Rectangle circle = new Rectangle(left, top, circleSize, circleSize);
            using (SolidBrush circleBrush = new SolidBrush(Palette.AccentSoft))
            {
                graphics.FillEllipse(circleBrush, circle);
            }
            TextRenderer.DrawText(graphics, number.ToString(), sectionFont, circle, Palette.Accent,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            int textLeft = circle.Right + ScaleForScreen(10);
            Size textSize = TextRenderer.MeasureText(graphics, title, sectionFont, Size.Empty, TextFormatFlags.NoPadding);
            TextRenderer.DrawText(graphics, title, sectionFont,
                new Rectangle(textLeft, top, textSize.Width + 2, circleSize), Palette.Accent,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            int lineY = top + circleSize / 2;
            using (Pen linePen = new Pen(Palette.Border))
            {
                graphics.DrawLine(linePen, textLeft + textSize.Width + ScaleForScreen(12), lineY, right, lineY);
            }
        }

        // a small line of text just under a field
        private void DrawHint(Graphics graphics, Rectangle field, string text, Color color)
        {
            TextRenderer.DrawText(graphics, text, hintFont,
                new Rectangle(field.X + 2, field.Bottom + ScaleForScreen(6), field.Width, hintFont.Height), color,
                TextFormatFlags.Left | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
        }

        private static string GetPermitTypeHint(string permitType)
        {
            if (permitType == "Student")
                return "Ends with a semester or year";
            if (permitType == "Staff")
                return "Ends with the calendar year";
            if (permitType == "Guest")
                return "A short visit of days or weeks";
            return "";
        }

        // under the zone: how many spots are free, with one small block per spot
        private void DrawZoneSpots(Graphics graphics, Rectangle zoneField)
        {
            if (takenInZone < 0)
                return;

            int freeSpots = Math.Max(ParkingLot.SpotsPerZone - takenInZone, 0);
            bool needsSpot = dtpValidUntil.Value.Date >= DateTime.Today;
            string text;
            Color textColor;
            if (!needsSpot)
            {
                text = "An expired permit needs no spot";
                textColor = Palette.Muted;
            }
            else if (freeSpots == 0)
            {
                text = "Zone full, choose another";
                textColor = Palette.RedText;
            }
            else
            {
                text = freeSpots + " of " + ParkingLot.SpotsPerZone + " spots free";
                textColor = Palette.GreenText;
            }
            DrawHint(graphics, zoneField, text, textColor);

            // the blocks, right-aligned under the field: blue = taken by another permit
            int blockWidth = ScaleForScreen(9);
            int blockHeight = ScaleForScreen(9);
            int gap = ScaleForScreen(3);
            int x = zoneField.Right - (blockWidth * ParkingLot.SpotsPerZone + gap * (ParkingLot.SpotsPerZone - 1)) - 2;
            int y = zoneField.Bottom + ScaleForScreen(6) + (hintFont.Height - blockHeight) / 2;
            for (int i = 0; i < ParkingLot.SpotsPerZone; i++)
            {
                Rectangle block = new Rectangle(x + i * (blockWidth + gap), y, blockWidth, blockHeight);
                Color blockColor = i < takenInZone ? (freeSpots == 0 ? Palette.Red : Palette.Accent) : Palette.AccentSoft;
                using (GraphicsPath blockPath = GraphicsHelper.CreateRoundedRectangle(block, 2))
                using (SolidBrush blockBrush = new SolidBrush(blockColor))
                {
                    graphics.FillPath(blockBrush, blockPath);
                }
            }
        }

        // next to the "Valid until" label: how far away the end date is
        private void DrawDaysUntil(Graphics graphics, Rectangle dateField)
        {
            int days = (dtpValidUntil.Value.Date - DateTime.Today).Days;
            string text;
            Color color = Palette.Muted;
            if (days > 1)
                text = "in " + days + " days";
            else if (days == 1)
                text = "tomorrow";
            else if (days == 0)
                text = "ends today";
            else
            {
                text = "ended " + (-days) + (days == -1 ? " day ago" : " days ago");
                color = Palette.RedText;
            }

            Rectangle row = new Rectangle(dateField.X, lblValidUntil.Top, dateField.Width - 2, lblValidUntil.Height);
            TextRenderer.DrawText(graphics, text, hintFont, row, color,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }

        // the card around the "paid" check box, with a sentence that explains the choice
        private void DrawPaymentCard(Graphics graphics, Rectangle card)
        {
            bool paid = chkIsPaid.Checked;
            Color fill = paid ? PaidCardColor : UnpaidCardColor;
            Color border = paid ? Color.FromArgb(168, 222, 190) : Palette.Border;

            using (GraphicsPath path = GraphicsHelper.CreateRoundedRectangle(card, ScaleForScreen(10)))
            using (SolidBrush fillBrush = new SolidBrush(fill))
            using (Pen borderPen = new Pen(border))
            {
                graphics.FillPath(fillBrush, path);
                graphics.DrawPath(borderPen, path);
            }

            GraphicsHelper.DrawIcon(graphics, paid ? LineIcon.Check : LineIcon.Card,
                new Rectangle(card.Right - ScaleForScreen(14) - 18, chkIsPaid.Top + (chkIsPaid.Height - 18) / 2, 18, 18),
                paid ? Palette.Green : Palette.Faint);

            string text;
            if (wasPaid)
                text = "Paid. A paid permit stays paid; this cannot be changed back.";
            else if (paid)
                text = "The permit becomes active. Payment cannot be undone later.";
            else
                text = "Not paid yet. The permit waits for payment and is not active.";

            int textLeft = chkIsPaid.Left + ScaleForScreen(2);
            Rectangle textArea = new Rectangle(textLeft, chkIsPaid.Bottom + ScaleForScreen(6),
                card.Right - ScaleForScreen(14) - textLeft, card.Bottom - chkIsPaid.Bottom - ScaleForScreen(12));
            TextRenderer.DrawText(graphics, text, hintFont, textArea, paid ? Palette.GreenText : Palette.Muted,
                TextFormatFlags.Left | TextFormatFlags.WordBreak | TextFormatFlags.NoPadding);
        }

        // footer: a thin line on top and the keyboard shortcuts on the left
        private void pnlFooter_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (Pen linePen = new Pen(Palette.Border))
            {
                graphics.DrawLine(linePen, 0, 0, pnlFooter.Width, 0);
            }

            int x = lblPlateNumber.Left - pnlFooter.Left;
            int centerY = btnSave.Top + btnSave.Height / 2;
            x = DrawKeyHint(graphics, x, centerY, "Enter", "save");
            DrawKeyHint(graphics, x + ScaleForScreen(14), centerY, "Esc", "cancel");
        }

        // a small key cap ("Enter") followed by what it does ("save"); returns where the text ends
        private int DrawKeyHint(Graphics graphics, int x, int centerY, string key, string action)
        {
            Size keySize = TextRenderer.MeasureText(graphics, key, keyFont, Size.Empty, TextFormatFlags.NoPadding);
            Rectangle cap = new Rectangle(x, centerY - (keySize.Height + ScaleForScreen(8)) / 2,
                keySize.Width + ScaleForScreen(12), keySize.Height + ScaleForScreen(8));
            using (GraphicsPath capPath = GraphicsHelper.CreateRoundedRectangle(cap, ScaleForScreen(4)))
            using (SolidBrush capBrush = new SolidBrush(Color.FromArgb(244, 247, 251)))
            using (Pen capPen = new Pen(Palette.FieldBorder))
            {
                graphics.FillPath(capBrush, capPath);
                graphics.DrawPath(capPen, capPath);
            }
            TextRenderer.DrawText(graphics, key, keyFont, cap, Palette.Muted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            int textLeft = cap.Right + ScaleForScreen(6);
            Size actionSize = TextRenderer.MeasureText(graphics, action, hintFont, Size.Empty, TextFormatFlags.NoPadding);
            TextRenderer.DrawText(graphics, action, hintFont,
                new Rectangle(textLeft, centerY - actionSize.Height / 2, actionSize.Width + 2, actionSize.Height),
                Palette.Faint, TextFormatFlags.Left | TextFormatFlags.NoPadding);
            return textLeft + actionSize.Width;
        }
    }
}
