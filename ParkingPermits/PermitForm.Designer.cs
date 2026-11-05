namespace ParkingPermits
{
    partial class PermitForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlPreview = new BufferedPanel();
            lblHeading = new Label();
            lblHint = new Label();
            lblPlateNumber = new Label();
            txtPlateNumber = new TextBox();
            lblOwnerName = new Label();
            txtOwnerName = new TextBox();
            lblPermitType = new Label();
            cmbPermitType = new ComboBox();
            lblZone = new Label();
            cmbZone = new ComboBox();
            lblValidUntil = new Label();
            pnlDateField = new Panel();
            dtpValidUntil = new DateTimePicker();
            btnOneMonth = new RoundedButton();
            btnSixMonths = new RoundedButton();
            btnOneYear = new RoundedButton();
            chkIsPaid = new CheckBox();
            pnlFooter = new BufferedPanel();
            btnSave = new RoundedButton();
            btnCancel = new RoundedButton();
            tipQuickDate = new ToolTip(components);
            pnlDateField.SuspendLayout();
            pnlFooter.SuspendLayout();
            SuspendLayout();
            //
            // pnlPreview
            //
            pnlPreview.BackColor = Color.FromArgb(30, 90, 168);
            pnlPreview.Dock = DockStyle.Left;
            pnlPreview.Location = new Point(0, 0);
            pnlPreview.Name = "pnlPreview";
            pnlPreview.Size = new Size(300, 556);
            pnlPreview.TabIndex = 0;
            pnlPreview.Paint += pnlPreview_Paint;
            //
            // lblHeading
            //
            lblHeading.AutoSize = true;
            lblHeading.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblHeading.ForeColor = Color.FromArgb(27, 42, 65);
            lblHeading.Location = new Point(328, 24);
            lblHeading.Name = "lblHeading";
            lblHeading.Size = new Size(80, 30);
            lblHeading.TabIndex = 1;
            lblHeading.Text = "Permit";
            //
            // lblHint
            //
            lblHint.AutoSize = true;
            lblHint.Font = new Font("Segoe UI", 9.75F);
            lblHint.ForeColor = Color.FromArgb(91, 107, 130);
            lblHint.Location = new Point(331, 58);
            lblHint.Name = "lblHint";
            lblHint.Size = new Size(200, 17);
            lblHint.TabIndex = 2;
            lblHint.Text = "Fields marked * are required.";
            //
            // lblPlateNumber
            //
            lblPlateNumber.AutoSize = true;
            lblPlateNumber.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblPlateNumber.ForeColor = Color.FromArgb(52, 68, 92);
            lblPlateNumber.Location = new Point(331, 124);
            lblPlateNumber.Name = "lblPlateNumber";
            lblPlateNumber.Size = new Size(88, 15);
            lblPlateNumber.TabIndex = 3;
            lblPlateNumber.Text = "Plate number *";
            //
            // txtPlateNumber
            //
            txtPlateNumber.BorderStyle = BorderStyle.None;
            txtPlateNumber.CharacterCasing = CharacterCasing.Upper;
            txtPlateNumber.Font = new Font("Segoe UI", 11F);
            txtPlateNumber.Location = new Point(344, 155);
            txtPlateNumber.MaxLength = 20;
            txtPlateNumber.Name = "txtPlateNumber";
            txtPlateNumber.PlaceholderText = "AB-123-CD";
            txtPlateNumber.Size = new Size(192, 20);
            txtPlateNumber.TabIndex = 4;
            txtPlateNumber.TextChanged += Field_ValueChanged;
            txtPlateNumber.Enter += Field_FocusChanged;
            txtPlateNumber.Leave += Field_FocusChanged;
            //
            // lblOwnerName
            //
            lblOwnerName.AutoSize = true;
            lblOwnerName.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblOwnerName.ForeColor = Color.FromArgb(52, 68, 92);
            lblOwnerName.Location = new Point(571, 124);
            lblOwnerName.Name = "lblOwnerName";
            lblOwnerName.Size = new Size(106, 15);
            lblOwnerName.TabIndex = 5;
            lblOwnerName.Text = "Owner full name *";
            //
            // txtOwnerName
            //
            txtOwnerName.BorderStyle = BorderStyle.None;
            txtOwnerName.Font = new Font("Segoe UI", 11F);
            txtOwnerName.Location = new Point(584, 155);
            txtOwnerName.MaxLength = 100;
            txtOwnerName.Name = "txtOwnerName";
            txtOwnerName.PlaceholderText = "Nino Beridze";
            txtOwnerName.Size = new Size(192, 20);
            txtOwnerName.TabIndex = 6;
            txtOwnerName.TextChanged += Field_ValueChanged;
            txtOwnerName.Enter += Field_FocusChanged;
            txtOwnerName.Leave += Field_FocusChanged;
            //
            // lblPermitType
            //
            lblPermitType.AutoSize = true;
            lblPermitType.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblPermitType.ForeColor = Color.FromArgb(52, 68, 92);
            lblPermitType.Location = new Point(331, 246);
            lblPermitType.Name = "lblPermitType";
            lblPermitType.Size = new Size(71, 15);
            lblPermitType.TabIndex = 7;
            lblPermitType.Text = "Permit type";
            //
            // cmbPermitType
            //
            cmbPermitType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPermitType.FlatStyle = FlatStyle.Flat;
            cmbPermitType.Font = new Font("Segoe UI", 11F);
            cmbPermitType.FormattingEnabled = true;
            cmbPermitType.Items.AddRange(new object[] { "Student", "Staff", "Guest" });
            cmbPermitType.Location = new Point(338, 274);
            cmbPermitType.Name = "cmbPermitType";
            cmbPermitType.Size = new Size(204, 28);
            cmbPermitType.TabIndex = 8;
            cmbPermitType.SelectedIndexChanged += Field_ValueChanged;
            cmbPermitType.Enter += Field_FocusChanged;
            cmbPermitType.Leave += Field_FocusChanged;
            //
            // lblZone
            //
            lblZone.AutoSize = true;
            lblZone.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblZone.ForeColor = Color.FromArgb(52, 68, 92);
            lblZone.Location = new Point(571, 246);
            lblZone.Name = "lblZone";
            lblZone.Size = new Size(35, 15);
            lblZone.TabIndex = 9;
            lblZone.Text = "Zone";
            //
            // cmbZone
            //
            cmbZone.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbZone.FlatStyle = FlatStyle.Flat;
            cmbZone.Font = new Font("Segoe UI", 11F);
            cmbZone.FormattingEnabled = true;
            cmbZone.Items.AddRange(new object[] { "A", "B", "C", "D" });
            cmbZone.Location = new Point(578, 274);
            cmbZone.Name = "cmbZone";
            cmbZone.Size = new Size(204, 28);
            cmbZone.TabIndex = 10;
            cmbZone.SelectedIndexChanged += cmbZone_SelectedIndexChanged;
            cmbZone.Enter += Field_FocusChanged;
            cmbZone.Leave += Field_FocusChanged;
            //
            // lblValidUntil
            //
            lblValidUntil.AutoSize = true;
            lblValidUntil.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblValidUntil.ForeColor = Color.FromArgb(52, 68, 92);
            lblValidUntil.Location = new Point(331, 368);
            lblValidUntil.Name = "lblValidUntil";
            lblValidUntil.Size = new Size(64, 15);
            lblValidUntil.TabIndex = 11;
            lblValidUntil.Text = "Valid until";
            //
            // pnlDateField
            //
            pnlDateField.Controls.Add(dtpValidUntil);
            pnlDateField.Location = new Point(338, 396);
            pnlDateField.Name = "pnlDateField";
            pnlDateField.Size = new Size(204, 27);
            pnlDateField.TabIndex = 12;
            //
            // dtpValidUntil
            //
            dtpValidUntil.CustomFormat = "dd MMM yyyy";
            dtpValidUntil.Font = new Font("Segoe UI", 11F);
            dtpValidUntil.Format = DateTimePickerFormat.Custom;
            dtpValidUntil.Location = new Point(-2, -2);
            dtpValidUntil.MinDate = new DateTime(2020, 1, 1, 0, 0, 0, 0);
            dtpValidUntil.Name = "dtpValidUntil";
            dtpValidUntil.Size = new Size(208, 27);
            dtpValidUntil.TabIndex = 0;
            dtpValidUntil.ValueChanged += Field_ValueChanged;
            dtpValidUntil.Enter += Field_FocusChanged;
            dtpValidUntil.Leave += Field_FocusChanged;
            //
            // btnOneMonth
            //
            btnOneMonth.BackColor = Color.FromArgb(236, 243, 252);
            btnOneMonth.FlatAppearance.BorderColor = Color.FromArgb(211, 227, 250);
            btnOneMonth.FlatAppearance.MouseOverBackColor = Color.FromArgb(211, 227, 250);
            btnOneMonth.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            btnOneMonth.ForeColor = Color.FromArgb(29, 78, 186);
            btnOneMonth.Location = new Point(332, 440);
            btnOneMonth.Name = "btnOneMonth";
            btnOneMonth.Size = new Size(66, 28);
            btnOneMonth.TabIndex = 13;
            btnOneMonth.TabStop = false;
            btnOneMonth.Text = "1 month";
            tipQuickDate.SetToolTip(btnOneMonth, "Valid for 1 month from today");
            btnOneMonth.UseVisualStyleBackColor = false;
            btnOneMonth.Click += btnOneMonth_Click;
            //
            // btnSixMonths
            //
            btnSixMonths.BackColor = Color.FromArgb(236, 243, 252);
            btnSixMonths.FlatAppearance.BorderColor = Color.FromArgb(211, 227, 250);
            btnSixMonths.FlatAppearance.MouseOverBackColor = Color.FromArgb(211, 227, 250);
            btnSixMonths.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            btnSixMonths.ForeColor = Color.FromArgb(29, 78, 186);
            btnSixMonths.Location = new Point(407, 440);
            btnSixMonths.Name = "btnSixMonths";
            btnSixMonths.Size = new Size(66, 28);
            btnSixMonths.TabIndex = 14;
            btnSixMonths.TabStop = false;
            btnSixMonths.Text = "6 months";
            tipQuickDate.SetToolTip(btnSixMonths, "Valid for 6 months from today");
            btnSixMonths.UseVisualStyleBackColor = false;
            btnSixMonths.Click += btnSixMonths_Click;
            //
            // btnOneYear
            //
            btnOneYear.BackColor = Color.FromArgb(236, 243, 252);
            btnOneYear.FlatAppearance.BorderColor = Color.FromArgb(211, 227, 250);
            btnOneYear.FlatAppearance.MouseOverBackColor = Color.FromArgb(211, 227, 250);
            btnOneYear.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            btnOneYear.ForeColor = Color.FromArgb(29, 78, 186);
            btnOneYear.Location = new Point(482, 440);
            btnOneYear.Name = "btnOneYear";
            btnOneYear.Size = new Size(66, 28);
            btnOneYear.TabIndex = 15;
            btnOneYear.TabStop = false;
            btnOneYear.Text = "1 year";
            tipQuickDate.SetToolTip(btnOneYear, "Valid for 1 year from today");
            btnOneYear.UseVisualStyleBackColor = false;
            btnOneYear.Click += btnOneYear_Click;
            //
            // chkIsPaid
            //
            chkIsPaid.AutoSize = true;
            chkIsPaid.BackColor = Color.FromArgb(246, 249, 253);
            chkIsPaid.Cursor = Cursors.Hand;
            chkIsPaid.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            chkIsPaid.ForeColor = Color.FromArgb(27, 42, 65);
            chkIsPaid.Location = new Point(588, 382);
            chkIsPaid.Name = "chkIsPaid";
            chkIsPaid.Size = new Size(150, 23);
            chkIsPaid.TabIndex = 16;
            chkIsPaid.Text = "Permit fee is paid";
            chkIsPaid.UseVisualStyleBackColor = false;
            chkIsPaid.CheckedChanged += chkIsPaid_CheckedChanged;
            //
            // pnlFooter
            //
            pnlFooter.BackColor = Color.White;
            pnlFooter.Controls.Add(btnSave);
            pnlFooter.Controls.Add(btnCancel);
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(300, 488);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.Size = new Size(520, 68);
            pnlFooter.TabIndex = 17;
            pnlFooter.Paint += pnlFooter_Paint;
            //
            // btnSave
            //
            btnSave.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSave.BackColor = Color.FromArgb(47, 111, 214);
            btnSave.FlatAppearance.BorderColor = Color.FromArgb(47, 111, 214);
            btnSave.FlatAppearance.MouseOverBackColor = Color.FromArgb(37, 92, 186);
            btnSave.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(352, 15);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(140, 38);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            //
            // btnCancel
            //
            btnCancel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnCancel.BackColor = Color.White;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(190, 204, 222);
            btnCancel.FlatAppearance.MouseOverBackColor = Color.FromArgb(236, 242, 250);
            btnCancel.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(52, 68, 92);
            btnCancel.Location = new Point(250, 15);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(92, 38);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            //
            // PermitForm
            //
            AcceptButton = btnSave;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = btnCancel;
            ClientSize = new Size(820, 556);
            Controls.Add(lblHeading);
            Controls.Add(lblHint);
            Controls.Add(lblPlateNumber);
            Controls.Add(txtPlateNumber);
            Controls.Add(lblOwnerName);
            Controls.Add(txtOwnerName);
            Controls.Add(lblPermitType);
            Controls.Add(cmbPermitType);
            Controls.Add(lblZone);
            Controls.Add(cmbZone);
            Controls.Add(lblValidUntil);
            Controls.Add(pnlDateField);
            Controls.Add(btnOneMonth);
            Controls.Add(btnSixMonths);
            Controls.Add(btnOneYear);
            Controls.Add(chkIsPaid);
            Controls.Add(pnlFooter);
            Controls.Add(pnlPreview);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PermitForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Permit";
            Paint += PermitForm_Paint;
            pnlDateField.ResumeLayout(false);
            pnlFooter.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private BufferedPanel pnlPreview;
        private Label lblHeading;
        private Label lblHint;
        private Label lblPlateNumber;
        private TextBox txtPlateNumber;
        private Label lblOwnerName;
        private TextBox txtOwnerName;
        private Label lblPermitType;
        private ComboBox cmbPermitType;
        private Label lblZone;
        private ComboBox cmbZone;
        private Label lblValidUntil;
        private Panel pnlDateField;
        private DateTimePicker dtpValidUntil;
        private RoundedButton btnOneMonth;
        private RoundedButton btnSixMonths;
        private RoundedButton btnOneYear;
        private CheckBox chkIsPaid;
        private BufferedPanel pnlFooter;
        private RoundedButton btnSave;
        private RoundedButton btnCancel;
        private ToolTip tipQuickDate;
    }
}
