namespace CreateGDAPI
{
    partial class FormReport
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSearchPartnerRef = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboFilterEndpoint = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnResetFilter = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblSuccessRate = new System.Windows.Forms.Label();
            this.lblFailedCount = new System.Windows.Forms.Label();
            this.lblSuccessCount = new System.Windows.Forms.Label();
            this.lblTotalRequests = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dgvStatistics = new System.Windows.Forms.DataGridView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.dgvDetails = new System.Windows.Forms.DataGridView();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSearchPartnerRef);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboFilterEndpoint);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnResetFilter);
            this.groupBox1.Controls.Add(this.btnFilter);
            this.groupBox1.Controls.Add(this.dtpTo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpFrom);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1260, 140);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "🔍 Filter by Date Range, Endpoint & PartnerRef";
            // 
            // txtSearchPartnerRef
            // 
            this.txtSearchPartnerRef.Location = new System.Drawing.Point(500, 65);
            this.txtSearchPartnerRef.Name = "txtSearchPartnerRef";
            this.txtSearchPartnerRef.PlaceholderText = "Nhập PartnerRef để tìm kiếm...";
            this.txtSearchPartnerRef.Size = new System.Drawing.Size(250, 23);
            this.txtSearchPartnerRef.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(360, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(130, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "🔎 Search PartnerRef:";
            // 
            // comboFilterEndpoint
            // 
            this.comboFilterEndpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFilterEndpoint.FormattingEnabled = true;
            this.comboFilterEndpoint.Location = new System.Drawing.Point(130, 65);
            this.comboFilterEndpoint.Name = "comboFilterEndpoint";
            this.comboFilterEndpoint.Size = new System.Drawing.Size(200, 23);
            this.comboFilterEndpoint.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Filter Endpoint:";
            // 
            // btnResetFilter
            // 
            this.btnResetFilter.Location = new System.Drawing.Point(1010, 95);
            this.btnResetFilter.Name = "btnResetFilter";
            this.btnResetFilter.Size = new System.Drawing.Size(120, 30);
            this.btnResetFilter.TabIndex = 5;
            this.btnResetFilter.Text = "🔄 Reset Filter";
            this.btnResetFilter.UseVisualStyleBackColor = true;
            this.btnResetFilter.Click += new System.EventHandler(this.btnResetFilter_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnFilter.Location = new System.Drawing.Point(870, 95);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(120, 30);
            this.btnFilter.TabIndex = 4;
            this.btnFilter.Text = "🔍 Apply Filter";
            this.btnFilter.UseVisualStyleBackColor = false;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // dtpTo
            // 
            this.dtpTo.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpTo.Location = new System.Drawing.Point(500, 30);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(250, 23);
            this.dtpTo.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(460, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "To:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.CustomFormat = "yyyy-MM-dd HH:mm:ss";
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFrom.Location = new System.Drawing.Point(80, 30);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(250, 23);
            this.dtpFrom.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "From:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lblSuccessRate);
            this.groupBox2.Controls.Add(this.lblFailedCount);
            this.groupBox2.Controls.Add(this.lblSuccessCount);
            this.groupBox2.Controls.Add(this.lblTotalRequests);
            this.groupBox2.Location = new System.Drawing.Point(12, 158);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1260, 80);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "📊 Summary Statistics";
            // 
            // lblSuccessRate
            // 
            this.lblSuccessRate.AutoSize = true;
            this.lblSuccessRate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSuccessRate.Location = new System.Drawing.Point(900, 35);
            this.lblSuccessRate.Name = "lblSuccessRate";
            this.lblSuccessRate.Size = new System.Drawing.Size(195, 19);
            this.lblSuccessRate.TabIndex = 3;
            this.lblSuccessRate.Text = "📈 Tỷ lệ thành công: 0.00%";
            // 
            // lblFailedCount
            // 
            this.lblFailedCount.AutoSize = true;
            this.lblFailedCount.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFailedCount.ForeColor = System.Drawing.Color.Red;
            this.lblFailedCount.Location = new System.Drawing.Point(600, 35);
            this.lblFailedCount.Name = "lblFailedCount";
            this.lblFailedCount.Size = new System.Drawing.Size(139, 19);
            this.lblFailedCount.TabIndex = 2;
            this.lblFailedCount.Text = "❌ Thất bại: 0";
            // 
            // lblSuccessCount
            // 
            this.lblSuccessCount.AutoSize = true;
            this.lblSuccessCount.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSuccessCount.ForeColor = System.Drawing.Color.Green;
            this.lblSuccessCount.Location = new System.Drawing.Point(300, 35);
            this.lblSuccessCount.Name = "lblSuccessCount";
            this.lblSuccessCount.Size = new System.Drawing.Size(164, 19);
            this.lblSuccessCount.TabIndex = 1;
            this.lblSuccessCount.Text = "✅ Thành công: 0";
            // 
            // lblTotalRequests
            // 
            this.lblTotalRequests.AutoSize = true;
            this.lblTotalRequests.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTotalRequests.Location = new System.Drawing.Point(20, 35);
            this.lblTotalRequests.Name = "lblTotalRequests";
            this.lblTotalRequests.Size = new System.Drawing.Size(186, 19);
            this.lblTotalRequests.TabIndex = 0;
            this.lblTotalRequests.Text = "📊 Tổng số requests: 0";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.dgvStatistics);
            this.groupBox3.Location = new System.Drawing.Point(12, 244);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1260, 200);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "📈 API Statistics by Endpoint";
            // 
            // dgvStatistics
            // 
            this.dgvStatistics.AllowUserToAddRows = false;
            this.dgvStatistics.AllowUserToDeleteRows = false;
            this.dgvStatistics.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStatistics.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStatistics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStatistics.Location = new System.Drawing.Point(3, 19);
            this.dgvStatistics.Name = "dgvStatistics";
            this.dgvStatistics.ReadOnly = true;
            this.dgvStatistics.RowTemplate.Height = 25;
            this.dgvStatistics.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStatistics.Size = new System.Drawing.Size(1254, 178);
            this.dgvStatistics.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.dgvDetails);
            this.groupBox4.Location = new System.Drawing.Point(12, 450);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(1260, 250);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "📋 Detailed Request Logs";
            // 
            // dgvDetails
            // 
            this.dgvDetails.AllowUserToAddRows = false;
            this.dgvDetails.AllowUserToDeleteRows = false;
            this.dgvDetails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDetails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetails.Location = new System.Drawing.Point(3, 19);
            this.dgvDetails.Name = "dgvDetails";
            this.dgvDetails.ReadOnly = true;
            this.dgvDetails.RowTemplate.Height = 25;
            this.dgvDetails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDetails.Size = new System.Drawing.Size(1254, 228);
            this.dgvDetails.TabIndex = 0;
            this.dgvDetails.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetails_CellClick);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnClear);
            this.groupBox5.Controls.Add(this.btnExportExcel);
            this.groupBox5.Controls.Add(this.btnRefresh);
            this.groupBox5.Location = new System.Drawing.Point(12, 706);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(1260, 70);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "⚙️ Actions";
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnClear.Location = new System.Drawing.Point(450, 25);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(200, 35);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "🗑️ Clear All Data";
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(144)))), ((int)(((byte)(238)))), ((int)(((byte)(144)))));
            this.btnExportExcel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnExportExcel.Location = new System.Drawing.Point(230, 25);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(200, 35);
            this.btnExportExcel.TabIndex = 1;
            this.btnExportExcel.Text = "📊 Export to Excel";
            this.btnExportExcel.UseVisualStyleBackColor = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(216)))), ((int)(((byte)(230)))));
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnRefresh.Location = new System.Drawing.Point(20, 25);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(200, 35);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "🔄 Refresh Data";
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // FormReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 788);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FormReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "📊 API Test Report - Statistics & Analytics";
            this.Load += new System.EventHandler(this.FormReport_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStatistics)).EndInit();
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetails)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.ComboBox comboFilterEndpoint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSearchPartnerRef;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblTotalRequests;
        private System.Windows.Forms.Label lblSuccessCount;
        private System.Windows.Forms.Label lblFailedCount;
        private System.Windows.Forms.Label lblSuccessRate;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView dgvStatistics;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView dgvDetails;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnResetFilter;
    }
}