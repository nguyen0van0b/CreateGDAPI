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
    public partial class Form3 : Form
    {
        private readonly HttpClientHandler? handler;
        private readonly HttpClient? client;
        private readonly Random rnd = new Random();

        private List<(string Code, string Branch)> _banks = new();
        private List<string> _provinces = new();
        private List<string> _countries = new();
        private List<string> _provincesBlackList = new();
        private Dictionary<string, List<string>> _wardsByProvinceName = new();

        // Danh sách transaction đã tạo để test các API khác
        private List<TransactionInfo> _createdTransactions = new();

        // Thêm biến để lưu config
        private FieldsConfig _fieldsConfig;

        // Auto push variables
        private System.Windows.Forms.Timer _autoTimer;
        private bool _isAutoPushing = false;
        private int _autoPushCount = 0;
        private int _autoPushTarget = 0;

        public Form3()
        {
            InitializeComponent();
            if (DesignMode) return;
            handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            client = new HttpClient(handler);

            // Initialize auto push timer
            _autoTimer = new System.Windows.Forms.Timer();
            _autoTimer.Tick += AutoTimer_Tick;
        }

        private async void AutoTimer_Tick(object sender, EventArgs e)
        {
            if (!_isAutoPushing) return;

            if (_autoPushCount >= _autoPushTarget)
            {
                StopAutoPush();
                MessageBox.Show($"✅ Đã hoàn thành {_autoPushCount} requests tự động!",
                    "Auto Push Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string endpoint = comboApiEndpoint.SelectedItem?.ToString() ?? "healthcheck";
            string partnerCode = txtPartnerCode.Text.Trim();
            string agencyCode = txtAgencyCode.Text.Trim();

            _autoPushCount++;
            await SendApiRequest(endpoint, partnerCode, agencyCode, _autoPushCount);
        }

        private void StartAutoPush()
        {
            _isAutoPushing = true;
            _autoPushCount = 0;
            btnStartAutoPush.Text = "⏸️ Stop Auto Push";
            btnStartAutoPush.BackColor = Color.FromArgb(255, 128, 128);
            btnSendRequest.Enabled = false;
            comboApiEndpoint.Enabled = false;

            AppendResult($"[AUTO PUSH] ▶️ Bắt đầu auto push {_autoPushTarget} requests mỗi {numAutoPushInterval.Value} giây\r\n");
        }

        private void StopAutoPush()
        {
            _isAutoPushing = false;
            _autoTimer.Stop();
            btnStartAutoPush.Text = "▶️ Start Auto Push";
            btnStartAutoPush.BackColor = Color.FromArgb(128, 255, 192);
            btnSendRequest.Enabled = true;
            comboApiEndpoint.Enabled = true;

            AppendResult($"[AUTO PUSH] ⏹️ Đã dừng auto push sau {_autoPushCount} requests\r\n");
        }

        private void btnStartAutoPush_Click(object sender, EventArgs e)
        {
            if (_isAutoPushing)
            {
                StopAutoPush();
                return;
            }

            if (string.IsNullOrEmpty(txtPartnerCode.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập Partner Code");
                return;
            }

            if (!int.TryParse(txtAutoPushCount.Text.Trim(), out int count) || count <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng auto push hợp lệ (> 0)");
                return;
            }

            _autoPushTarget = count;
            _autoTimer.Interval = (int)numAutoPushInterval.Value * 1000; // Convert to milliseconds
            _autoTimer.Start();
            StartAutoPush();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // Khởi tạo combo boxes
            comboApiEndpoint.Items.AddRange(new string[]
            {
                "healthcheck",
                "transfer",
                "acctinq",
                "canceltrans",
                "queryinfor",
                "transinq",
                "updatetrans"
            });
            comboApiEndpoint.SelectedIndex = 0;

            comboCurrency.Items.AddRange(new string[] { "VND", "USD" });
            comboCurrency.SelectedIndex = 0;

            comboServiceType.Items.AddRange(new string[] { "AD", "WD", "CP", "HD" });
            comboServiceType.SelectedIndex = 0;

            // Load dữ liệu Excel
            LoadMasterData();

            // Load fields config
            _fieldsConfig = FormSettings.LoadFieldsConfig();
        }

        private void LoadMasterData()
        {
            try
            {
                string basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "re");
                _banks = LoadBankCodes(Path.Combine(basePath, "MasterBanksList.xlsx"));
                LoadProvincesWithBlackList(Path.Combine(basePath, "MasterProvincesList.xlsx"));
                LoadWardsByProvinceName(Path.Combine(basePath, "MasterWardsList.xlsx"));
                _countries = LoadListFromExcel(Path.Combine(basePath, "MasterCountriesList.xlsx"), 3);

                AppendResult("[INFO] ✅ Load dữ liệu thành công.\r\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi load dữ liệu: {ex.Message}");
            }
        }

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
                    string provinceName = row.Cell(2).GetString().Trim();
                    string isBlack = row.Cell(5).GetString().Trim().ToUpper();

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
                    string wardName = row.Cell(3).GetString().Trim();
                    string provinceName = row.Cell(6).GetString().Trim();

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

        private async void btnSendRequest_Click(object sender, EventArgs e)
        {
            string endpoint = comboApiEndpoint.SelectedItem?.ToString() ?? "healthcheck";
            string partnerCode = txtPartnerCode.Text.Trim();
            string agencyCode = txtAgencyCode.Text.Trim();

            if (string.IsNullOrEmpty(partnerCode))
            {
                MessageBox.Show("Vui lòng nhập Partner Code");
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
                await SendApiRequest(endpoint, partnerCode, agencyCode, i);
            }
        }

        private async Task SendApiRequest(string endpoint, string partnerCode, string agencyCode, int stt)
        {
            string json = "";
            string url = $"https://58.186.16.67/api/partner/{endpoint}";

            try
            {
                switch (endpoint)
                {
                    case "healthcheck":
                        var start = DateTime.Now;
                        HttpResponseMessage healthResponse = await client.GetAsync(url);
                        string healthResult = await healthResponse.Content.ReadAsStringAsync();
                        var elapsed = DateTime.Now - start;

                        string logText = $@"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] #{stt} - HEALTHCHECK
⏱️ Duration: {elapsed.TotalMilliseconds:F0} ms
RESPONSE: {healthResponse.StatusCode}
{healthResult}
----------------------------------------------------

";
                        AppendResult(logText);
                        WriteLogToFile(logText);
                        return;

                    case "transfer":
                        json = CreateTransferRequest(partnerCode, agencyCode);
                        break;

                    case "acctinq":
                        json = CreateAcctInqRequest(partnerCode);
                        break;

                    case "canceltrans":
                        json = CreateCancelTransRequest(partnerCode, agencyCode);
                        break;

                    case "queryinfor":
                        json = CreateQueryInforRequest(partnerCode);
                        break;

                    case "transinq":
                        json = CreateTransInqRequest(partnerCode);
                        break;

                    case "updatetrans":
                        json = CreateUpdateTransRequest(partnerCode, agencyCode);
                        break;

                    default:
                        AppendResult($"[ERROR] Endpoint không được hỗ trợ: {endpoint}\r\n");
                        return;
                }

                await SendPostRequest(url, json, stt, endpoint);
            }
            catch (Exception ex)
            {
                string errorLog = $@"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] #{stt} ❌ ERROR: {ex.Message}
----------------------------------------------------

";
                AppendResult(errorLog);
                WriteLogToFile(errorLog);
            }
        }

        private async Task SendPostRequest(string url, string json, int stt, string endpoint)
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var start = DateTime.Now;

                HttpResponseMessage response = await client.PostAsync(url, content);
                string result = await response.Content.ReadAsStringAsync();

                var elapsed = DateTime.Now - start;

                string responseCode = "";
                try
                {
                    using var doc = JsonDocument.Parse(result);
                    if (doc.RootElement.TryGetProperty("response", out var responseObj) &&
                        responseObj.TryGetProperty("responseCode", out var codeProp))
                    {
                        responseCode = codeProp.GetString();
                    }

                    if (endpoint == "transfer" && responseCode == "00")
                    {
                        SaveTransactionInfo(doc.RootElement);
                    }
                }
                catch
                {
                    responseCode = "(parse error)";
                }

                // Format JSON cho dễ đọc
                string formattedRequest = FormatJsonForLog(json);
                string formattedResponse = FormatJsonForLog(result);

                string statusIcon = responseCode == "00" ? "✅" : "❌";
                string logText = $@"
╔══════════════════════════════════════════════════════════════════════════════╗
║ [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] #{stt} - {endpoint.ToUpper()}
║ {statusIcon} Status: {(responseCode == "00" ? "SUCCESS" : "FAILED")}
║ ⏱️  Duration: {elapsed.TotalMilliseconds:F0} ms
║ 💬 ResponseCode: {responseCode}
╚══════════════════════════════════════════════════════════════════════════════╝

📤 REQUEST:
{formattedRequest}

📥 RESPONSE: {response.StatusCode}
{formattedResponse}

════════════════════════════════════════════════════════════════════════════════

";

                AppendResult(logText);
                WriteLogToFile(logText);
            }
            catch (Exception ex)
            {
                string logText = $@"
╔══════════════════════════════════════════════════════════════════════════════╗
║ [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] #{stt} - ERROR
║ ❌ {ex.Message}
╚══════════════════════════════════════════════════════════════════════════════╝

════════════════════════════════════════════════════════════════════════════════

";
                AppendResult(logText);
                WriteLogToFile(logText);
            }
        }

        private string FormatJsonForLog(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "No data";

            try
            {
                using var doc = JsonDocument.Parse(json);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                return JsonSerializer.Serialize(doc, options);
            }
            catch
            {
                return json;
            }
        }

        private void SaveTransactionInfo(JsonElement root)
        {
            try
            {
                var info = new TransactionInfo
                {
                    RefNo = root.GetProperty("refNo").GetString(),
                    PartnerRef = root.GetProperty("partnerRef").GetString(),
                    PartnerCode = root.GetProperty("partnerCode").GetString(),
                    TransactionRef = root.GetProperty("transactionRef").GetString()
                };

                _createdTransactions.Add(info);

                if (_createdTransactions.Count > 100)
                {
                    _createdTransactions.RemoveAt(0);
                }
            }
            catch { }
        }

        private string CreateTransferRequest(string partnerCode, string agencyCode)
        {
            string refNo = Guid.NewGuid().ToString();
            string partnerRef = "PartnerRef-" + agencyCode + GenerateRandomNumber(6);
            string pin = "PIN-" + agencyCode + GenerateRandomNumber(6);
            string serviceType = comboServiceType.SelectedItem?.ToString() ?? "AD";
            string currency = comboCurrency.SelectedItem?.ToString() ?? "VND";

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

            string amount = rnd.Next(10, 100).ToString() + "000000.00";
            string fee = rnd.Next(1, 10).ToString() + "000.00";

            if (currency == "USD")
            {
                amount = rnd.Next(10, 100).ToString() + ".00";
                fee = rnd.Next(1, 10).ToString() + ".00";
            }

            var listProvince = _fieldsConfig.UseBlackListOnly ? _provincesBlackList : _provinces;
            string province = listProvince.Count > 0 ? listProvince[rnd.Next(listProvince.Count)] : "";

            string ward = "";
            if (_wardsByProvinceName.TryGetValue(province, out var wardsList) && wardsList.Count > 0)
            {
                ward = wardsList[rnd.Next(wardsList.Count)];
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
            if (FieldSelected("senderInfo.address")) senderInfo["address"] = "Address " + GenerateRandomNumber(3);
            if (FieldSelected("senderInfo.country")) senderInfo["country"] = _countries[rnd.Next(_countries.Count)];
            if (FieldSelected("senderInfo.transferPurpose")) senderInfo["transferPurpose"] = "Gift";
            if (FieldSelected("senderInfo.fundSource")) senderInfo["fundSource"] = "Salary";
            if (FieldSelected("senderInfo.recipientRelationship")) senderInfo["recipientRelationship"] = "Friend";
            if (FieldSelected("senderInfo.content")) senderInfo["content"] = "Transfer money";
            root["senderInfo"] = senderInfo;

            var receiverInfo = new Dictionary<string, object>
            {
                ["fullName"] = receiverName,
                ["phoneNumber"] = phone,
                ["documentType"] = "CCCD"
            };
            if (FieldSelected("receiverInfo.address")) receiverInfo["address"] = "Address " + GenerateRandomNumber(3);
            if (FieldSelected("receiverInfo.fullName2")) receiverInfo["fullName2"] = GenerateRandomName();
            if (FieldSelected("receiverInfo.phoneNumber2")) receiverInfo["phoneNumber2"] = "09" + GenerateRandomNumber(8);
            if (FieldSelected("receiverInfo.address2")) receiverInfo["address2"] = "Address2 " + GenerateRandomNumber(3);
            if (FieldSelected("receiverInfo.idNumber")) receiverInfo["idNumber"] = idNumber;
            if (FieldSelected("receiverInfo.issueDate")) receiverInfo["issueDate"] = RandomDate(2020, 2023);
            if (FieldSelected("receiverInfo.issuer")) receiverInfo["issuer"] = "Gov";
            if (FieldSelected("receiverInfo.nationality")) receiverInfo["nationality"] = "VN";
            if (FieldSelected("receiverInfo.gender")) receiverInfo["gender"] = rnd.Next(2) == 0 ? "M" : "F";
            if (FieldSelected("receiverInfo.doB")) receiverInfo["doB"] = RandomDate(1985, 2005);
            if (FieldSelected("receiverInfo.ethnicity")) receiverInfo["ethnicity"] = "Kinh";
            if (FieldSelected("receiverInfo.occupation")) receiverInfo["occupation"] = "Engineer";
            if (FieldSelected("receiverInfo.province")) receiverInfo["province"] = province;
            if (FieldSelected("receiverInfo.ward")) receiverInfo["ward"] = ward;
            if (FieldSelected("receiverInfo.transferPurpose")) receiverInfo["transferPurpose"] = "Gift";
            if (FieldSelected("receiverInfo.senderRelationship")) receiverInfo["senderRelationship"] = "Friend";
            if (FieldSelected("receiverInfo.accountNumber")) receiverInfo["accountNumber"] = accNumber;
            if (FieldSelected("receiverInfo.bankCode")) receiverInfo["bankCode"] = bankCode;
            if (FieldSelected("receiverInfo.bankBranchCode")) receiverInfo["bankBranchCode"] = bankBranch;
            root["receiverInfo"] = receiverInfo;

            return JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        private bool FieldSelected(string field)
        {
            return _fieldsConfig?.SelectedFields?.Contains(field) ?? false;
        }

        private string CreateAcctInqRequest(string partnerCode)
        {
            var bank = _banks[rnd.Next(_banks.Count)];
            string accNumber = GenerateRandomNumber(16);
            string currency = comboCurrency.SelectedItem?.ToString() ?? "VND";

            var root = new Dictionary<string, object>
            {
                ["refNo"] = Guid.NewGuid().ToString(),
                ["partnerCode"] = partnerCode,
                ["receiver"] = new Dictionary<string, object>
                {
                    ["bankCode"] = bank.Code,
                    ["account"] = accNumber,
                    ["fullName"] = GenerateRandomName(),
                    ["currency"] = currency
                }
            };

            return JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        private string CreateCancelTransRequest(string partnerCode, string agencyCode)
        {
            string partnerRef = "";
            string pin = "";

            if (_createdTransactions.Count > 0)
            {
                var trans = _createdTransactions[rnd.Next(_createdTransactions.Count)];
                partnerRef = trans.PartnerRef;
                pin = "PIN-" + agencyCode + GenerateRandomNumber(6);
            }
            else
            {
                partnerRef = "PartnerRef-" + agencyCode + GenerateRandomNumber(6);
                pin = "PIN-" + agencyCode + GenerateRandomNumber(6);
            }

            var root = new Dictionary<string, object>
            {
                ["refNo"] = Guid.NewGuid().ToString(),
                ["partnerCode"] = partnerCode,
                ["agentCode"] = agencyCode,
                ["partnerRef"] = partnerRef,
                ["pin"] = pin,
                ["paymentType"] = comboServiceType.SelectedItem?.ToString() ?? "AD",
                ["cancelReason"] = "Customer request"
            };

            return JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        private string CreateQueryInforRequest(string partnerCode)
        {
            string currency = comboCurrency.SelectedItem?.ToString() ?? "VND";

            var root = new Dictionary<string, object>
            {
                ["refNo"] = Guid.NewGuid().ToString(),
                ["partnerCode"] = partnerCode,
                ["currency"] = currency
            };

            return JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        private string CreateTransInqRequest(string partnerCode)
        {
            string partnerRef = "";
            string pin = "";

            if (_createdTransactions.Count > 0)
            {
                var trans = _createdTransactions[rnd.Next(_createdTransactions.Count)];
                partnerRef = trans.PartnerRef;
                pin = "PIN-" + GenerateRandomNumber(6);
            }
            else
            {
                partnerRef = "PartnerRef-" + GenerateRandomNumber(6);
                pin = "PIN-" + GenerateRandomNumber(6);
            }

            var root = new Dictionary<string, object>
            {
                ["refNo"] = Guid.NewGuid().ToString(),
                ["partnerCode"] = partnerCode,
                ["partnerRef"] = partnerRef,
                ["pin"] = pin
            };

            return JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        private string CreateUpdateTransRequest(string partnerCode, string agencyCode)
        {
            string partnerRef = "";
            string pin = "";

            if (_createdTransactions.Count > 0)
            {
                var trans = _createdTransactions[rnd.Next(_createdTransactions.Count)];
                partnerRef = trans.PartnerRef;
                pin = "PIN-" + agencyCode + GenerateRandomNumber(6);
            }
            else
            {
                partnerRef = "PartnerRef-" + agencyCode + GenerateRandomNumber(6);
                pin = "PIN-" + agencyCode + GenerateRandomNumber(6);
            }

            string receiverName = GenerateRandomName();
            string phone = "0" + GenerateRandomNumber(9);
            string idNumber = GenerateRandomNumber(12);
            string accNumber = GenerateRandomNumber(16);

            var bank = _banks[rnd.Next(_banks.Count)];
            string bankCode = bank.Code;
            string bankBranch = bank.Branch + rnd.Next(1, 100).ToString("D2");
            bankBranch = bankBranch.Replace(" ", "").Trim();
            if (bankBranch.Length > 8) bankBranch = bankBranch.Substring(0, 8);

            string province = _provinces[rnd.Next(_provinces.Count)];
            string ward = "";
            if (_wardsByProvinceName.TryGetValue(province, out var wardsList) && wardsList.Count > 0)
            {
                ward = wardsList[rnd.Next(wardsList.Count)];
            }

            var root = new Dictionary<string, object>
            {
                ["refNo"] = Guid.NewGuid().ToString(),
                ["partnerCode"] = partnerCode,
                ["agencyCode"] = agencyCode,
                ["partnerRef"] = partnerRef,
                ["pin"] = pin,
                ["serviceType"] = comboServiceType.SelectedItem?.ToString() ?? "AD",
                ["updateReason"] = "Update receiver information",
                ["receiverInfo"] = new Dictionary<string, object>
                {
                    ["fullName"] = receiverName,
                    ["phoneNumber"] = phone,
                    ["address"] = "Address " + GenerateRandomNumber(3),
                    ["documentType"] = "CCCD",
                    ["idNumber"] = idNumber,
                    ["issueDate"] = RandomDate(2020, 2023),
                    ["issuer"] = "Gov",
                    ["nationality"] = "VN",
                    ["gender"] = rnd.Next(2) == 0 ? "M" : "F",
                    ["doB"] = RandomDate(1985, 2005),
                    ["ethnicity"] = "Kinh",
                    ["occupation"] = "Engineer",
                    ["province"] = province,
                    ["ward"] = ward,
                    ["transferPurpose"] = "Gift",
                    ["senderRelationship"] = "Friend",
                    ["accountNumber"] = accNumber,
                    ["bankCode"] = bankCode,
                    ["bankBranchCode"] = bankBranch
                }
            };

            return JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        private string GenerateRandomName()
        {
            string[] ho = {
                "Nguyen", "Tran", "Le", "Pham", "Hoang", "Huynh", "Phan", "Vu", "Vo", "Dang",
                "Bui", "Do", "Ngo", "Duong", "Ly"
            };

            string[] lot = {
                "Van", "Thi", "Huu", "Minh", "Quoc", "Gia", "Thanh", "Duc", "Khanh", "Kim"
            };

            string[] ten = {
                "An", "Binh", "Cuong", "Dung", "Dong", "Hanh", "Lan", "Mai", "Nam", "Phuc"
            };

            return $"{ho[rnd.Next(ho.Length)]} {lot[rnd.Next(lot.Length)]} {ten[rnd.Next(ten.Length)]}";
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

                string logPath = Path.Combine(logDir, $"logs_all_apis_{DateTime.Now:yyyyMMdd}.txt");
                File.AppendAllText(logPath, text + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }

        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            txtResult.Clear();
        }

        private void btnViewTransactions_Click(object sender, EventArgs e)
        {
            if (_createdTransactions.Count == 0)
            {
                MessageBox.Show("Chưa có transaction nào được tạo!");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine($"=== DANH SÁCH {_createdTransactions.Count} TRANSACTIONS ===\n");

            for (int i = 0; i < _createdTransactions.Count; i++)
            {
                var t = _createdTransactions[i];
                sb.AppendLine($"#{i + 1}:");
                sb.AppendLine($"  RefNo: {t.RefNo}");
                sb.AppendLine($"  PartnerRef: {t.PartnerRef}");
                sb.AppendLine($"  TransactionRef: {t.TransactionRef}");
                sb.AppendLine();
            }

            txtResult.Text = sb.ToString();
        }

        private void btnReloadData_Click(object sender, EventArgs e)
        {
            LoadMasterData();
            _fieldsConfig = FormSettings.LoadFieldsConfig();
            AppendResult("[INFO] ✅ Đã reload cấu hình và dữ liệu.\r\n");
        }

        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            var settingsForm = new FormSettings();
            settingsForm.ShowDialog();

            _fieldsConfig = FormSettings.LoadFieldsConfig();
            AppendResult("[INFO] ✅ Đã reload cấu hình từ Settings.\r\n");
        }

        private void btnOpenReport_Click(object sender, EventArgs e)
        {
            var reportForm = new FormReport();
            reportForm.Show();
        }
    }

    public class TransactionInfo
    {
        public string RefNo { get; set; }
        public string PartnerRef { get; set; }
        public string PartnerCode { get; set; }
        public string TransactionRef { get; set; }
    }
}