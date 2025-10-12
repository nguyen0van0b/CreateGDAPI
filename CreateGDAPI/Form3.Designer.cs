namespace CreateGDAPI
{
    partial class Form3
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
            groupBox1 = new GroupBox();
            comboApiEndpoint = new ComboBox();
            label5 = new Label();
            comboServiceType = new ComboBox();
            label4 = new Label();
            comboCurrency = new ComboBox();
            label3 = new Label();
            txtSoLuong = new TextBox();
            label2 = new Label();
            txtAgencyCode = new TextBox();
            label1 = new Label();
            txtPartnerCode = new TextBox();
            lblPartnerCode = new Label();
            groupBox2 = new GroupBox();
            btnAutoTest = new Button();
            btnViewTransactions = new Button();
            btnClearLogs = new Button();
            btnReloadData = new Button();
            btnOpenSettings = new Button();
            btnOpenReport = new Button();
            btnSendRequest = new Button();
            groupBoxAutoPush = new GroupBox();
            lblAutoPushStatus = new Label();
            btnStartAutoPush = new Button();
            numAutoPushInterval = new NumericUpDown();
            label7 = new Label();
            txtAutoPushCount = new TextBox();
            label6 = new Label();
            groupBox3 = new GroupBox();
            txtResult = new TextBox();
            txtPartnerRefList = new TextBox();
            btnCancelByList = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBoxAutoPush.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numAutoPushInterval).BeginInit();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtPartnerRefList);
            groupBox1.Controls.Add(comboApiEndpoint);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(comboServiceType);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(comboCurrency);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txtSoLuong);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtAgencyCode);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(txtPartnerCode);
            groupBox1.Controls.Add(lblPartnerCode);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1160, 120);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "API Configuration";
            // 
            // comboApiEndpoint
            // 
            comboApiEndpoint.DropDownStyle = ComboBoxStyle.DropDownList;
            comboApiEndpoint.FormattingEnabled = true;
            comboApiEndpoint.Location = new Point(120, 25);
            comboApiEndpoint.Name = "comboApiEndpoint";
            comboApiEndpoint.Size = new Size(200, 23);
            comboApiEndpoint.TabIndex = 11;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(20, 28);
            label5.Name = "label5";
            label5.Size = new Size(79, 15);
            label5.TabIndex = 10;
            label5.Text = "API Endpoint:";
            // 
            // comboServiceType
            // 
            comboServiceType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboServiceType.FormattingEnabled = true;
            comboServiceType.Location = new Point(723, 28);
            comboServiceType.Name = "comboServiceType";
            comboServiceType.Size = new Size(200, 23);
            comboServiceType.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(633, 31);
            label4.Name = "label4";
            label4.Size = new Size(75, 15);
            label4.TabIndex = 8;
            label4.Text = "Service Type:";
            // 
            // comboCurrency
            // 
            comboCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            comboCurrency.FormattingEnabled = true;
            comboCurrency.Location = new Point(723, 63);
            comboCurrency.Name = "comboCurrency";
            comboCurrency.Size = new Size(200, 23);
            comboCurrency.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(633, 66);
            label3.Name = "label3";
            label3.Size = new Size(58, 15);
            label3.TabIndex = 6;
            label3.Text = "Currency:";
            // 
            // txtSoLuong
            // 
            txtSoLuong.Location = new Point(420, 60);
            txtSoLuong.Name = "txtSoLuong";
            txtSoLuong.Size = new Size(200, 23);
            txtSoLuong.TabIndex = 5;
            txtSoLuong.Text = "1";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(330, 63);
            label2.Name = "label2";
            label2.Size = new Size(57, 15);
            label2.TabIndex = 4;
            label2.Text = "Số lượng:";
            // 
            // txtAgencyCode
            // 
            txtAgencyCode.Location = new Point(420, 25);
            txtAgencyCode.Name = "txtAgencyCode";
            txtAgencyCode.Size = new Size(200, 23);
            txtAgencyCode.TabIndex = 3;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(330, 28);
            label1.Name = "label1";
            label1.Size = new Size(81, 15);
            label1.TabIndex = 2;
            label1.Text = "Agency Code:";
            // 
            // txtPartnerCode
            // 
            txtPartnerCode.Location = new Point(120, 60);
            txtPartnerCode.Name = "txtPartnerCode";
            txtPartnerCode.Size = new Size(200, 23);
            txtPartnerCode.TabIndex = 1;
            // 
            // lblPartnerCode
            // 
            lblPartnerCode.AutoSize = true;
            lblPartnerCode.Location = new Point(20, 63);
            lblPartnerCode.Name = "lblPartnerCode";
            lblPartnerCode.Size = new Size(79, 15);
            lblPartnerCode.TabIndex = 0;
            lblPartnerCode.Text = "Partner Code:";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnAutoTest);
            groupBox2.Controls.Add(btnViewTransactions);
            groupBox2.Controls.Add(btnClearLogs);
            groupBox2.Controls.Add(btnReloadData);
            groupBox2.Controls.Add(btnOpenSettings);
            groupBox2.Controls.Add(btnOpenReport);
            groupBox2.Controls.Add(btnSendRequest);
            groupBox2.Location = new Point(12, 138);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(1160, 70);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Actions";
            // 
            // btnAutoTest
            // 
            btnAutoTest.Location = new Point(310, 25);
            btnAutoTest.Name = "btnAutoTest";
            btnAutoTest.Size = new Size(140, 30);
            btnAutoTest.TabIndex = 6;
            btnAutoTest.Text = "🤖 Auto Test";
            btnAutoTest.UseVisualStyleBackColor = true;
            btnAutoTest.Click += btnAutoTest_Click;
            // 
            // btnViewTransactions
            // 
            btnViewTransactions.Location = new Point(1013, 25);
            btnViewTransactions.Name = "btnViewTransactions";
            btnViewTransactions.Size = new Size(127, 30);
            btnViewTransactions.TabIndex = 5;
            btnViewTransactions.Text = "📋 View Transactions";
            btnViewTransactions.UseVisualStyleBackColor = true;
            btnViewTransactions.Click += btnViewTransactions_Click;
            // 
            // btnClearLogs
            // 
            btnClearLogs.Location = new Point(917, 25);
            btnClearLogs.Name = "btnClearLogs";
            btnClearLogs.Size = new Size(90, 30);
            btnClearLogs.TabIndex = 4;
            btnClearLogs.Text = "🗑️ Clear Logs";
            btnClearLogs.UseVisualStyleBackColor = true;
            btnClearLogs.Click += btnClearLogs_Click;
            // 
            // btnReloadData
            // 
            btnReloadData.Location = new Point(770, 25);
            btnReloadData.Name = "btnReloadData";
            btnReloadData.Size = new Size(140, 30);
            btnReloadData.TabIndex = 3;
            btnReloadData.Text = "🔄 Reload Data";
            btnReloadData.UseVisualStyleBackColor = true;
            btnReloadData.Click += btnReloadData_Click;
            // 
            // btnOpenSettings
            // 
            btnOpenSettings.BackColor = Color.FromArgb(192, 255, 255);
            btnOpenSettings.Location = new Point(624, 25);
            btnOpenSettings.Name = "btnOpenSettings";
            btnOpenSettings.Size = new Size(140, 30);
            btnOpenSettings.TabIndex = 2;
            btnOpenSettings.Text = "⚙️ Settings";
            btnOpenSettings.UseVisualStyleBackColor = false;
            btnOpenSettings.Click += btnOpenSettings_Click;
            // 
            // btnOpenReport
            // 
            btnOpenReport.BackColor = Color.FromArgb(255, 224, 192);
            btnOpenReport.Location = new Point(468, 25);
            btnOpenReport.Name = "btnOpenReport";
            btnOpenReport.Size = new Size(140, 30);
            btnOpenReport.TabIndex = 1;
            btnOpenReport.Text = "📊 Report";
            btnOpenReport.UseVisualStyleBackColor = false;
            btnOpenReport.Click += btnOpenReport_Click;
            // 
            // btnSendRequest
            // 
            btnSendRequest.BackColor = Color.FromArgb(0, 192, 0);
            btnSendRequest.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSendRequest.ForeColor = Color.White;
            btnSendRequest.Location = new Point(20, 25);
            btnSendRequest.Name = "btnSendRequest";
            btnSendRequest.Size = new Size(280, 30);
            btnSendRequest.TabIndex = 0;
            btnSendRequest.Text = "🚀 Send API Request";
            btnSendRequest.UseVisualStyleBackColor = false;
            btnSendRequest.Click += btnSendRequest_Click;
            // 
            // groupBoxAutoPush
            // 
            groupBoxAutoPush.Controls.Add(btnCancelByList);
            groupBoxAutoPush.Controls.Add(lblAutoPushStatus);
            groupBoxAutoPush.Controls.Add(btnStartAutoPush);
            groupBoxAutoPush.Controls.Add(numAutoPushInterval);
            groupBoxAutoPush.Controls.Add(label7);
            groupBoxAutoPush.Controls.Add(txtAutoPushCount);
            groupBoxAutoPush.Controls.Add(label6);
            groupBoxAutoPush.Location = new Point(12, 214);
            groupBoxAutoPush.Name = "groupBoxAutoPush";
            groupBoxAutoPush.Size = new Size(1160, 80);
            groupBoxAutoPush.TabIndex = 3;
            groupBoxAutoPush.TabStop = false;
            groupBoxAutoPush.Text = "🤖 Auto Push Configuration";
            // 
            // lblAutoPushStatus
            // 
            lblAutoPushStatus.AutoSize = true;
            lblAutoPushStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAutoPushStatus.Location = new Point(770, 35);
            lblAutoPushStatus.Name = "lblAutoPushStatus";
            lblAutoPushStatus.Size = new Size(93, 15);
            lblAutoPushStatus.TabIndex = 5;
            lblAutoPushStatus.Text = "⏸️ Not Running";
            // 
            // btnStartAutoPush
            // 
            btnStartAutoPush.BackColor = Color.FromArgb(128, 255, 192);
            btnStartAutoPush.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnStartAutoPush.Location = new Point(540, 25);
            btnStartAutoPush.Name = "btnStartAutoPush";
            btnStartAutoPush.Size = new Size(200, 35);
            btnStartAutoPush.TabIndex = 4;
            btnStartAutoPush.Text = "▶️ Start Auto Push";
            btnStartAutoPush.UseVisualStyleBackColor = false;
            btnStartAutoPush.Click += btnStartAutoPush_Click;
            // 
            // numAutoPushInterval
            // 
            numAutoPushInterval.Location = new Point(420, 32);
            numAutoPushInterval.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            numAutoPushInterval.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numAutoPushInterval.Name = "numAutoPushInterval";
            numAutoPushInterval.Size = new Size(100, 23);
            numAutoPushInterval.TabIndex = 3;
            numAutoPushInterval.Value = new decimal(new int[] { 5, 0, 0, 0 });
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(280, 35);
            label7.Name = "label7";
            label7.Size = new Size(125, 15);
            label7.TabIndex = 2;
            label7.Text = "Interval (seconds/req):";
            // 
            // txtAutoPushCount
            // 
            txtAutoPushCount.Location = new Point(150, 32);
            txtAutoPushCount.Name = "txtAutoPushCount";
            txtAutoPushCount.Size = new Size(100, 23);
            txtAutoPushCount.TabIndex = 1;
            txtAutoPushCount.Text = "10";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(20, 35);
            label6.Name = "label6";
            label6.Size = new Size(129, 15);
            label6.TabIndex = 0;
            label6.Text = "Total Requests to Send:";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(txtResult);
            groupBox3.Location = new Point(12, 300);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(1160, 400);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Response Logs";
            // 
            // txtResult
            // 
            txtResult.BackColor = Color.FromArgb(30, 30, 30);
            txtResult.Font = new Font("Consolas", 9.5F);
            txtResult.ForeColor = Color.FromArgb(220, 220, 220);
            txtResult.Location = new Point(6, 22);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.ScrollBars = ScrollBars.Both;
            txtResult.Size = new Size(1148, 372);
            txtResult.TabIndex = 0;
            txtResult.WordWrap = false;
            // 
            // txtPartnerRefList
            // 
            txtPartnerRefList.Font = new Font("Consolas", 9F);
            txtPartnerRefList.Location = new Point(940, 28);
            txtPartnerRefList.Multiline = true;
            txtPartnerRefList.Name = "txtPartnerRefList";
            txtPartnerRefList.PlaceholderText = "Nhập danh sách partnerRef (mỗi dòng 1 mã)";
            txtPartnerRefList.ScrollBars = ScrollBars.Vertical;
            txtPartnerRefList.Size = new Size(200, 60);
            txtPartnerRefList.TabIndex = 13;
            // 
            // btnCancelByList
            // 
            btnCancelByList.BackColor = Color.FromArgb(255, 192, 192);
            btnCancelByList.Location = new Point(991, 26);
            btnCancelByList.Name = "btnCancelByList";
            btnCancelByList.Size = new Size(127, 30);
            btnCancelByList.TabIndex = 12;
            btnCancelByList.Text = "❌ Cancel By List";
            btnCancelByList.UseVisualStyleBackColor = false;
            btnCancelByList.Click += btnCancelByList_Click;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 712);
            Controls.Add(groupBoxAutoPush);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Name = "Form3";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "API Testing Tool - All Endpoints";
            Load += Form3_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBoxAutoPush.ResumeLayout(false);
            groupBoxAutoPush.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numAutoPushInterval).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboApiEndpoint;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboServiceType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboCurrency;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSoLuong;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtAgencyCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPartnerCode;
        private System.Windows.Forms.Label lblPartnerCode;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnViewTransactions;
        private System.Windows.Forms.Button btnClearLogs;
        private System.Windows.Forms.Button btnReloadData;
        private System.Windows.Forms.Button btnSendRequest;
        private System.Windows.Forms.Button btnOpenSettings;
        private System.Windows.Forms.Button btnOpenReport;
        private System.Windows.Forms.Button btnAutoTest;
        private System.Windows.Forms.GroupBox groupBoxAutoPush;
        private System.Windows.Forms.Label lblAutoPushStatus;
        private System.Windows.Forms.Button btnStartAutoPush;
        private System.Windows.Forms.NumericUpDown numAutoPushInterval;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAutoPushCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtResult;
        private System.Windows.Forms.TextBox txtPartnerRefList;
        private System.Windows.Forms.Button btnCancelByList;
    }
}