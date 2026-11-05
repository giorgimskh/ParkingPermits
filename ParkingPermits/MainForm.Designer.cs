namespace ParkingPermits
{
    partial class MainForm
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
            DataGridViewCellStyle alternatingRowStyle = new DataGridViewCellStyle();
            DataGridViewCellStyle headerStyle = new DataGridViewCellStyle();
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();
            DataGridViewCellStyle plateStyle = new DataGridViewCellStyle();
            DataGridViewCellStyle dateStyle = new DataGridViewCellStyle();
            DataGridViewCellStyle daysLeftStyle = new DataGridViewCellStyle();
            pnlHeader = new BufferedPanel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            lblStatus = new Label();
            btnExport = new RoundedButton();
            btnAdd = new RoundedButton();
            pnlPermitsPage = new BufferedPanel();
            pnlOverviewPage = new BufferedPanel();
            pnlMapPage = new BufferedPanel();
            tipSpot = new ToolTip(components);
            pnlList = new CardPanel();
            pnlTabs = new BufferedPanel();
            txtSearch = new TextBox();
            lblPermitType = new Label();
            cmbPermitType = new ComboBox();
            lblSort = new Label();
            cmbSort = new ComboBox();
            dgvPermits = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colPlateNumber = new DataGridViewTextBoxColumn();
            colOwnerName = new DataGridViewTextBoxColumn();
            colPermitType = new DataGridViewTextBoxColumn();
            colZone = new DataGridViewTextBoxColumn();
            colValidUntil = new DataGridViewTextBoxColumn();
            colDaysLeft = new DataGridViewTextBoxColumn();
            colPayment = new DataGridViewTextBoxColumn();
            colValidity = new DataGridViewTextBoxColumn();
            pnlLegend = new BufferedPanel();
            lblCount = new Label();
            pnlDetails = new CardPanel();
            btnEdit = new RoundedButton();
            btnDelete = new RoundedButton();
            tmrStatus = new System.Windows.Forms.Timer(components);
            pnlHeader.SuspendLayout();
            pnlPermitsPage.SuspendLayout();
            pnlList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPermits).BeginInit();
            pnlDetails.SuspendLayout();
            SuspendLayout();
            //
            // pnlHeader
            //
            pnlHeader.BackColor = Color.FromArgb(30, 90, 168);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblStatus);
            pnlHeader.Controls.Add(btnExport);
            pnlHeader.Controls.Add(btnAdd);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(1280, 64);
            pnlHeader.TabIndex = 0;
            pnlHeader.Paint += pnlHeader_Paint;
            pnlHeader.MouseClick += pnlHeader_MouseClick;
            pnlHeader.MouseLeave += pnlHeader_MouseLeave;
            pnlHeader.MouseMove += pnlHeader_MouseMove;
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(70, 8);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(152, 25);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Parking Permits";
            //
            // lblSubtitle
            //
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(200, 218, 240);
            lblSubtitle.Location = new Point(72, 36);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(176, 15);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Campus parking permit register";
            //
            // lblStatus
            //
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblStatus.BackColor = Color.FromArgb(220, 245, 230);
            lblStatus.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(23, 112, 63);
            lblStatus.Location = new Point(682, 16);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(300, 32);
            lblStatus.TabIndex = 2;
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            lblStatus.Visible = false;
            //
            // btnExport
            //
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExport.BackColor = Color.FromArgb(45, 106, 186);
            btnExport.FlatAppearance.BorderColor = Color.FromArgb(120, 158, 214);
            btnExport.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 122, 204);
            btnExport.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnExport.ForeColor = Color.White;
            btnExport.Location = new Point(994, 13);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(110, 38);
            btnExport.TabIndex = 3;
            btnExport.Text = "Export CSV";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += btnExport_Click;
            //
            // btnAdd
            //
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.BackColor = Color.White;
            btnAdd.FlatAppearance.BorderColor = Color.White;
            btnAdd.FlatAppearance.MouseOverBackColor = Color.FromArgb(225, 236, 250);
            btnAdd.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnAdd.ForeColor = Color.FromArgb(30, 90, 168);
            btnAdd.Location = new Point(1116, 13);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(140, 38);
            btnAdd.TabIndex = 4;
            btnAdd.Text = "+  New permit";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += btnAdd_Click;
            //
            // pnlPermitsPage
            //
            pnlPermitsPage.BackColor = Color.FromArgb(227, 235, 245);
            pnlPermitsPage.Controls.Add(pnlList);
            pnlPermitsPage.Controls.Add(pnlDetails);
            pnlPermitsPage.Dock = DockStyle.Fill;
            pnlPermitsPage.Location = new Point(0, 64);
            pnlPermitsPage.Name = "pnlPermitsPage";
            pnlPermitsPage.Size = new Size(1280, 696);
            pnlPermitsPage.TabIndex = 1;
            //
            // pnlOverviewPage
            //
            pnlOverviewPage.BackColor = Color.FromArgb(227, 235, 245);
            pnlOverviewPage.Dock = DockStyle.Fill;
            pnlOverviewPage.Location = new Point(0, 64);
            pnlOverviewPage.Name = "pnlOverviewPage";
            pnlOverviewPage.Size = new Size(1280, 696);
            pnlOverviewPage.TabIndex = 2;
            pnlOverviewPage.Visible = false;
            pnlOverviewPage.Paint += pnlOverviewPage_Paint;
            pnlOverviewPage.MouseClick += pnlOverviewPage_MouseClick;
            pnlOverviewPage.MouseMove += pnlOverviewPage_MouseMove;
            //
            // pnlMapPage
            //
            pnlMapPage.BackColor = Color.FromArgb(227, 235, 245);
            pnlMapPage.Dock = DockStyle.Fill;
            pnlMapPage.Location = new Point(0, 64);
            pnlMapPage.Name = "pnlMapPage";
            pnlMapPage.Size = new Size(1280, 696);
            pnlMapPage.TabIndex = 3;
            pnlMapPage.Visible = false;
            pnlMapPage.Paint += pnlMapPage_Paint;
            pnlMapPage.MouseClick += pnlMapPage_MouseClick;
            pnlMapPage.MouseLeave += pnlMapPage_MouseLeave;
            pnlMapPage.MouseMove += pnlMapPage_MouseMove;
            //
            // pnlTabs
            //
            pnlTabs.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlTabs.BackColor = Color.White;
            pnlTabs.Cursor = Cursors.Hand;
            pnlTabs.Location = new Point(16, 68);
            pnlTabs.Name = "pnlTabs";
            pnlTabs.Size = new Size(860, 44);
            pnlTabs.TabIndex = 5;
            pnlTabs.Paint += pnlTabs_Paint;
            pnlTabs.MouseClick += pnlTabs_MouseClick;
            pnlTabs.MouseLeave += pnlTabs_MouseLeave;
            pnlTabs.MouseMove += pnlTabs_MouseMove;
            //
            // pnlList
            //
            pnlList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlList.BackColor = Color.White;
            pnlList.Controls.Add(txtSearch);
            pnlList.Controls.Add(lblPermitType);
            pnlList.Controls.Add(cmbPermitType);
            pnlList.Controls.Add(lblSort);
            pnlList.Controls.Add(cmbSort);
            pnlList.Controls.Add(dgvPermits);
            pnlList.Controls.Add(pnlTabs);
            pnlList.Controls.Add(pnlLegend);
            pnlList.Controls.Add(lblCount);
            pnlList.Location = new Point(24, 24);
            pnlList.Name = "pnlList";
            pnlList.Size = new Size(892, 648);
            pnlList.TabIndex = 2;
            pnlList.Paint += pnlList_Paint;
            //
            // txtSearch
            //
            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.Font = new Font("Segoe UI", 10.5F);
            txtSearch.ForeColor = Color.FromArgb(27, 42, 65);
            txtSearch.Location = new Point(50, 26);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search plate number or owner";
            txtSearch.Size = new Size(254, 19);
            txtSearch.TabIndex = 0;
            txtSearch.TextChanged += txtSearch_TextChanged;
            //
            // lblPermitType
            //
            lblPermitType.AutoSize = true;
            lblPermitType.ForeColor = Color.FromArgb(91, 107, 130);
            lblPermitType.Location = new Point(334, 28);
            lblPermitType.Name = "lblPermitType";
            lblPermitType.Size = new Size(31, 15);
            lblPermitType.TabIndex = 1;
            lblPermitType.Text = "Type";
            //
            // cmbPermitType
            //
            cmbPermitType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPermitType.FlatStyle = FlatStyle.Flat;
            cmbPermitType.Font = new Font("Segoe UI", 10.5F);
            cmbPermitType.ForeColor = Color.FromArgb(27, 42, 65);
            cmbPermitType.FormattingEnabled = true;
            cmbPermitType.Items.AddRange(new object[] { "All types", "Student", "Staff", "Guest" });
            cmbPermitType.Location = new Point(376, 22);
            cmbPermitType.Name = "cmbPermitType";
            cmbPermitType.Size = new Size(120, 27);
            cmbPermitType.TabIndex = 2;
            cmbPermitType.SelectedIndexChanged += cmbPermitType_SelectedIndexChanged;
            //
            // lblSort
            //
            lblSort.AutoSize = true;
            lblSort.ForeColor = Color.FromArgb(91, 107, 130);
            lblSort.Location = new Point(520, 28);
            lblSort.Name = "lblSort";
            lblSort.Size = new Size(43, 15);
            lblSort.TabIndex = 3;
            lblSort.Text = "Sort by";
            //
            // cmbSort
            //
            cmbSort.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSort.FlatStyle = FlatStyle.Flat;
            cmbSort.Font = new Font("Segoe UI", 10.5F);
            cmbSort.ForeColor = Color.FromArgb(27, 42, 65);
            cmbSort.FormattingEnabled = true;
            cmbSort.Items.AddRange(new object[] { "Plate A–Z", "Owner A–Z", "Expiring first", "Expiring last" });
            cmbSort.Location = new Point(574, 22);
            cmbSort.Name = "cmbSort";
            cmbSort.Size = new Size(150, 27);
            cmbSort.TabIndex = 4;
            cmbSort.SelectedIndexChanged += cmbSort_SelectedIndexChanged;
            //
            // dgvPermits
            //
            dgvPermits.AllowUserToAddRows = false;
            dgvPermits.AllowUserToDeleteRows = false;
            dgvPermits.AllowUserToResizeRows = false;
            alternatingRowStyle.BackColor = Color.FromArgb(244, 247, 251);
            dgvPermits.AlternatingRowsDefaultCellStyle = alternatingRowStyle;
            dgvPermits.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPermits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPermits.BackgroundColor = Color.White;
            dgvPermits.BorderStyle = BorderStyle.None;
            dgvPermits.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvPermits.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            headerStyle.BackColor = Color.White;
            headerStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
            headerStyle.ForeColor = Color.FromArgb(91, 107, 130);
            headerStyle.Padding = new Padding(8, 0, 0, 0);
            headerStyle.SelectionBackColor = Color.White;
            headerStyle.SelectionForeColor = Color.FromArgb(91, 107, 130);
            headerStyle.WrapMode = DataGridViewTriState.False;
            dgvPermits.ColumnHeadersDefaultCellStyle = headerStyle;
            dgvPermits.ColumnHeadersHeight = 40;
            dgvPermits.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvPermits.Columns.AddRange(new DataGridViewColumn[] { colId, colPlateNumber, colOwnerName, colPermitType, colZone, colValidUntil, colDaysLeft, colPayment, colValidity });
            cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            cellStyle.BackColor = Color.White;
            cellStyle.Font = new Font("Segoe UI", 9.75F);
            cellStyle.ForeColor = Color.FromArgb(27, 42, 65);
            cellStyle.Padding = new Padding(8, 0, 0, 0);
            cellStyle.SelectionBackColor = Color.FromArgb(211, 227, 250);
            cellStyle.SelectionForeColor = Color.FromArgb(27, 42, 65);
            cellStyle.WrapMode = DataGridViewTriState.False;
            dgvPermits.DefaultCellStyle = cellStyle;
            dgvPermits.EnableHeadersVisualStyles = false;
            dgvPermits.GridColor = Color.FromArgb(230, 236, 244);
            dgvPermits.Location = new Point(16, 120);
            dgvPermits.MultiSelect = false;
            dgvPermits.Name = "dgvPermits";
            dgvPermits.ReadOnly = true;
            dgvPermits.RowHeadersVisible = false;
            dgvPermits.RowTemplate.Height = 42;
            dgvPermits.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPermits.Size = new Size(860, 472);
            dgvPermits.TabIndex = 6;
            dgvPermits.CellDoubleClick += dgvPermits_CellDoubleClick;
            dgvPermits.CellFormatting += dgvPermits_CellFormatting;
            dgvPermits.CellPainting += dgvPermits_CellPainting;
            dgvPermits.SelectionChanged += dgvPermits_SelectionChanged;
            dgvPermits.Paint += dgvPermits_Paint;
            //
            // colId
            //
            colId.DataPropertyName = "Id";
            colId.HeaderText = "Id";
            colId.Name = "colId";
            colId.ReadOnly = true;
            colId.Visible = false;
            //
            // colPlateNumber
            //
            colPlateNumber.DataPropertyName = "PlateNumber";
            plateStyle.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold);
            plateStyle.Padding = new Padding(16, 0, 0, 0);
            colPlateNumber.DefaultCellStyle = plateStyle;
            colPlateNumber.FillWeight = 95F;
            colPlateNumber.HeaderText = "PLATE";
            colPlateNumber.Name = "colPlateNumber";
            colPlateNumber.ReadOnly = true;
            //
            // colOwnerName
            //
            colOwnerName.DataPropertyName = "OwnerName";
            colOwnerName.FillWeight = 130F;
            colOwnerName.HeaderText = "OWNER";
            colOwnerName.Name = "colOwnerName";
            colOwnerName.ReadOnly = true;
            //
            // colPermitType
            //
            colPermitType.DataPropertyName = "PermitType";
            colPermitType.FillWeight = 70F;
            colPermitType.HeaderText = "TYPE";
            colPermitType.Name = "colPermitType";
            colPermitType.ReadOnly = true;
            //
            // colZone
            //
            colZone.DataPropertyName = "Zone";
            colZone.FillWeight = 50F;
            colZone.HeaderText = "ZONE";
            colZone.Name = "colZone";
            colZone.ReadOnly = true;
            //
            // colValidUntil
            //
            colValidUntil.DataPropertyName = "ValidUntil";
            dateStyle.Format = "dd MMM yyyy";
            colValidUntil.DefaultCellStyle = dateStyle;
            colValidUntil.FillWeight = 90F;
            colValidUntil.HeaderText = "VALID UNTIL";
            colValidUntil.Name = "colValidUntil";
            colValidUntil.ReadOnly = true;
            //
            // colDaysLeft
            //
            colDaysLeft.DataPropertyName = "DaysLeft";
            daysLeftStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            daysLeftStyle.Padding = new Padding(0, 0, 16, 0);
            colDaysLeft.DefaultCellStyle = daysLeftStyle;
            colDaysLeft.FillWeight = 75F;
            colDaysLeft.HeaderText = "DAYS LEFT";
            colDaysLeft.Name = "colDaysLeft";
            colDaysLeft.ReadOnly = true;
            //
            // colPayment
            //
            colPayment.DataPropertyName = "Payment";
            colPayment.FillWeight = 70F;
            colPayment.HeaderText = "PAYMENT";
            colPayment.Name = "colPayment";
            colPayment.ReadOnly = true;
            //
            // colValidity
            //
            colValidity.DataPropertyName = "Validity";
            colValidity.FillWeight = 110F;
            colValidity.HeaderText = "STATUS";
            colValidity.Name = "colValidity";
            colValidity.ReadOnly = true;
            //
            // pnlLegend
            //
            pnlLegend.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            pnlLegend.BackColor = Color.White;
            pnlLegend.Location = new Point(20, 606);
            pnlLegend.Name = "pnlLegend";
            pnlLegend.Size = new Size(440, 24);
            pnlLegend.TabIndex = 6;
            pnlLegend.Paint += pnlLegend_Paint;
            //
            // lblCount
            //
            lblCount.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblCount.ForeColor = Color.FromArgb(91, 107, 130);
            lblCount.Location = new Point(616, 610);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(256, 17);
            lblCount.TabIndex = 7;
            lblCount.TextAlign = ContentAlignment.MiddleRight;
            //
            // pnlDetails
            //
            pnlDetails.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            pnlDetails.BackColor = Color.White;
            pnlDetails.Controls.Add(btnEdit);
            pnlDetails.Controls.Add(btnDelete);
            pnlDetails.Location = new Point(932, 24);
            pnlDetails.Name = "pnlDetails";
            pnlDetails.Size = new Size(324, 648);
            pnlDetails.TabIndex = 3;
            pnlDetails.Paint += pnlDetails_Paint;
            //
            // btnEdit
            //
            btnEdit.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnEdit.BackColor = Color.White;
            btnEdit.FlatAppearance.BorderColor = Color.FromArgb(120, 190, 180);
            btnEdit.FlatAppearance.MouseOverBackColor = Color.FromArgb(222, 244, 240);
            btnEdit.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnEdit.ForeColor = Color.FromArgb(12, 99, 94);
            btnEdit.Location = new Point(24, 586);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(132, 38);
            btnEdit.TabIndex = 0;
            btnEdit.Text = "Edit";
            btnEdit.UseVisualStyleBackColor = false;
            btnEdit.Click += btnEdit_Click;
            //
            // btnDelete
            //
            btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDelete.BackColor = Color.White;
            btnDelete.FlatAppearance.BorderColor = Color.FromArgb(240, 170, 180);
            btnDelete.FlatAppearance.MouseOverBackColor = Color.FromArgb(253, 238, 241);
            btnDelete.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            btnDelete.ForeColor = Color.FromArgb(163, 32, 62);
            btnDelete.Location = new Point(168, 586);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(132, 38);
            btnDelete.TabIndex = 1;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = false;
            btnDelete.Click += btnDelete_Click;
            //
            // tmrStatus
            //
            tmrStatus.Interval = 3000;
            tmrStatus.Tick += tmrStatus_Tick;
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(227, 235, 245);
            ClientSize = new Size(1280, 760);
            Controls.Add(pnlPermitsPage);
            Controls.Add(pnlOverviewPage);
            Controls.Add(pnlMapPage);
            Controls.Add(pnlHeader);
            MinimumSize = new Size(1200, 800);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Parking Permits";
            Load += MainForm_Load;
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlList.ResumeLayout(false);
            pnlList.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPermits).EndInit();
            pnlDetails.ResumeLayout(false);
            pnlPermitsPage.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private BufferedPanel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblStatus;
        private RoundedButton btnExport;
        private RoundedButton btnAdd;
        private BufferedPanel pnlPermitsPage;
        private BufferedPanel pnlOverviewPage;
        private BufferedPanel pnlMapPage;
        private ToolTip tipSpot;
        private BufferedPanel pnlTabs;
        private CardPanel pnlList;
        private TextBox txtSearch;
        private Label lblPermitType;
        private ComboBox cmbPermitType;
        private Label lblSort;
        private ComboBox cmbSort;
        private DataGridView dgvPermits;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colPlateNumber;
        private DataGridViewTextBoxColumn colOwnerName;
        private DataGridViewTextBoxColumn colPermitType;
        private DataGridViewTextBoxColumn colZone;
        private DataGridViewTextBoxColumn colValidUntil;
        private DataGridViewTextBoxColumn colDaysLeft;
        private DataGridViewTextBoxColumn colPayment;
        private DataGridViewTextBoxColumn colValidity;
        private BufferedPanel pnlLegend;
        private Label lblCount;
        private CardPanel pnlDetails;
        private RoundedButton btnEdit;
        private RoundedButton btnDelete;
        private System.Windows.Forms.Timer tmrStatus;
    }
}
