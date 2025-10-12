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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkFields = new System.Windows.Forms.CheckedListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnSelectReceiverInfo = new System.Windows.Forms.Button();
            this.btnSelectSenderInfo = new System.Windows.Forms.Button();
            this.btnSelectPaymentInfo = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblBlacklistInfo = new System.Windows.Forms.Label();
            this.chkUseBlackListOnly = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnResetDefault = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSaveAndClose = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnSetAllNotSend = new System.Windows.Forms.Button();
            this.btnSetAllNull = new System.Windows.Forms.Button();
            this.btnResetFieldModes = new System.Windows.Forms.Button();
            this.lblFieldModeInfo = new System.Windows.Forms.Label();
            this.comboFieldMode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkFields);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(500, 500);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "📋 Chọn Fields để gửi trong Request";
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtSearch.Location = new System.Drawing.Point(80, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Tìm kiếm field...";
            this.txtSearch.Size = new System.Drawing.Size(410, 25);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "🔍 Search:";
            // 
            // chkFields
            // 
            this.chkFields.CheckOnClick = true;
            this.chkFields.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.chkFields.FormattingEnabled = true;
            this.chkFields.Location = new System.Drawing.Point(15, 60);
            this.chkFields.Name = "chkFields";
            this.chkFields.Size = new System.Drawing.Size(475, 418);
            this.chkFields.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnSelectReceiverInfo);
            this.groupBox2.Controls.Add(this.btnSelectSenderInfo);
            this.groupBox2.Controls.Add(this.btnSelectPaymentInfo);
            this.groupBox2.Controls.Add(this.btnDeselectAll);
            this.groupBox2.Controls.Add(this.btnSelectAll);
            this.groupBox2.Location = new System.Drawing.Point(530, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(280, 250);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "⚡ Quick Actions";
            // 
            // btnSelectReceiverInfo
            // 
            this.btnSelectReceiverInfo.Location = new System.Drawing.Point(15, 160);
            this.btnSelectReceiverInfo.Name = "btnSelectReceiverInfo";
            this.btnSelectReceiverInfo.Size = new System.Drawing.Size(250, 35);
            this.btnSelectReceiverInfo.TabIndex = 4;
            this.btnSelectReceiverInfo.Text = "✅ Select ReceiverInfo";
            this.btnSelectReceiverInfo.UseVisualStyleBackColor = true;
            this.btnSelectReceiverInfo.Click += new System.EventHandler(this.btnSelectReceiverInfo_Click);
            // 
            // btnSelectSenderInfo
            // 
            this.btnSelectSenderInfo.Location = new System.Drawing.Point(15, 115);
            this.btnSelectSenderInfo.Name = "btnSelectSenderInfo";
            this.btnSelectSenderInfo.Size = new System.Drawing.Size(250, 35);
            this.btnSelectSenderInfo.TabIndex = 3;
            this.btnSelectSenderInfo.Text = "✅ Select SenderInfo";
            this.btnSelectSenderInfo.UseVisualStyleBackColor = true;
            this.btnSelectSenderInfo.Click += new System.EventHandler(this.btnSelectSenderInfo_Click);
            // 
            // btnSelectPaymentInfo
            // 
            this.btnSelectPaymentInfo.Location = new System.Drawing.Point(15, 70);
            this.btnSelectPaymentInfo.Name = "btnSelectPaymentInfo";
            this.btnSelectPaymentInfo.Size = new System.Drawing.Size(250, 35);
            this.btnSelectPaymentInfo.TabIndex = 2;
            this.btnSelectPaymentInfo.Text = "✅ Select PaymentInfo";
            this.btnSelectPaymentInfo.UseVisualStyleBackColor = true;
            this.btnSelectPaymentInfo.Click += new System.EventHandler(this.btnSelectPaymentInfo_Click);
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.btnDeselectAll.Location = new System.Drawing.Point(140, 25);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(125, 35);
            this.btnDeselectAll.TabIndex = 1;
            this.btnDeselectAll.Text = "❌ Deselect All";
            this.btnDeselectAll.UseVisualStyleBackColor = false;
            this.btnDeselectAll.Click += new System.EventHandler(this.btnDeselectAll_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnSelectAll.Location = new System.Drawing.Point(15, 25);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(120, 35);
            this.btnSelectAll.TabIndex = 0;
            this.btnSelectAll.Text = "✔️ Select All";
            this.btnSelectAll.UseVisualStyleBackColor = false;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.lblBlacklistInfo);
            this.groupBox3.Controls.Add(this.chkUseBlackListOnly);
            this.groupBox3.Location = new System.Drawing.Point(530, 270);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(280, 100);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "🚫 BlackList Configuration";
            // 
            // lblBlacklistInfo
            // 
            this.lblBlacklistInfo.Location = new System.Drawing.Point(15, 55);
            this.lblBlacklistInfo.Name = "lblBlacklistInfo";
            this.lblBlacklistInfo.Size = new System.Drawing.Size(250, 35);
            this.lblBlacklistInfo.TabIndex = 1;
            this.lblBlacklistInfo.Text = "ℹ️ Chỉ dùng tỉnh/thành trong BlackList (cột BlackList = 1)";
            // 
            // chkUseBlackListOnly
            // 
            this.chkUseBlackListOnly.AutoSize = true;
            this.chkUseBlackListOnly.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.chkUseBlackListOnly.Location = new System.Drawing.Point(15, 25);
            this.chkUseBlackListOnly.Name = "chkUseBlackListOnly";
            this.chkUseBlackListOnly.Size = new System.Drawing.Size(165, 23);
            this.chkUseBlackListOnly.TabIndex = 0;
            this.chkUseBlackListOnly.Text = "🔒 Use BlackList Only";
            this.chkUseBlackListOnly.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblStatus);
            this.groupBox4.Controls.Add(this.btnResetDefault);
            this.groupBox4.Controls.Add(this.btnClose);
            this.groupBox4.Controls.Add(this.btnSaveAndClose);
            this.groupBox4.Controls.Add(this.btnSave);
            this.groupBox4.Location = new System.Drawing.Point(12, 520);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(798, 90);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "💾 Save Configuration";
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.Location = new System.Drawing.Point(15, 60);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(770, 20);
            this.lblStatus.TabIndex = 4;
            this.lblStatus.Text = "Ready";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnResetDefault
            // 
            this.btnResetDefault.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnResetDefault.Location = new System.Drawing.Point(600, 25);
            this.btnResetDefault.Name = "btnResetDefault";
            this.btnResetDefault.Size = new System.Drawing.Size(185, 30);
            this.btnResetDefault.TabIndex = 3;
            this.btnResetDefault.Text = "🔄 Reset Default";
            this.btnResetDefault.UseVisualStyleBackColor = false;
            this.btnResetDefault.Click += new System.EventHandler(this.btnResetDefault_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(410, 25);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(180, 30);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "❌ Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSaveAndClose
            // 
            this.btnSaveAndClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnSaveAndClose.Location = new System.Drawing.Point(210, 25);
            this.btnSaveAndClose.Name = "btnSaveAndClose";
            this.btnSaveAndClose.Size = new System.Drawing.Size(190, 30);
            this.btnSaveAndClose.TabIndex = 1;
            this.btnSaveAndClose.Text = "💾 Save && Close";
            this.btnSaveAndClose.UseVisualStyleBackColor = false;
            this.btnSaveAndClose.Click += new System.EventHandler(this.btnSaveAndClose_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnSave.Location = new System.Drawing.Point(15, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(185, 30);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "💾 Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnSetAllNotSend);
            this.groupBox5.Controls.Add(this.btnSetAllNull);
            this.groupBox5.Controls.Add(this.btnResetFieldModes);
            this.groupBox5.Controls.Add(this.lblFieldModeInfo);
            this.groupBox5.Controls.Add(this.comboFieldMode);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Location = new System.Drawing.Point(530, 378);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(280, 134);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "🎯 Field Mode (Null/NotSend)";
            // 
            // btnSetAllNotSend
            // 
            this.btnSetAllNotSend.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnSetAllNotSend.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnSetAllNotSend.Location = new System.Drawing.Point(180, 95);
            this.btnSetAllNotSend.Name = "btnSetAllNotSend";
            this.btnSetAllNotSend.Size = new System.Drawing.Size(85, 28);
            this.btnSetAllNotSend.TabIndex = 5;
            this.btnSetAllNotSend.Text = "🚫 All NotSend";
            this.btnSetAllNotSend.UseVisualStyleBackColor = false;
            this.btnSetAllNotSend.Click += new System.EventHandler(this.btnSetAllNotSend_Click);
            // 
            // btnSetAllNull
            // 
            this.btnSetAllNull.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnSetAllNull.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnSetAllNull.Location = new System.Drawing.Point(95, 95);
            this.btnSetAllNull.Name = "btnSetAllNull";
            this.btnSetAllNull.Size = new System.Drawing.Size(75, 28);
            this.btnSetAllNull.TabIndex = 4;
            this.btnSetAllNull.Text = "⚠️ All Null";
            this.btnSetAllNull.UseVisualStyleBackColor = false;
            this.btnSetAllNull.Click += new System.EventHandler(this.btnSetAllNull_Click);
            // 
            // btnResetFieldModes
            // 
            this.btnResetFieldModes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnResetFieldModes.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnResetFieldModes.Location = new System.Drawing.Point(15, 95);
            this.btnResetFieldModes.Name = "btnResetFieldModes";
            this.btnResetFieldModes.Size = new System.Drawing.Size(70, 28);
            this.btnResetFieldModes.TabIndex = 3;
            this.btnResetFieldModes.Text = "🔄 Reset";
            this.btnResetFieldModes.UseVisualStyleBackColor = false;
            this.btnResetFieldModes.Click += new System.EventHandler(this.btnResetFieldModes_Click);
            // 
            // lblFieldModeInfo
            // 
            this.lblFieldModeInfo.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Italic);
            this.lblFieldModeInfo.ForeColor = System.Drawing.Color.Gray;
            this.lblFieldModeInfo.Location = new System.Drawing.Point(15, 68);
            this.lblFieldModeInfo.Name = "lblFieldModeInfo";
            this.lblFieldModeInfo.Size = new System.Drawing.Size(250, 20);
            this.lblFieldModeInfo.TabIndex = 2;
            this.lblFieldModeInfo.Text = "Select a field to change mode";
            // 
            // comboFieldMode
            // 
            this.comboFieldMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFieldMode.Enabled = false;
            this.comboFieldMode.FormattingEnabled = true;
            this.comboFieldMode.Location = new System.Drawing.Point(15, 40);
            this.comboFieldMode.Name = "comboFieldMode";
            this.comboFieldMode.Size = new System.Drawing.Size(250, 23);
            this.comboFieldMode.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(154, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "🎛️ Mode for selected field:";
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 622);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "⚙️ Settings - Fields, BlackList & Null Mode";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSettings_FormClosing);
            this.Load += new System.EventHandler(this.FormSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
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
    }
}