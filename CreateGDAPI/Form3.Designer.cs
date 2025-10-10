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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboApiEndpoint = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.comboServiceType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboCurrency = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSoLuong = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtAgencyCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPartnerCode = new System.Windows.Forms.TextBox();
            this.lblPartnerCode = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnViewTransactions = new System.Windows.Forms.Button();
            this.btnClearLogs = new System.Windows.Forms.Button();
            this.btnReloadData = new System.Windows.Forms.Button();
            this.btnOpenSettings = new System.Windows.Forms.Button();
            this.btnOpenReport = new System.Windows.Forms.Button();
            this.btnSendRequest = new System.Windows.Forms.Button();
            this.groupBoxAutoPush = new System.Windows.Forms.GroupBox();
            this.lblAutoPushStatus = new System.Windows.Forms.Label();
            this.btnStartAutoPush = new System.Windows.Forms.Button();
            this.numAutoPushInterval = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.txtAutoPushCount = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxAutoPush.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoPushInterval)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboApiEndpoint);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboServiceType);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.comboCurrency);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtSoLuong);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtAgencyCode);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtPartnerCode);
            this.groupBox1.Controls.Add(this.lblPartnerCode);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1160, 120);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "API Configuration";
            // 
            // comboApiEndpoint
            // 
            this.comboApiEndpoint.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboApiEndpoint.FormattingEnabled = true;
            this.comboApiEndpoint.Location = new System.Drawing.Point(120, 25);
            this.comboApiEndpoint.Name = "comboApiEndpoint";
            this.comboApiEndpoint.Size = new System.Drawing.Size(200, 23);
            this.comboApiEndpoint.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(82, 15);
            this.label5.TabIndex = 10;
            this.label5.Text = "API Endpoint:";
            // 
            // comboServiceType
            // 
            this.comboServiceType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboServiceType.FormattingEnabled = true;
            this.comboServiceType.Location = new System.Drawing.Point(940, 25);
            this.comboServiceType.Name = "comboServiceType";
            this.comboServiceType.Size = new System.Drawing.Size(200, 23);
            this.comboServiceType.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(850, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(78, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Service Type:";
            // 
            // comboCurrency
            // 
            this.comboCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCurrency.FormattingEnabled = true;
            this.comboCurrency.Location = new System.Drawing.Point(940, 60);
            this.comboCurrency.Name = "comboCurrency";
            this.comboCurrency.Size = new System.Drawing.Size(200, 23);
            this.comboCurrency.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(850, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Currency:";
            // 
            // txtSoLuong
            // 
            this.txtSoLuong.Location = new System.Drawing.Point(530, 60);
            this.txtSoLuong.Name = "txtSoLuong";
            this.txtSoLuong.Size = new System.Drawing.Size(200, 23);
            this.txtSoLuong.TabIndex = 5;
            this.txtSoLuong.Text = "1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(440, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Số lượng:";
            // 
            // txtAgencyCode
            // 
            this.txtAgencyCode.Location = new System.Drawing.Point(530, 25);
            this.txtAgencyCode.Name = "txtAgencyCode";
            this.txtAgencyCode.Size = new System.Drawing.Size(200, 23);
            this.txtAgencyCode.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(440, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Agency Code:";
            // 
            // txtPartnerCode
            // 
            this.txtPartnerCode.Location = new System.Drawing.Point(120, 60);
            this.txtPartnerCode.Name = "txtPartnerCode";
            this.txtPartnerCode.Size = new System.Drawing.Size(200, 23);
            this.txtPartnerCode.TabIndex = 1;
            // 
            // lblPartnerCode
            // 
            this.lblPartnerCode.AutoSize = true;
            this.lblPartnerCode.Location = new System.Drawing.Point(20, 63);
            this.lblPartnerCode.Name = "lblPartnerCode";
            this.lblPartnerCode.Size = new System.Drawing.Size(82, 15);
            this.lblPartnerCode.TabIndex = 0;
            this.lblPartnerCode.Text = "Partner Code:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnViewTransactions);
            this.groupBox2.Controls.Add(this.btnClearLogs);
            this.groupBox2.Controls.Add(this.btnReloadData);
            this.groupBox2.Controls.Add(this.btnOpenSettings);
            this.groupBox2.Controls.Add(this.btnOpenReport);
            this.groupBox2.Controls.Add(this.btnSendRequest);
            this.groupBox2.Location = new System.Drawing.Point(12, 138);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1160, 70);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Actions";
            // 
            // btnViewTransactions
            // 
            this.btnViewTransactions.Location = new System.Drawing.Point(920, 25);
            this.btnViewTransactions.Name = "btnViewTransactions";
            this.btnViewTransactions.Size = new System.Drawing.Size(220, 30);
            this.btnViewTransactions.TabIndex = 5;
            this.btnViewTransactions.Text = "📋 View Transactions";
            this.btnViewTransactions.UseVisualStyleBackColor = true;
            this.btnViewTransactions.Click += new System.EventHandler(this.btnViewTransactions_Click);
            // 
            // btnClearLogs
            // 
            this.btnClearLogs.Location = new System.Drawing.Point(770, 25);
            this.btnClearLogs.Name = "btnClearLogs";
            this.btnClearLogs.Size = new System.Drawing.Size(130, 30);
            this.btnClearLogs.TabIndex = 4;
            this.btnClearLogs.Text = "🗑️ Clear Logs";
            this.btnClearLogs.UseVisualStyleBackColor = true;
            this.btnClearLogs.Click += new System.EventHandler(this.btnClearLogs_Click);
            // 
            // btnReloadData
            // 
            this.btnReloadData.Location = new System.Drawing.Point(620, 25);
            this.btnReloadData.Name = "btnReloadData";
            this.btnReloadData.Size = new System.Drawing.Size(140, 30);
            this.btnReloadData.TabIndex = 3;
            this.btnReloadData.Text = "🔄 Reload Data";
            this.btnReloadData.UseVisualStyleBackColor = true;
            this.btnReloadData.Click += new System.EventHandler(this.btnReloadData_Click);
            // 
            // btnOpenSettings
            // 
            this.btnOpenSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnOpenSettings.Location = new System.Drawing.Point(470, 25);
            this.btnOpenSettings.Name = "btnOpenSettings";
            this.btnOpenSettings.Size = new System.Drawing.Size(140, 30);
            this.btnOpenSettings.TabIndex = 2;
            this.btnOpenSettings.Text = "⚙️ Settings";
            this.btnOpenSettings.UseVisualStyleBackColor = false;
            this.btnOpenSettings.Click += new System.EventHandler(this.btnOpenSettings_Click);
            // 
            // btnOpenReport
            // 
            this.btnOpenReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnOpenReport.Location = new System.Drawing.Point(320, 25);
            this.btnOpenReport.Name = "btnOpenReport";
            this.btnOpenReport.Size = new System.Drawing.Size(140, 30);
            this.btnOpenReport.TabIndex = 1;
            this.btnOpenReport.Text = "📊 Report";
            this.btnOpenReport.UseVisualStyleBackColor = false;
            this.btnOpenReport.Click += new System.EventHandler(this.btnOpenReport_Click);
            // 
            // btnSendRequest
            // 
            this.btnSendRequest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnSendRequest.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnSendRequest.ForeColor = System.Drawing.Color.White;
            this.btnSendRequest.Location = new System.Drawing.Point(20, 25);
            this.btnSendRequest.Name = "btnSendRequest";
            this.btnSendRequest.Size = new System.Drawing.Size(280, 30);
            this.btnSendRequest.TabIndex = 0;
            this.btnSendRequest.Text = "🚀 Send API Request";
            this.btnSendRequest.UseVisualStyleBackColor = false;
            this.btnSendRequest.Click += new System.EventHandler(this.btnSendRequest_Click);
            // 
            // groupBoxAutoPush
            // 
            this.groupBoxAutoPush.Controls.Add(this.lblAutoPushStatus);
            this.groupBoxAutoPush.Controls.Add(this.btnStartAutoPush);
            this.groupBoxAutoPush.Controls.Add(this.numAutoPushInterval);
            this.groupBoxAutoPush.Controls.Add(this.label7);
            this.groupBoxAutoPush.Controls.Add(this.txtAutoPushCount);
            this.groupBoxAutoPush.Controls.Add(this.label6);
            this.groupBoxAutoPush.Location = new System.Drawing.Point(12, 214);
            this.groupBoxAutoPush.Name = "groupBoxAutoPush";
            this.groupBoxAutoPush.Size = new System.Drawing.Size(1160, 80);
            this.groupBoxAutoPush.TabIndex = 3;
            this.groupBoxAutoPush.TabStop = false;
            this.groupBoxAutoPush.Text = "🤖 Auto Push Configuration";
            // 
            // lblAutoPushStatus
            // 
            this.lblAutoPushStatus.AutoSize = true;
            this.lblAutoPushStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblAutoPushStatus.Location = new System.Drawing.Point(770, 35);
            this.lblAutoPushStatus.Name = "lblAutoPushStatus";
            this.lblAutoPushStatus.Size = new System.Drawing.Size(95, 15);
            this.lblAutoPushStatus.TabIndex = 5;
            this.lblAutoPushStatus.Text = "⏸️ Not Running";
            // 
            // btnStartAutoPush
            // 
            this.btnStartAutoPush.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnStartAutoPush.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnStartAutoPush.Location = new System.Drawing.Point(540, 25);
            this.btnStartAutoPush.Name = "btnStartAutoPush";
            this.btnStartAutoPush.Size = new System.Drawing.Size(200, 35);
            this.btnStartAutoPush.TabIndex = 4;
            this.btnStartAutoPush.Text = "▶️ Start Auto Push";
            this.btnStartAutoPush.UseVisualStyleBackColor = false;
            this.btnStartAutoPush.Click += new System.EventHandler(this.btnStartAutoPush_Click);
            // 
            // numAutoPushInterval
            // 
            this.numAutoPushInterval.Location = new System.Drawing.Point(420, 32);
            this.numAutoPushInterval.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numAutoPushInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAutoPushInterval.Name = "numAutoPushInterval";
            this.numAutoPushInterval.Size = new System.Drawing.Size(100, 23);
            this.numAutoPushInterval.TabIndex = 3;
            this.numAutoPushInterval.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(280, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(134, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "Interval (seconds/req):";
            // 
            // txtAutoPushCount
            // 
            this.txtAutoPushCount.Location = new System.Drawing.Point(150, 32);
            this.txtAutoPushCount.Name = "txtAutoPushCount";
            this.txtAutoPushCount.Size = new System.Drawing.Size(100, 23);
            this.txtAutoPushCount.TabIndex = 1;
            this.txtAutoPushCount.Text = "10";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 35);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(124, 15);
            this.label6.TabIndex = 0;
            this.label6.Text = "Total Requests to Send:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtResult);
            this.groupBox3.Location = new System.Drawing.Point(12, 300);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(1160, 400);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Response Logs";
            // 
            // txtResult
            // 
            this.txtResult.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtResult.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtResult.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtResult.Location = new System.Drawing.Point(6, 22);
            this.txtResult.Multiline = true;
            this.txtResult.Name = "txtResult";
            this.txtResult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtResult.Size = new System.Drawing.Size(1148, 372);
            this.txtResult.TabIndex = 0;
            this.txtResult.WordWrap = false;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 712);
            this.Controls.Add(this.groupBoxAutoPush);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "API Testing Tool - All Endpoints";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBoxAutoPush.ResumeLayout(false);
            this.groupBoxAutoPush.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAutoPushInterval)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
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
        private System.Windows.Forms.GroupBox groupBoxAutoPush;
        private System.Windows.Forms.Label lblAutoPushStatus;
        private System.Windows.Forms.Button btnStartAutoPush;
        private System.Windows.Forms.NumericUpDown numAutoPushInterval;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtAutoPushCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtResult;
    }
}