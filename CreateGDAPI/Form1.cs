using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using ClosedXML.Excel;
using System.Text.Json;
using System.IO;
using System.Linq;

namespace CreateGDAPI
{
    public partial class Form1 : Form
    {
        private readonly HttpClientHandler handler;
        private readonly HttpClient client;
        private readonly Random rnd = new Random();

        private List<(string Code, string Branch)> _banks = new();
        private List<string> _wards = new();
        private List<string> _provinces = new();
        private List<string> _countries = new();

        private readonly List<string> _occupations = new() { "Engineer", "Teacher", "Doctor", "Farmer", "Student", "Worker", "Nurse" };
        private readonly List<string> _relationships = new() { "Friend", "Brother", "Sister", "Colleague", "Parent", "Relative" };
        private readonly List<string> _purposes = new() { "Gift", "Payment", "Support", "Loan", "Investment" };
        private readonly List<string> _fundSources = new() { "Salary", "Savings", "Business", "Allowance" };
        private readonly List<string> _contents = new() { "Thanh toán hóa đơn", "Gửi quà", "Hỗ trợ tài chính", "Mua hàng online" };

        public Form1()
        {
            InitializeComponent();

            handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            client = new HttpClient(handler);

            comboServiceType.Items.AddRange(new string[] { "AD", "WD", "CP", "HD" });
            comboCurrency.Items.AddRange(new string[] { "VND", "USD" });
            comboServiceType.SelectedIndex = 0;
            comboCurrency.SelectedIndex = 0;

            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "re");
            _banks = LoadBankCodes(Path.Combine(basePath, "MasterBanksList.xlsx"));
            _wards = LoadListFromExcel(Path.Combine(basePath, "MasterWardsList.xlsx"), 1);
            _provinces = LoadListFromExcel(Path.Combine(basePath, "MasterProvincesList.xlsx"), 1);
            _countries = LoadListFromExcel(Path.Combine(basePath, "MasterCountriesList.xlsx"), 1);

            // nạp field vào checkedListBox
            chkFields.Items.AddRange(new string[]
            {
                "paymentInfo.exchangeRate","paymentInfo.feeAmount","paymentInfo.feeCurrency",
                "senderInfo.phoneNumber","senderInfo.documentType","senderInfo.idNumber",
                "senderInfo.issueDate","senderInfo.issuer","senderInfo.nationality",
                "senderInfo.gender","senderInfo.doB","senderInfo.address","senderInfo.city",
                "senderInfo.country","senderInfo.transferPurpose","senderInfo.fundSource",
                "senderInfo.recipientRelationship","senderInfo.content",
                "receiverInfo.address","receiverInfo.fullName2","receiverInfo.phoneNumber2",
                "receiverInfo.address2","receiverInfo.idNumber","receiverInfo.issueDate",
                "receiverInfo.issuer","receiverInfo.nationality","receiverInfo.gender","receiverInfo.doB",
                "receiverInfo.ethnicity","receiverInfo.occupation","receiverInfo.province","receiverInfo.ward",
                "receiverInfo.transferPurpose","receiverInfo.senderRelationship",
                "receiverInfo.accountNumber","receiverInfo.bankCode","receiverInfo.bankBranchCode"
            });

            for (int i = 0; i < chkFields.Items.Count; i++)
                chkFields.SetItemChecked(i, true);
        }

        private bool FieldSelected(string field) => chkFields.CheckedItems.Contains(field);

        private List<(string, string)> LoadBankCodes(string filePath)
        {
            var list = new List<(string, string)>();
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var ws = workbook.Worksheet(1);
                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    string code = row.Cell(1).GetString();
                    string branch = row.Cell(4).GetString();
                    if (!string.IsNullOrWhiteSpace(code) && code.All(Char.IsDigit))
                        list.Add((code.Trim(), branch.Trim()));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể load BankCodes: " + ex.Message);
            }
            return list;
        }

        private List<string> LoadListFromExcel(string filePath, int colIndex)
        {
            var list = new List<string>();
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var ws = workbook.Worksheet(1);
                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    string val = row.Cell(colIndex).GetString().Trim();
                    if (!string.IsNullOrEmpty(val)) list.Add(val);
                }
            }
            catch { }
            return list;
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            string partnerCode = txtPartnerCode.Text.Trim();
            string agencyCode = txtAgencyCode.Text.Trim();
            string serviceType = comboServiceType.SelectedItem?.ToString() ?? "AD";
            string currency = comboCurrency.SelectedItem?.ToString() ?? "VND";

            if (string.IsNullOrEmpty(partnerCode) || string.IsNullOrEmpty(agencyCode))
            {
                MessageBox.Show("Vui lòng nhập PartnerCode và AgencyCode");
                return;
            }
            if (!int.TryParse(txtSoLuong.Text.Trim(), out int soLuong) || soLuong <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng hợp lệ");
                return;
            }

            txtResult.Clear();
            for (int i = 1; i <= soLuong; i++)
            {
                string json = TaoJson(partnerCode, agencyCode, serviceType, currency);
                await GuiApi(json, i);
            }
        }

        private string TaoJson(string partnerCode, string agencyCode, string serviceType, string currency)
        {
            string refNo = "RefNo-" + agencyCode + GenerateRandomNumber(6);
            string partnerRef = "PartnerRef-" + agencyCode + GenerateRandomNumber(6);
            string pin = "PIN-" + agencyCode + GenerateRandomNumber(6);

            string senderName = GenerateRandomName();
            string receiverName = GenerateRandomName();
            string phone = "0" + GenerateRandomNumber(9);
            string idNumber = GenerateRandomNumber(12);
            string accNumber = GenerateRandomNumber(16);

            var bank = _banks[rnd.Next(_banks.Count)];
            string bankCode = bank.Code;
            string bankBranch = bank.Branch + rnd.Next(1, 100).ToString("D2");
            string amount = GenerateRandomNumber(7) + ".00";

            var root = new Dictionary<string, object>
            {
                ["refNo"] = refNo,
                ["partnerCode"] = partnerCode,
                ["agencyCode"] = agencyCode,
                ["partnerRef"] = partnerRef,
                ["pin"] = pin,
                ["serviceType"] = serviceType
            };

            // paymentInfo
            var paymentInfo = new Dictionary<string, object>
            {
                ["debtAmount"] = amount,
                ["debtCurrency"] = currency,
                ["disbursementAmount"] = amount,
                ["disbursementCurrency"] = currency
            };
            if (FieldSelected("paymentInfo.exchangeRate")) paymentInfo["exchangeRate"] = "1.0";
            if (FieldSelected("paymentInfo.feeAmount")) paymentInfo["feeAmount"] = "100.00";
            if (FieldSelected("paymentInfo.feeCurrency")) paymentInfo["feeCurrency"] = currency;
            root["paymentInfo"] = paymentInfo;

            // senderInfo
            var senderInfo = new Dictionary<string, object>
            {
                ["fullName"] = senderName
            };
            if (FieldSelected("senderInfo.phoneNumber")) senderInfo["phoneNumber"] = phone;
            if (FieldSelected("senderInfo.documentType")) senderInfo["documentType"] = "ID";
            if (FieldSelected("senderInfo.idNumber")) senderInfo["idNumber"] = idNumber;
            if (FieldSelected("senderInfo.issueDate")) senderInfo["issueDate"] = RandomDate(2020, 2023);
            if (FieldSelected("senderInfo.issuer")) senderInfo["issuer"] = "Gov";
            if (FieldSelected("senderInfo.nationality")) senderInfo["nationality"] = _countries[rnd.Next(_countries.Count)];
            if (FieldSelected("senderInfo.gender")) senderInfo["gender"] = rnd.Next(2) == 0 ? "M" : "F";
            if (FieldSelected("senderInfo.doB")) senderInfo["doB"] = RandomDate(1970, 2000);
            if (FieldSelected("senderInfo.address")) senderInfo["address"] = "ĐC " + _wards[rnd.Next(_wards.Count)];
            if (FieldSelected("senderInfo.city")) senderInfo["city"] = _provinces[rnd.Next(_provinces.Count)];
            if (FieldSelected("senderInfo.country")) senderInfo["country"] = _countries[rnd.Next(_countries.Count)];
            if (FieldSelected("senderInfo.transferPurpose")) senderInfo["transferPurpose"] = _purposes[rnd.Next(_purposes.Count)];
            if (FieldSelected("senderInfo.fundSource")) senderInfo["fundSource"] = _fundSources[rnd.Next(_fundSources.Count)];
            if (FieldSelected("senderInfo.recipientRelationship")) senderInfo["recipientRelationship"] = _relationships[rnd.Next(_relationships.Count)];
            if (FieldSelected("senderInfo.content")) senderInfo["content"] = _contents[rnd.Next(_contents.Count)];
            root["senderInfo"] = senderInfo;

            // receiverInfo
            var receiverInfo = new Dictionary<string, object>
            {
                ["fullName"] = receiverName,
                ["phoneNumber"] = phone,
                ["documentType"] = "CCCD" // luôn CCCD
            };
            if (FieldSelected("receiverInfo.address")) receiverInfo["address"] = "ĐC " + _wards[rnd.Next(_wards.Count)];
            if (FieldSelected("receiverInfo.fullName2")) receiverInfo["fullName2"] = GenerateRandomName();
            if (FieldSelected("receiverInfo.phoneNumber2")) receiverInfo["phoneNumber2"] = "09" + GenerateRandomNumber(8);
            if (FieldSelected("receiverInfo.address2")) receiverInfo["address2"] = "ĐC " + _wards[rnd.Next(_wards.Count)];
            if (FieldSelected("receiverInfo.idNumber")) receiverInfo["idNumber"] = idNumber;
            if (FieldSelected("receiverInfo.issueDate")) receiverInfo["issueDate"] = RandomDate(2020, 2023);
            if (FieldSelected("receiverInfo.issuer")) receiverInfo["issuer"] = "Gov";
            if (FieldSelected("receiverInfo.nationality")) receiverInfo["nationality"] = _countries[rnd.Next(_countries.Count)];
            if (FieldSelected("receiverInfo.gender")) receiverInfo["gender"] = rnd.Next(2) == 0 ? "M" : "F";
            if (FieldSelected("receiverInfo.doB")) receiverInfo["doB"] = RandomDate(1985, 2005);
            if (FieldSelected("receiverInfo.ethnicity")) receiverInfo["ethnicity"] = "Kinh";
            if (FieldSelected("receiverInfo.occupation")) receiverInfo["occupation"] = _occupations[rnd.Next(_occupations.Count)];
            if (FieldSelected("receiverInfo.province")) receiverInfo["province"] = _provinces[rnd.Next(_provinces.Count)];
            if (FieldSelected("receiverInfo.ward")) receiverInfo["ward"] = _wards[rnd.Next(_wards.Count)];
            if (FieldSelected("receiverInfo.transferPurpose")) receiverInfo["transferPurpose"] = _purposes[rnd.Next(_purposes.Count)];
            if (FieldSelected("receiverInfo.senderRelationship")) receiverInfo["senderRelationship"] = _relationships[rnd.Next(_relationships.Count)];
            if (FieldSelected("receiverInfo.accountNumber")) receiverInfo["accountNumber"] = accNumber;
            if (FieldSelected("receiverInfo.bankCode")) receiverInfo["bankCode"] = bankCode;
            if (FieldSelected("receiverInfo.bankBranchCode")) receiverInfo["bankBranchCode"] = bankBranch;
            root["receiverInfo"] = receiverInfo;

            return JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
        }

        private async Task GuiApi(string json, int stt)
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://58.186.16.67/api/partner/transfer", content);
                string result = await response.Content.ReadAsStringAsync();

                string logText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] #{stt}\r\nREQUEST:\r\n{json}\r\nRESPONSE: {response.StatusCode}\r\n{result}\r\n-------------------\r\n";
                AppendResult(logText);
                WriteLogToFile(logText);
            }
            catch (Exception ex)
            {
                string logText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] #{stt} ERROR: {ex.Message}\r\n-------------------\r\n";
                AppendResult(logText);
                WriteLogToFile(logText);
            }
        }

        private string GenerateRandomName()
        {
            string[] ho = { "Nguyen", "Tran", "Le", "Pham", "Hoang" };
            string[] ten = { "An", "Binh", "Cuong", "Dung", "Dong", "Hanh", "Lan", "Mai" };
            return ho[rnd.Next(ho.Length)] + " " + ten[rnd.Next(ten.Length)];
        }

        private string GenerateRandomNumber(int length)
        {
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++) sb.Append(rnd.Next(0, 10));
            return sb.ToString();
        }

        private string RandomDate(int startYear, int endYear)
        {
            DateTime start = new DateTime(startYear, 1, 1);
            DateTime end = new DateTime(endYear, 12, 31);
            int range = (end - start).Days;
            return start.AddDays(rnd.Next(range)).ToString("yyyy-MM-dd");
        }
        private void AppendResult(string text)
        {
            if (txtResult.InvokeRequired)
                txtResult.Invoke(new Action(() => txtResult.AppendText(text)));
            else
                txtResult.AppendText(text);
        }

        private void WriteLogToFile(string text)
        {
            try
            {
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "re");
                if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);
                string logPath = Path.Combine(logDir, "logs.txt");
                File.AppendAllText(logPath, text + Environment.NewLine, Encoding.UTF8);
            }
            catch { /* ignore lỗi ghi log để tool không bị crash */ }
        }

    }
}
