namespace Newsletter.Forms
{
    partial class frmSyncSubscriber
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSync = new DevComponents.DotNetBar.ButtonX();
            this.lblExplanation = new DevComponents.DotNetBar.LabelX();
            this.FieldContainer = new System.Windows.Forms.ListView();
            this.chkSelectAll = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.circularProgress = new DevComponents.DotNetBar.Controls.CircularProgress();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.SuspendLayout();
            // 
            // btnSync
            // 
            this.btnSync.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.BackColor = System.Drawing.Color.Transparent;
            this.btnSync.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSync.Location = new System.Drawing.Point(585, 373);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(75, 23);
            this.btnSync.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSync.TabIndex = 0;
            this.btnSync.Text = "同步";
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // lblExplanation
            // 
            this.lblExplanation.AutoSize = true;
            this.lblExplanation.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lblExplanation.BackgroundStyle.Class = "";
            this.lblExplanation.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lblExplanation.Location = new System.Drawing.Point(24, 14);
            this.lblExplanation.Name = "lblExplanation";
            this.lblExplanation.Size = new System.Drawing.Size(114, 21);
            this.lblExplanation.TabIndex = 23;
            this.lblExplanation.Text = "請選擇電子報名單";
            // 
            // FieldContainer
            // 
            this.FieldContainer.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.FieldContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FieldContainer.CheckBoxes = true;
            this.FieldContainer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.FieldContainer.HideSelection = false;
            this.FieldContainer.Location = new System.Drawing.Point(24, 40);
            this.FieldContainer.Name = "FieldContainer";
            this.FieldContainer.Size = new System.Drawing.Size(636, 316);
            this.FieldContainer.TabIndex = 22;
            this.FieldContainer.UseCompatibleStateImageBehavior = false;
            this.FieldContainer.View = System.Windows.Forms.View.List;
            this.FieldContainer.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.FieldContainer_ItemChecked);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.chkSelectAll.BackgroundStyle.Class = "";
            this.chkSelectAll.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.chkSelectAll.Checked = true;
            this.chkSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelectAll.CheckValue = "Y";
            this.chkSelectAll.Location = new System.Drawing.Point(27, 362);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(54, 21);
            this.chkSelectAll.TabIndex = 21;
            this.chkSelectAll.Text = "全選";
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // circularProgress
            // 
            this.circularProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.circularProgress.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.circularProgress.BackgroundStyle.Class = "";
            this.circularProgress.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.circularProgress.Location = new System.Drawing.Point(504, 376);
            this.circularProgress.Name = "circularProgress";
            this.circularProgress.Size = new System.Drawing.Size(75, 23);
            this.circularProgress.Style = DevComponents.DotNetBar.eDotNetBarStyle.OfficeXP;
            this.circularProgress.TabIndex = 24;
            this.circularProgress.Visible = false;
            // 
            // labelX1
            // 
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Font = new System.Drawing.Font("微軟正黑體", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.labelX1.ForeColor = System.Drawing.Color.Blue;
            this.labelX1.Location = new System.Drawing.Point(144, 4);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(516, 32);
            this.labelX1.TabIndex = 25;
            this.labelX1.Text = "學生同步欄位：學生系統編號、姓名、性別、生日、入學年度、系所組別、電子郵件、學生狀態\n教師同步欄位：教師系統編號、姓名、性別、生日、電子郵件";
            this.labelX1.TextAlignment = System.Drawing.StringAlignment.Far;
            this.labelX1.WordWrap = true;
            // 
            // frmSyncSubscriber
            // 
            this.ClientSize = new System.Drawing.Size(684, 411);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.circularProgress);
            this.Controls.Add(this.lblExplanation);
            this.Controls.Add(this.FieldContainer);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.btnSync);
            this.DoubleBuffered = true;
            this.Name = "frmSyncSubscriber";
            this.Text = "同步MailChimp電子報訂閱者名單";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnSync;
        public DevComponents.DotNetBar.LabelX lblExplanation;
        public System.Windows.Forms.ListView FieldContainer;
        public DevComponents.DotNetBar.Controls.CheckBoxX chkSelectAll;
        private DevComponents.DotNetBar.Controls.CircularProgress circularProgress;
        private DevComponents.DotNetBar.LabelX labelX1;
    }
}