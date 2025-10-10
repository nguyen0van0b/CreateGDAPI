namespace CreateGDAPI
{
    partial class Form2
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtPartnerCode;
        private TextBox txtAgencyCode;
        private ComboBox comboServiceType;
        private ComboBox comboCurrency;
        private TextBox txtSoLuong;
        private Button btnSend;
        private Button btnSendBlackList;
        private TextBox txtResult;
        private GroupBox grpFields;
        private CheckedListBox chkFields;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtPartnerCode = new TextBox();
            txtAgencyCode = new TextBox();
            comboServiceType = new ComboBox();
            comboCurrency = new ComboBox();
            txtSoLuong = new TextBox();
            btnSend = new Button();
            btnSendBlackList = new Button();
            txtResult = new TextBox();
            grpFields = new GroupBox();
            chkFields = new CheckedListBox();
            grpFields.SuspendLayout();
            SuspendLayout();
            // 
            // txtPartnerCode
            // 
            txtPartnerCode.Location = new Point(20, 20);
            txtPartnerCode.Name = "txtPartnerCode";
            txtPartnerCode.PlaceholderText = "Partner Code";
            txtPartnerCode.Size = new Size(200, 23);
            txtPartnerCode.TabIndex = 1;
            // 
            // txtAgencyCode
            // 
            txtAgencyCode.Location = new Point(20, 60);
            txtAgencyCode.Name = "txtAgencyCode";
            txtAgencyCode.PlaceholderText = "Agency Code";
            txtAgencyCode.Size = new Size(200, 23);
            txtAgencyCode.TabIndex = 2;
            // 
            // comboServiceType
            // 
            comboServiceType.Location = new Point(20, 100);
            comboServiceType.Name = "comboServiceType";
            comboServiceType.Size = new Size(200, 23);
            comboServiceType.TabIndex = 3;
            // 
            // comboCurrency
            // 
            comboCurrency.Location = new Point(20, 140);
            comboCurrency.Name = "comboCurrency";
            comboCurrency.Size = new Size(200, 23);
            comboCurrency.TabIndex = 4;
            // 
            // txtSoLuong
            // 
            txtSoLuong.Location = new Point(20, 180);
            txtSoLuong.Name = "txtSoLuong";
            txtSoLuong.PlaceholderText = "Số lượng giao dịch";
            txtSoLuong.Size = new Size(200, 23);
            txtSoLuong.TabIndex = 5;
            // 
            // btnSend
            // 
            btnSend.Location = new Point(20, 220);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(200, 30);
            btnSend.TabIndex = 6;
            btnSend.Text = "Gửi API (Tất cả)";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // btnSendBlackList
            // 
            btnSendBlackList.Location = new Point(20, 260);
            btnSendBlackList.Name = "btnSendBlackList";
            btnSendBlackList.Size = new Size(200, 30);
            btnSendBlackList.TabIndex = 7;
            btnSendBlackList.Text = "Gửi API (BlackList)";
            btnSendBlackList.UseVisualStyleBackColor = true;
            btnSendBlackList.Click += btnSendBlackList_Click;
            // 
            // txtResult
            // 
            txtResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtResult.Location = new Point(250, 20);
            txtResult.Multiline = true;
            txtResult.Name = "txtResult";
            txtResult.ScrollBars = ScrollBars.Vertical;
            txtResult.Size = new Size(400, 270);
            txtResult.TabIndex = 8;
            // 
            // grpFields
            // 
            grpFields.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            grpFields.Controls.Add(chkFields);
            grpFields.Location = new Point(660, 20);
            grpFields.Name = "grpFields";
            grpFields.Size = new Size(300, 300);
            grpFields.TabIndex = 9;
            grpFields.TabStop = false;
            grpFields.Text = "Chọn các field gửi đi";
            // 
            // chkFields
            // 
            chkFields.CheckOnClick = true;
            chkFields.FormattingEnabled = true;
            chkFields.Location = new Point(10, 20);
            chkFields.Name = "chkFields";
            chkFields.Size = new Size(280, 256);
            chkFields.TabIndex = 0;
            // 
            // Form2
            // 
            ClientSize = new Size(980, 350);
            Controls.Add(grpFields);
            Controls.Add(txtPartnerCode);
            Controls.Add(txtAgencyCode);
            Controls.Add(comboServiceType);
            Controls.Add(comboCurrency);
            Controls.Add(txtSoLuong);
            Controls.Add(btnSend);
            Controls.Add(btnSendBlackList);
            Controls.Add(txtResult);
            Name = "Form2";
            Text = "API Transfer Tool (.NET 8)";
            Load += Form2_Load;
            grpFields.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
