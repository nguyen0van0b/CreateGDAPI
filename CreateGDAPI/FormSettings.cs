using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace CreateGDAPI
{
    public class FieldsConfig
    {
        public List<string> SelectedFields { get; set; } = new();
        public bool UseBlackListOnly { get; set; } = false;
    }

    public partial class FormSettings : Form
    {
        private string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fieldsConfig.json");

        public FormSettings()
        {
            InitializeComponent();
        }

        private void FormSettings_Load(object sender, EventArgs e)
        {
            // Thêm tất cả các fields vào CheckedListBox
            chkFields.Items.AddRange(new string[]
            {
                // PaymentInfo
                "paymentInfo.exchangeRate",
                "paymentInfo.feeAmount",
                "paymentInfo.feeCurrency",
                
                // SenderInfo
                "senderInfo.phoneNumber",
                "senderInfo.documentType",
                "senderInfo.idNumber",
                "senderInfo.issueDate",
                "senderInfo.issuer",
                "senderInfo.nationality",
                "senderInfo.gender",
                "senderInfo.doB",
                "senderInfo.address",
                "senderInfo.country",
                "senderInfo.transferPurpose",
                "senderInfo.fundSource",
                "senderInfo.recipientRelationship",
                "senderInfo.content",
                
                // ReceiverInfo
                "receiverInfo.address",
                "receiverInfo.fullName2",
                "receiverInfo.phoneNumber2",
                "receiverInfo.address2",
                "receiverInfo.idNumber",
                "receiverInfo.issueDate",
                "receiverInfo.issuer",
                "receiverInfo.nationality",
                "receiverInfo.gender",
                "receiverInfo.doB",
                "receiverInfo.ethnicity",
                "receiverInfo.occupation",
                "receiverInfo.province",
                "receiverInfo.ward",
                "receiverInfo.transferPurpose",
                "receiverInfo.senderRelationship",
                "receiverInfo.accountNumber",
                "receiverInfo.bankCode",
                "receiverInfo.bankBranchCode"
            });

            // Load cấu hình đã lưu
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (!File.Exists(configPath))
            {
                // Mặc định check tất cả
                for (int i = 0; i < chkFields.Items.Count; i++)
                {
                    chkFields.SetItemChecked(i, true);
                }
                return;
            }

            try
            {
                string json = File.ReadAllText(configPath);
                var config = JsonSerializer.Deserialize<FieldsConfig>(json);

                if (config == null) return;

                // Set checked items
                for (int i = 0; i < chkFields.Items.Count; i++)
                {
                    string item = chkFields.Items[i].ToString();
                    chkFields.SetItemChecked(i, config.SelectedFields.Contains(item));
                }

                // Set blacklist checkbox
                chkUseBlackListOnly.Checked = config.UseBlackListOnly;

                lblStatus.Text = "✅ Đã load cấu hình thành công";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load cấu hình: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveConfig()
        {
            try
            {
                var config = new FieldsConfig
                {
                    UseBlackListOnly = chkUseBlackListOnly.Checked
                };

                foreach (var item in chkFields.CheckedItems)
                {
                    config.SelectedFields.Add(item.ToString());
                }

                string json = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(configPath, json);

                lblStatus.Text = "✅ Đã lưu cấu hình thành công";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi lưu cấu hình: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "❌ Lưu cấu hình thất bại";
                lblStatus.ForeColor = System.Drawing.Color.Red;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkFields.Items.Count; i++)
            {
                chkFields.SetItemChecked(i, true);
            }
        }

        private void btnDeselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < chkFields.Items.Count; i++)
            {
                chkFields.SetItemChecked(i, false);
            }
        }

        private void btnSelectPaymentInfo_Click(object sender, EventArgs e)
        {
            SelectByPrefix("paymentInfo.");
        }

        private void btnSelectSenderInfo_Click(object sender, EventArgs e)
        {
            SelectByPrefix("senderInfo.");
        }

        private void btnSelectReceiverInfo_Click(object sender, EventArgs e)
        {
            SelectByPrefix("receiverInfo.");
        }

        private void SelectByPrefix(string prefix)
        {
            for (int i = 0; i < chkFields.Items.Count; i++)
            {
                string item = chkFields.Items[i].ToString();
                if (item.StartsWith(prefix))
                {
                    chkFields.SetItemChecked(i, true);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSaveAndClose_Click(object sender, EventArgs e)
        {
            SaveConfig();
            System.Threading.Thread.Sleep(500); // Delay để user thấy message
            this.Close();
        }

        // Phương thức static để load config từ form khác
        public static FieldsConfig LoadFieldsConfig()
        {
            string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fieldsConfig.json");

            if (!File.Exists(configPath))
            {
                // Trả về config mặc định: tất cả fields được chọn
                var defaultConfig = new FieldsConfig { UseBlackListOnly = false };

                // Add all fields
                string[] allFields = new string[]
                {
                    "paymentInfo.exchangeRate","paymentInfo.feeAmount","paymentInfo.feeCurrency",
                    "senderInfo.phoneNumber","senderInfo.documentType","senderInfo.idNumber",
                    "senderInfo.issueDate","senderInfo.issuer","senderInfo.nationality",
                    "senderInfo.gender","senderInfo.doB","senderInfo.address",
                    "senderInfo.country","senderInfo.transferPurpose","senderInfo.fundSource",
                    "senderInfo.recipientRelationship","senderInfo.content",
                    "receiverInfo.address","receiverInfo.fullName2","receiverInfo.phoneNumber2",
                    "receiverInfo.address2","receiverInfo.idNumber","receiverInfo.issueDate",
                    "receiverInfo.issuer","receiverInfo.nationality","receiverInfo.gender","receiverInfo.doB",
                    "receiverInfo.ethnicity","receiverInfo.occupation","receiverInfo.province","receiverInfo.ward",
                    "receiverInfo.transferPurpose","receiverInfo.senderRelationship",
                    "receiverInfo.accountNumber","receiverInfo.bankCode","receiverInfo.bankBranchCode"
                };

                defaultConfig.SelectedFields.AddRange(allFields);
                return defaultConfig;
            }

            try
            {
                string json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<FieldsConfig>(json) ?? new FieldsConfig();
            }
            catch
            {
                return new FieldsConfig();
            }
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Tự động lưu khi đóng form
            if (MessageBox.Show("Bạn có muốn lưu cấu hình trước khi đóng?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SaveConfig();
            }
        }

        private void btnResetDefault_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn reset về cấu hình mặc định (chọn tất cả)?",
                "Xác nhận",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                // Select all fields
                for (int i = 0; i < chkFields.Items.Count; i++)
                {
                    chkFields.SetItemChecked(i, true);
                }

                // Uncheck blacklist
                chkUseBlackListOnly.Checked = false;

                lblStatus.Text = "✅ Đã reset về cấu hình mặc định";
                lblStatus.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                // Hiển thị lại tất cả
                chkFields.Items.Clear();
                chkFields.Items.AddRange(GetAllFields());
                LoadConfig(); // Restore checked state
                return;
            }

            // Filter items
            var allFields = GetAllFields();
            var filtered = allFields.Where(f => f.ToLower().Contains(searchText)).ToArray();

            chkFields.Items.Clear();
            chkFields.Items.AddRange(filtered);

            // Restore checked state
            LoadConfig();
        }

        private string[] GetAllFields()
        {
            return new string[]
            {
                "paymentInfo.exchangeRate","paymentInfo.feeAmount","paymentInfo.feeCurrency",
                "senderInfo.phoneNumber","senderInfo.documentType","senderInfo.idNumber",
                "senderInfo.issueDate","senderInfo.issuer","senderInfo.nationality",
                "senderInfo.gender","senderInfo.doB","senderInfo.address",
                "senderInfo.country","senderInfo.transferPurpose","senderInfo.fundSource",
                "senderInfo.recipientRelationship","senderInfo.content",
                "receiverInfo.address","receiverInfo.fullName2","receiverInfo.phoneNumber2",
                "receiverInfo.address2","receiverInfo.idNumber","receiverInfo.issueDate",
                "receiverInfo.issuer","receiverInfo.nationality","receiverInfo.gender","receiverInfo.doB",
                "receiverInfo.ethnicity","receiverInfo.occupation","receiverInfo.province","receiverInfo.ward",
                "receiverInfo.transferPurpose","receiverInfo.senderRelationship",
                "receiverInfo.accountNumber","receiverInfo.bankCode","receiverInfo.bankBranchCode"
            };
        }
    }
}