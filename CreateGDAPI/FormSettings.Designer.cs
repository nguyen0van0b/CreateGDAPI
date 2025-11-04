namespace CreateGDAPI
{
    partial class FormSettings
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
            txtSearch = new TextBox();
            label1 = new Label();
            chkFields = new CheckedListBox();
            groupBox2 = new GroupBox();
            btnSelectReceiverInfo = new Button();
            btnSelectSenderInfo = new Button();
            btnSelectPaymentInfo = new Button();
            btnDeselectAll = new Button();
            btnSelectAll = new Button();
            groupBox3 = new GroupBox();
            lblBlacklistInfo = new Label();
            chkUseBlackListOnly = new CheckBox();
            groupBox4 = new GroupBox();
            lblStatus = new Label();
            btnResetDefault = new Button();
            btnClose = new Button();
            btnSaveAndClose = new Button();
            btnSave = new Button();
            groupBox5 = new GroupBox();
            btnSetAllNotSend = new Button();
            btnSetAllNull = new Button();
            btnResetFieldModes = new Button();
            lblFieldModeInfo = new Label();
            comboFieldMode = new ComboBox();
            label2 = new Label();
            chkAutoCancelIfNotPaid = new CheckBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtSearch);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(chkFields);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(500, 500);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "📋 Chọn Fields để gửi trong Request";
            // 
            // txtSearch
            // 
            txtSearch.Font = new Font("Segoe UI", 10F);
            txtSearch.Location = new Point(80, 25);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Tìm kiếm field...";
            txtSearch.Size = new Size(410, 25);
            txtSearch.TabIndex = 2;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, 30);
            label1.Name = "label1";
            label1.Size = new Size(60, 15);
            label1.TabIndex = 1;
            label1.Text = "🔍 Search:";
            // 
            // chkFields
            // 
            chkFields.CheckOnClick = true;
            chkFields.Font = new Font("Consolas", 9F);
            chkFields.FormattingEnabled = true;
            chkFields.Location = new Point(15, 60);
            chkFields.Name = "chkFields";
            chkFields.Size = new Size(475, 412);
            chkFields.TabIndex = 0;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnSelectReceiverInfo);
            groupBox2.Controls.Add(btnSelectSenderInfo);
            groupBox2.Controls.Add(btnSelectPaymentInfo);
            groupBox2.Controls.Add(btnDeselectAll);
            groupBox2.Controls.Add(btnSelectAll);
            groupBox2.Location = new Point(530, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(280, 250);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "⚡ Quick Actions";
            // 
            // btnSelectReceiverInfo
            // 
            btnSelectReceiverInfo.Location = new Point(15, 160);
            btnSelectReceiverInfo.Name = "btnSelectReceiverInfo";
            btnSelectReceiverInfo.Size = new Size(250, 35);
            btnSelectReceiverInfo.TabIndex = 4;
            btnSelectReceiverInfo.Text = "✅ Select ReceiverInfo";
            btnSelectReceiverInfo.UseVisualStyleBackColor = true;
            btnSelectReceiverInfo.Click += btnSelectReceiverInfo_Click;
            // 
            // btnSelectSenderInfo
            // 
            btnSelectSenderInfo.Location = new Point(15, 115);
            btnSelectSenderInfo.Name = "btnSelectSenderInfo";
            btnSelectSenderInfo.Size = new Size(250, 35);
            btnSelectSenderInfo.TabIndex = 3;
            btnSelectSenderInfo.Text = "✅ Select SenderInfo";
            btnSelectSenderInfo.UseVisualStyleBackColor = true;
            btnSelectSenderInfo.Click += btnSelectSenderInfo_Click;
            // 
            // btnSelectPaymentInfo
            // 
            btnSelectPaymentInfo.Location = new Point(15, 70);
            btnSelectPaymentInfo.Name = "btnSelectPaymentInfo";
            btnSelectPaymentInfo.Size = new Size(250, 35);
            btnSelectPaymentInfo.TabIndex = 2;
            btnSelectPaymentInfo.Text = "✅ Select PaymentInfo";
            btnSelectPaymentInfo.UseVisualStyleBackColor = true;
            btnSelectPaymentInfo.Click += btnSelectPaymentInfo_Click;
            // 
            // btnDeselectAll
            // 
            btnDeselectAll.BackColor = Color.FromArgb(255, 192, 128);
            btnDeselectAll.Location = new Point(140, 25);
            btnDeselectAll.Name = "btnDeselectAll";
            btnDeselectAll.Size = new Size(125, 35);
            btnDeselectAll.TabIndex = 1;
            btnDeselectAll.Text = "❌ Deselect All";
            btnDeselectAll.UseVisualStyleBackColor = false;
            btnDeselectAll.Click += btnDeselectAll_Click;
            // 
            // btnSelectAll
            // 
            btnSelectAll.BackColor = Color.FromArgb(128, 255, 128);
            btnSelectAll.Location = new Point(15, 25);
            btnSelectAll.Name = "btnSelectAll";
            btnSelectAll.Size = new Size(120, 35);
            btnSelectAll.TabIndex = 0;
            btnSelectAll.Text = "✔️ Select All";
            btnSelectAll.UseVisualStyleBackColor = false;
            btnSelectAll.Click += btnSelectAll_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(lblBlacklistInfo);
            groupBox3.Controls.Add(chkUseBlackListOnly);
            groupBox3.Location = new Point(530, 270);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(280, 100);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "🚫 BlackList Configuration";
            // 
            // lblBlacklistInfo
            // 
            lblBlacklistInfo.Location = new Point(15, 55);
            lblBlacklistInfo.Name = "lblBlacklistInfo";
            lblBlacklistInfo.Size = new Size(250, 35);
            lblBlacklistInfo.TabIndex = 1;
            lblBlacklistInfo.Text = "ℹ️ Chỉ dùng tỉnh/thành trong BlackList (cột BlackList = 1)";
            // 
            // chkUseBlackListOnly
            // 
            chkUseBlackListOnly.AutoSize = true;
            chkUseBlackListOnly.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            chkUseBlackListOnly.Location = new Point(15, 25);
            chkUseBlackListOnly.Name = "chkUseBlackListOnly";
            chkUseBlackListOnly.Size = new Size(173, 23);
            chkUseBlackListOnly.TabIndex = 0;
            chkUseBlackListOnly.Text = "🔒 Use BlackList Only";
            chkUseBlackListOnly.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(lblStatus);
            groupBox4.Controls.Add(btnResetDefault);
            groupBox4.Controls.Add(btnClose);
            groupBox4.Controls.Add(btnSaveAndClose);
            groupBox4.Controls.Add(btnSave);
            groupBox4.Location = new Point(12, 518);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(798, 90);
            groupBox4.TabIndex = 3;
            groupBox4.TabStop = false;
            groupBox4.Text = "💾 Save Configuration";
            // 
            // lblStatus
            // 
            lblStatus.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblStatus.Location = new Point(15, 60);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(770, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Ready";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnResetDefault
            // 
            btnResetDefault.BackColor = Color.FromArgb(255, 224, 192);
            btnResetDefault.Location = new Point(600, 25);
            btnResetDefault.Name = "btnResetDefault";
            btnResetDefault.Size = new Size(185, 30);
            btnResetDefault.TabIndex = 3;
            btnResetDefault.Text = "🔄 Reset Default";
            btnResetDefault.UseVisualStyleBackColor = false;
            btnResetDefault.Click += btnResetDefault_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(410, 25);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(180, 30);
            btnClose.TabIndex = 2;
            btnClose.Text = "❌ Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnSaveAndClose
            // 
            btnSaveAndClose.BackColor = Color.FromArgb(128, 255, 255);
            btnSaveAndClose.Location = new Point(210, 25);
            btnSaveAndClose.Name = "btnSaveAndClose";
            btnSaveAndClose.Size = new Size(190, 30);
            btnSaveAndClose.TabIndex = 1;
            btnSaveAndClose.Text = "💾 Save && Close";
            btnSaveAndClose.UseVisualStyleBackColor = false;
            btnSaveAndClose.Click += btnSaveAndClose_Click;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(128, 255, 128);
            btnSave.Location = new Point(15, 25);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(185, 30);
            btnSave.TabIndex = 0;
            btnSave.Text = "💾 Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(btnSetAllNotSend);
            groupBox5.Controls.Add(btnSetAllNull);
            groupBox5.Controls.Add(btnResetFieldModes);
            groupBox5.Controls.Add(lblFieldModeInfo);
            groupBox5.Controls.Add(comboFieldMode);
            groupBox5.Controls.Add(label2);
            groupBox5.Location = new Point(530, 378);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(280, 134);
            groupBox5.TabIndex = 4;
            groupBox5.TabStop = false;
            groupBox5.Text = "🎯 Field Mode (Null/NotSend)";
            // 
            // btnSetAllNotSend
            // 
            btnSetAllNotSend.BackColor = Color.FromArgb(255, 192, 192);
            btnSetAllNotSend.Font = new Font("Segoe UI", 8F);
            btnSetAllNotSend.Location = new Point(180, 95);
            btnSetAllNotSend.Name = "btnSetAllNotSend";
            btnSetAllNotSend.Size = new Size(85, 28);
            btnSetAllNotSend.TabIndex = 5;
            btnSetAllNotSend.Text = "🚫 All NotSend";
            btnSetAllNotSend.UseVisualStyleBackColor = false;
            btnSetAllNotSend.Click += btnSetAllNotSend_Click;
            // 
            // btnSetAllNull
            // 
            btnSetAllNull.BackColor = Color.FromArgb(255, 224, 192);
            btnSetAllNull.Font = new Font("Segoe UI", 8F);
            btnSetAllNull.Location = new Point(95, 95);
            btnSetAllNull.Name = "btnSetAllNull";
            btnSetAllNull.Size = new Size(75, 28);
            btnSetAllNull.TabIndex = 4;
            btnSetAllNull.Text = "⚠️ All Null";
            btnSetAllNull.UseVisualStyleBackColor = false;
            btnSetAllNull.Click += btnSetAllNull_Click;
            // 
            // btnResetFieldModes
            // 
            btnResetFieldModes.BackColor = Color.FromArgb(192, 255, 192);
            btnResetFieldModes.Font = new Font("Segoe UI", 8F);
            btnResetFieldModes.Location = new Point(15, 95);
            btnResetFieldModes.Name = "btnResetFieldModes";
            btnResetFieldModes.Size = new Size(70, 28);
            btnResetFieldModes.TabIndex = 3;
            btnResetFieldModes.Text = "🔄 Reset";
            btnResetFieldModes.UseVisualStyleBackColor = false;
            btnResetFieldModes.Click += btnResetFieldModes_Click;
            // 
            // lblFieldModeInfo
            // 
            lblFieldModeInfo.Font = new Font("Segoe UI", 8F, FontStyle.Italic);
            lblFieldModeInfo.ForeColor = Color.Gray;
            lblFieldModeInfo.Location = new Point(15, 68);
            lblFieldModeInfo.Name = "lblFieldModeInfo";
            lblFieldModeInfo.Size = new Size(250, 20);
            lblFieldModeInfo.TabIndex = 2;
            lblFieldModeInfo.Text = "Select a field to change mode";
            // 
            // comboFieldMode
            // 
            comboFieldMode.DropDownStyle = ComboBoxStyle.DropDownList;
            comboFieldMode.Enabled = false;
            comboFieldMode.FormattingEnabled = true;
            comboFieldMode.Location = new Point(15, 40);
            comboFieldMode.Name = "comboFieldMode";
            comboFieldMode.Size = new Size(250, 23);
            comboFieldMode.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(15, 22);
            label2.Name = "label2";
            label2.Size = new Size(146, 15);
            label2.TabIndex = 0;
            label2.Text = "🎛️ Mode for selected field:";
            // 
            // chkAutoCancelIfNotPaid
            // 
            chkAutoCancelIfNotPaid.AutoSize = true;
            chkAutoCancelIfNotPaid.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkAutoCancelIfNotPaid.ForeColor = Color.Red;
            chkAutoCancelIfNotPaid.Location = new Point(12, 614);
            chkAutoCancelIfNotPaid.Name = "chkAutoCancelIfNotPaid";
            chkAutoCancelIfNotPaid.Size = new Size(433, 19);
            chkAutoCancelIfNotPaid.TabIndex = 6;
            chkAutoCancelIfNotPaid.Text = "🚫 Auto Cancel - Tự động hủy giao dịch ngay khi Transfer trả về NOT PAID";
            chkAutoCancelIfNotPaid.UseVisualStyleBackColor = true;
            // 
            // FormSettings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(822, 638);
            Controls.Add(groupBox5);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(chkAutoCancelIfNotPaid);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "FormSettings";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "⚙️ Settings - Fields, BlackList & Null Mode";
            FormClosing += FormSettings_FormClosing;
            Load += FormSetting_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox chkFields;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnSelectReceiverInfo;
        private System.Windows.Forms.Button btnSelectSenderInfo;
        private System.Windows.Forms.Button btnSelectPaymentInfo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkUseBlackListOnly;
        private System.Windows.Forms.Label lblBlacklistInfo;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnSaveAndClose;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnResetDefault;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox comboFieldMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblFieldModeInfo;
        private System.Windows.Forms.Button btnResetFieldModes;
        private System.Windows.Forms.Button btnSetAllNull;
        private System.Windows.Forms.Button btnSetAllNotSend;
        private System.Windows.Forms.CheckBox chkAutoCancelIfNotPaid;
    }
}