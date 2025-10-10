using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CreateGDAPI
{
    public partial class Form2 : Form
    {
        private readonly HttpClientHandler? handler;
        private readonly HttpClient? client;
        private readonly Random rnd = new Random();
        private bool _useBlackListOnly = false;

        private List<(string Code, string Branch)> _banks = new();
        private List<string> _wards = new();
        private List<string> _provinces = new();
        private List<string> _countries = new();
        private List<string> _provincesBlackList = new();
        private Dictionary<string, List<string>> _wardsByProvinceName = new();
        // Nghề nghiệp (occupations)
        private readonly List<string> _occupations = new()
{
    "Engineer", "Teacher", "Doctor", "Farmer", "Student", "Worker", "Nurse",
    "Police", "Soldier", "Pilot", "Chef", "Driver", "Scientist", "Artist",
    "Musician", "Actor", "Writer", "Photographer", "Athlete", "Lawyer",
    "Accountant", "Designer", "Programmer", "Technician", "Mechanic",
    "Manager", "Consultant", "Entrepreneur", "Barista", "Waiter"
};

        // Quan hệ (relationships)
        private readonly List<string> _relationships = new()
{
    "Friend", "Brother", "Sister", "Colleague", "Parent", "Relative",
    "Uncle", "Aunt", "Cousin", "Grandparent", "Neighbor", "Partner",
    "Boss", "Employee", "Husband", "Wife", "Son", "Daughter", "Classmate", "Mentor"
};

        // Mục đích (purposes)
        private readonly List<string> _purposes = new()
{
    "Gift", "Payment", "Support", "Loan", "Investment",
    "Donation", "Charity", "Study", "Travel", "Shopping",
    "Medical", "Emergency", "Insurance", "Business", "Housing",
    "Transport", "Entertainment", "Debt", "Saving", "Other"
};

        // Nguồn tiền (fundSources)
        private readonly List<string> _fundSources = new()
{
    "Salary", "Savings", "Business", "Allowance",
    "Bonus", "Inheritance", "Pension", "Insurance", "Loan", "Other"
};

        // Nội dung (contents)
        private readonly List<string> _contents = new()
{
    "Thanh toán hóa đơn", "Gửi quà", "Hỗ trợ tài chính", "Mua hàng online",
    "Trả nợ", "Đóng học phí", "Đóng viện phí", "Tiền thuê nhà",
    "Tiền điện", "Tiền nước", "Tiền Internet", "Tiền điện thoại",
    "Đầu tư cổ phiếu", "Đầu tư bất động sản", "Mua bảo hiểm",
    "Du lịch", "Mua vé máy bay", "Đóng phí dịch vụ", "Tiền sinh hoạt", "Tiết kiệm"
};
        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "checkedFields.json");
        private void Form1_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // lưu lại checked khi thoát chương trình
            SaveCheckedItemsToFile(configPath);
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
        private void LoadProvincesWithBlackList(string filePath)
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var ws = workbook.Worksheet(1);
                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    string provinceName = row.Cell(2).GetString().Trim();  // cột 3: Tên tiếng Việt
                    string isBlack = row.Cell(5).GetString().Trim().ToUpper(); // cột 4: BlackList

                    if (!string.IsNullOrEmpty(provinceName))
                    {
                        _provinces.Add(provinceName);
                        if (isBlack == "1" || isBlack == "TRUE" || isBlack == "YES")
                            _provincesBlackList.Add(provinceName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể load danh sách tỉnh/thành: " + ex.Message);
            }
        }
        private void LoadWardsByProvinceName(string filePath)
        {
            try
            {
                using var workbook = new XLWorkbook(filePath);
                var ws = workbook.Worksheet(1);
                foreach (var row in ws.RowsUsed().Skip(1))
                {
                    string wardName = row.Cell(3).GetString().Trim();       // Tên phường tiếng Việt
                    string provinceName = row.Cell(6).GetString().Trim();   // Tên tỉnh tiếng Việt

                    if (string.IsNullOrEmpty(wardName) || string.IsNullOrEmpty(provinceName))
                        continue;

                    if (!_wardsByProvinceName.ContainsKey(provinceName))
                        _wardsByProvinceName[provinceName] = new List<string>();

                    _wardsByProvinceName[provinceName].Add(wardName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể load danh sách phường/xã: " + ex.Message);
            }
        }

        public Form2()
        {
            InitializeComponent();
            if (DesignMode) return;
            handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            client = new HttpClient(handler);
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            comboServiceType.Items.AddRange(new string[] { "AD", "WD", "CP", "HD" });
            comboCurrency.Items.AddRange(new string[] { "VND", "USD" });
            comboServiceType.SelectedIndex = 0;
            comboCurrency.SelectedIndex = 0;

            string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "re");
            _banks = LoadBankCodes(Path.Combine(basePath, "MasterBanksList.xlsx"));
            LoadProvincesWithBlackList(Path.Combine(basePath, "MasterProvincesList.xlsx"));
            LoadWardsByProvinceName(Path.Combine(basePath, "MasterWardsList.xlsx"));
            _countries = LoadListFromExcel(Path.Combine(basePath, "MasterCountriesList.xlsx"), 3);

            // nạp field vào checkedListBox
            chkFields.Items.AddRange(new string[]
            {
                "paymentInfo.exchangeRate","paymentInfo.feeAmount","paymentInfo.feeCurrency",
                "senderInfo.phoneNumber","senderInfo.documentType","senderInfo.idNumber",
                "senderInfo.issueDate","senderInfo.issuer","senderInfo.nationality",
                "senderInfo.gender","senderInfo.doB","senderInfo.address",//"senderInfo.city",
                "senderInfo.country","senderInfo.transferPurpose","senderInfo.fundSource",
                "senderInfo.recipientRelationship","senderInfo.content",
                "receiverInfo.address","receiverInfo.fullName2","receiverInfo.phoneNumber2",
                "receiverInfo.address2","receiverInfo.idNumber","receiverInfo.issueDate",
                "receiverInfo.issuer","receiverInfo.nationality","receiverInfo.gender","receiverInfo.doB",
                "receiverInfo.ethnicity","receiverInfo.occupation","receiverInfo.province","receiverInfo.ward",
                "receiverInfo.transferPurpose","receiverInfo.senderRelationship",
                "receiverInfo.accountNumber","receiverInfo.bankCode","receiverInfo.bankBranchCode"
            });

            // phục hồi checked trước đó
            RestoreCheckedItemsFromFile(configPath);
            this.FormClosing += new FormClosingEventHandler(this.Form1_FormClosing);

        }
        private async void btnSendBlackList_Click(object sender, EventArgs e)
        {
            _useBlackListOnly = true;
            await SendTransactions();
            _useBlackListOnly = false;
        }
        private async Task SendTransactions()
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
            _useBlackListOnly = false;
            await SendTransactions();
        }
        private string TaoJson(string partnerCode, string agencyCode, string serviceType, string currency)
        {
            //string refNo = "RefNo-" + agencyCode + GenerateRandomNumber(6);
            string refNo = Guid.NewGuid().ToString();
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
            bankBranch = bankBranch.Replace(" ", "").Trim();
            if (bankBranch.Length > 8) bankBranch = bankBranch.Substring(0, 8);
            string amount = GenerateRandomNumber(2) + "000000.00";
            string fee = GenerateRandomNumber(2) + "0.00";
            if (currency == "USD")
            {
                amount = rnd.Next(10, 100).ToString() + ".00";  // 10–99 USD
                fee = rnd.Next(1, 10).ToString() + ".00";       // 1–9 USD
            }
            else if (currency == "VND")
            {
                amount = rnd.Next(10, 100).ToString() + "000000.00"; // 10–99 triệu VND
                fee = rnd.Next(1, 10).ToString() + "000.00";         // 1–9 ngàn VND
            }
            string province = "";
            var listProvince = _useBlackListOnly ? _provincesBlackList : _provinces;
            if (listProvince.Count > 0)
                province = listProvince[rnd.Next(listProvince.Count)];

            // chọn phường theo tỉnh
            string ward = "";
            if (!string.IsNullOrEmpty(province) && _wardsByProvinceName.TryGetValue(province, out var wardsList))
            {
                if (wardsList.Count > 0)
                    ward = wardsList[rnd.Next(wardsList.Count)];
            }
            else
            {
                // fallback nếu không tìm thấy
                var allWards = _wardsByProvinceName.Values.SelectMany(x => x).ToList();
                if (allWards.Count > 0)
                    ward = allWards[rnd.Next(allWards.Count)];
            }
            // chọn phường theo tỉnh hiện tại nếu có
            string wardForAddress = "";
            if (!string.IsNullOrEmpty(province) && _wardsByProvinceName.TryGetValue(province, out var wardsListForAddr) && wardsListForAddr.Count > 0)
            {
                wardForAddress = wardsListForAddr[rnd.Next(wardsListForAddr.Count)];
            }
            else
            {
                // fallback nếu không có phường cho tỉnh này
                var allWards = _wardsByProvinceName.Values.SelectMany(x => x).ToList();
                if (allWards.Count > 0)
                    wardForAddress = allWards[rnd.Next(allWards.Count)];
                else
                    wardForAddress = "Phường ngẫu nhiên";
            }
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
            if (FieldSelected("paymentInfo.feeAmount")) paymentInfo["feeAmount"] = fee;
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
            //if (FieldSelected("senderInfo.city")) senderInfo["city"] = _provinces[rnd.Next(_provinces.Count)];
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
            if (FieldSelected("receiverInfo.address"))
                receiverInfo["address"] = "ĐC " + wardForAddress;
            if (FieldSelected("receiverInfo.fullName2")) receiverInfo["fullName2"] = GenerateRandomName();
            if (FieldSelected("receiverInfo.phoneNumber2")) receiverInfo["phoneNumber2"] = "09" + GenerateRandomNumber(8);
            if (FieldSelected("receiverInfo.address2"))
                receiverInfo["address2"] = "ĐC " + wardForAddress;
            if (FieldSelected("receiverInfo.idNumber")) receiverInfo["idNumber"] = idNumber;
            if (FieldSelected("receiverInfo.issueDate")) receiverInfo["issueDate"] = RandomDate(2020, 2023);
            if (FieldSelected("receiverInfo.issuer")) receiverInfo["issuer"] = "Gov";
            if (FieldSelected("receiverInfo.nationality")) receiverInfo["nationality"] = _countries[rnd.Next(_countries.Count)];
            if (FieldSelected("receiverInfo.gender")) receiverInfo["gender"] = rnd.Next(2) == 0 ? "M" : "F";
            if (FieldSelected("receiverInfo.doB")) receiverInfo["doB"] = RandomDate(1985, 2005);
            if (FieldSelected("receiverInfo.ethnicity")) receiverInfo["ethnicity"] = "Kinh";
            if (FieldSelected("receiverInfo.occupation")) receiverInfo["occupation"] = _occupations[rnd.Next(_occupations.Count)];
            if (FieldSelected("receiverInfo.province")) receiverInfo["province"] = province;
            if (FieldSelected("receiverInfo.ward")) receiverInfo["ward"] = ward;
            if (FieldSelected("receiverInfo.transferPurpose")) receiverInfo["transferPurpose"] = _purposes[rnd.Next(_purposes.Count)];
            if (FieldSelected("receiverInfo.senderRelationship")) receiverInfo["senderRelationship"] = _relationships[rnd.Next(_relationships.Count)];
            if (FieldSelected("receiverInfo.accountNumber")) receiverInfo["accountNumber"] = accNumber;
            if (FieldSelected("receiverInfo.bankCode")) receiverInfo["bankCode"] = bankCode;
            if (FieldSelected("receiverInfo.bankBranchCode")) receiverInfo["bankBranchCode"] = bankBranch;
            root["receiverInfo"] = receiverInfo;

            //return JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true });
            return JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

        }

        private async Task GuiApi(string json, int stt)
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var start = DateTime.Now;

                HttpResponseMessage response = await client.PostAsync("https://58.186.16.67/api/partner/transfer", content);
                string result = await response.Content.ReadAsStringAsync();

                var elapsed = DateTime.Now - start;

                // Thử parse JSON response để lấy mã phản hồi (responseCode)
                string responseCode = "";
                try
                {
                    using var doc = JsonDocument.Parse(result);
                    if (doc.RootElement.TryGetProperty("response", out var responseObj) &&
                        responseObj.TryGetProperty("responseCode", out var codeProp))
                    {
                        responseCode = codeProp.GetString();
                    }
                }
                catch
                {
                    responseCode = "(parse error)";
                }

                // Format log rõ ràng và dễ đọc
                string logText =
                                $@"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] #{stt}
⏱️ Duration: {elapsed.TotalMilliseconds:F0} ms
➡️ ResponseCode: {responseCode}
REQUEST:
{json}

RESPONSE: {response.StatusCode}
{result}
----------------------------------------------------

";

                AppendResult(logText);
                WriteLogToFile(logText);
            }
            catch (Exception ex)
            {
                string logText =
        $@"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] #{stt} ❌ ERROR: {ex.Message}
----------------------------------------------------

";
                AppendResult(logText);
                WriteLogToFile(logText);
            }
        }


        private string GenerateRandomName()
        {
            string[] ho = {
        "Nguyen", "Tran", "Le", "Pham", "Hoang", "Huynh", "Phan", "Vu", "Vo", "Dang",
        "Bui", "Do", "Ngo", "Duong", "Ly", "Cao", "Chu", "La", "Luong", "Mai",
        "Trinh", "Tieu", "Hua", "Ton", "Quach", "Truong", "Diep", "Han", "Ngoc", "Dang"
    };

            string[] lot = {
        "Van", "Thi", "Huu", "Minh", "Quoc", "Gia", "Thanh", "Duc", "Khanh", "Kim",
        "Hong", "Anh", "Ngoc", "Bao", "Thuy", "Xuan", "Tan", "Phuoc", "Chau", "Lan"
    };

            string[] ten = {
        "An", "Binh", "Cuong", "Dung", "Dong", "Hanh", "Lan", "Mai", "Nam", "Phuc",
        "Son", "Tuan", "Vuong", "Yen", "Thao", "Trang", "Ly", "Hoa", "Hieu", "Khanh",
        "Linh", "Nguyet", "Tam", "Vy", "Duy", "Hoang", "Manh", "Quang", "Thien", "Viet"
    };

            string h = ho[rnd.Next(ho.Length)];
            string l = lot[rnd.Next(lot.Length)];
            string t = ten[rnd.Next(ten.Length)];

            return $"{h} {l} {t}";
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
        private void SaveCheckedItemsToFile(string filePath)
        {
            var selected = new List<string>();
            foreach (var item in chkFields.CheckedItems)
            {
                selected.Add(item.ToString());
            }

            var config = new CheckedFieldsConfig { SelectedFields = selected };

            string json = System.Text.Json.JsonSerializer.Serialize(config, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(filePath, json);
        }
        private void RestoreCheckedItemsFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            string json = File.ReadAllText(filePath);
            var config = System.Text.Json.JsonSerializer.Deserialize<CheckedFieldsConfig>(json);

            if (config == null) return;

            for (int i = 0; i < chkFields.Items.Count; i++)
            {
                string item = chkFields.Items[i].ToString();
                chkFields.SetItemChecked(i, config.SelectedFields.Contains(item));
            }
        }

    }
}
