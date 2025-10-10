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
        private bool _isAutoTesting = false;
        private int _autoTestStep = 0;
        private List<(string Code, string Branch)> _banks = new();
        private List<string> _provinces = new();
        private List<string> _countries = new();
        private List<string> _provincesBlackList = new();
        private Dictionary<string, List<string>> _wardsByProvinceName = new();
        private System.Threading.Timer _batchTimer;
        private readonly object _lockObj = new object();
        private int _pendingRequests = 0;
        private bool _isAutoPushingBatch = false;
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
            _isAutoPushingBatch = false;

            // Dispose batch timer
            if (_batchTimer != null)
            {
                _batchTimer.Dispose();
                _batchTimer = null;
            }

            // Dispose old timer nếu có
            if (_autoTimer != null)
            {
                _autoTimer.Stop();
            }

            btnStartAutoPush.Text = "▶️ Start Auto Push";
            btnStartAutoPush.BackColor = Color.FromArgb(128, 255, 192);
            btnSendRequest.Enabled = true;
            comboApiEndpoint.Enabled = true;

            AppendResult($"[AUTO PUSH] ⏹️ Đã dừng auto push sau {_autoPushCount} requests " +
                         $"(Pending: {_pendingRequests})\r\n");

            // Hiển thị thống kê
            DisplayTransactionStatistics();
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
            _autoPushCount = 0;
            _isAutoPushing = true;
            _isAutoPushingBatch = true;
            _pendingRequests = 0;

            btnStartAutoPush.Text = "⏸️ Stop Auto Push";
            btnStartAutoPush.BackColor = Color.FromArgb(255, 128, 128);
            btnSendRequest.Enabled = false;
            comboApiEndpoint.Enabled = false;

            // ✅ TÍNH TOÁN INTERVAL DựA trên numAutoPushInterval
            // VD: numAutoPushInterval = 0.1 giây → 10 requests/second → 100ms interval
            double intervalSeconds = (double)numAutoPushInterval.Value;
            int intervalMs = (int)(intervalSeconds * 1000);

            AppendResult($"[AUTO PUSH] ▶️ Bắt đầu auto push {_autoPushTarget} requests " +
                         $"(Interval: {intervalMs}ms = {1000.0 / intervalMs:F1} req/s)\r\n");

            // ✅ SỬ DỤNG System.Threading.Timer ĐỂ PUSH NHANH
            _batchTimer = new System.Threading.Timer(
                async _ => await ProcessBatchRequest(),
                null,
                0,  // Bắt đầu ngay
                intervalMs  // Lặp lại mỗi intervalMs
            );
        }
        private async Task ProcessBatchRequest()
        {
            if (!_isAutoPushingBatch || _autoPushCount >= _autoPushTarget)
            {
                if (_autoPushCount >= _autoPushTarget)
                {
                    // Stop và thông báo
                    this.Invoke((MethodInvoker)delegate
                    {
                        StopAutoPush();
                        MessageBox.Show($"✅ Đã hoàn thành {_autoPushCount} requests tự động!",
                            "Auto Push Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    });
                }
                return;
            }

            // Giới hạn pending requests
            lock (_lockObj)
            {
                if (_pendingRequests > 50)  // Max 50 pending
                {
                    return;
                }
                _pendingRequests++;
            }

            try
            {
                string endpoint = null;
                string partnerCode = null;
                string agencyCode = null;

                this.Invoke((MethodInvoker)delegate
                {
                    endpoint = comboApiEndpoint.SelectedItem?.ToString() ?? "healthcheck";
                    partnerCode = txtPartnerCode.Text.Trim();
                    agencyCode = txtAgencyCode.Text.Trim();
                });

                // Fire and forget - không đợi
                _ = Task.Run(async () =>
                {
                    try
                    {
                        int currentCount;
                        lock (_lockObj)
                        {
                            currentCount = ++_autoPushCount;
                        }

                        await SendApiRequest(endpoint, partnerCode, agencyCode, currentCount);

                        lock (_lockObj)
                        {
                            _pendingRequests--;
                        }

                        // Update UI
                        this.Invoke((MethodInvoker)delegate
                        {
                            // Có thể thêm progress bar nếu muốn
                        });
                    }
                    catch (Exception ex)
                    {
                        lock (_lockObj)
                        {
                            _pendingRequests--;
                        }
                        Console.WriteLine($"❌ Batch request error: {ex.Message}");
                    }
                });
            }
            catch (Exception ex)
            {
                lock (_lockObj)
                {
                    _pendingRequests--;
                }
                Console.WriteLine($"❌ Error in batch processing: {ex.Message}");
            }
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

            // ✅ THÊM: Load paid transactions từ log
            LoadPaidTransactionsFromLog();
        }
        private void LoadPaidTransactionsFromLog()
        {
            try
            {
                string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "re");
                if (!Directory.Exists(logDirectory))
                {
                    return;
                }

                string todayLog = Path.Combine(logDirectory,
                    $"logs_all_apis_{DateTime.Now:yyyyMMdd}.txt");

                if (!File.Exists(todayLog))
                {
                    AppendResult("[INFO] Không tìm thấy log hôm nay.\r\n");
                    return;
                }

                string content = File.ReadAllText(todayLog, Encoding.UTF8);
                var entries = content.Split(new[] { "----------------------------------------------------" },
                    StringSplitOptions.RemoveEmptyEntries);

                int loadedPaid = 0;
                int loadedCancelled = 0;
                int loadedError99 = 0;  // ✅ ĐẾM SỐ LƯỢNG LỖI 99

                foreach (var entry in entries)
                {
                    // ✅ LOAD TRANSFER TRANSACTIONS (bao gồm cả PAID, PENDING và ERROR 99)
                    if (entry.Contains("TRANSFER") && !entry.Contains("UPDATETRANS"))
                    {
                        string partnerRef = null;
                        string transactionRef = null;
                        string partnerCode = null;
                        string responseCode = null;
                        bool isPaid = false;

                        var lines = entry.Split('\n');
                        foreach (var line in lines)
                        {
                            // Extract ResponseCode
                            if (line.Contains("ResponseCode:"))
                            {
                                var parts = line.Split(':');
                                if (parts.Length > 1)
                                {
                                    responseCode = parts[1].Trim();
                                }
                            }

                            // Extract PartnerRef for PAID
                            if (line.Contains("🔒 Transaction PAID - PartnerRef:"))
                            {
                                var parts = line.Split(new[] { "PartnerRef:" }, StringSplitOptions.None);
                                if (parts.Length > 1)
                                {
                                    partnerRef = parts[1].Trim();
                                    isPaid = true;
                                }
                            }

                            // Extract TransactionRef
                            if (line.Contains("🆔 TransactionRef:"))
                            {
                                var parts = line.Split(new[] { "TransactionRef:" }, StringSplitOptions.None);
                                if (parts.Length > 1)
                                {
                                    transactionRef = parts[1].Trim();
                                }
                            }

                            // Extract PartnerCode from JSON
                            if (line.Contains("\"partnerCode\""))
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(
                                    line, @"""partnerCode"":\s*""([^""]+)""");
                                if (match.Success)
                                {
                                    partnerCode = match.Groups[1].Value;
                                }
                            }

                            // Extract PartnerRef from JSON if not already found
                            if (string.IsNullOrEmpty(partnerRef) && line.Contains("\"partnerRef\""))
                            {
                                var match = System.Text.RegularExpressions.Regex.Match(
                                    line, @"""partnerRef"":\s*""([^""]+)""");
                                if (match.Success)
                                {
                                    partnerRef = match.Groups[1].Value;
                                }
                            }
                        }

                        // ✅ TẠO TRANSACTION INFO CHO MỌI TRANSFER (không chỉ PAID)
                        if (!string.IsNullOrEmpty(partnerRef) && !string.IsNullOrEmpty(responseCode))
                        {
                            var existingTransaction = _createdTransactions
                                .FirstOrDefault(t => t.PartnerRef == partnerRef);

                            if (existingTransaction == null)
                            {
                                _createdTransactions.Add(new TransactionInfo
                                {
                                    PartnerRef = partnerRef,
                                    TransactionRef = transactionRef,
                                    PartnerCode = partnerCode,
                                    IsPaid = isPaid,
                                    IsCancelled = false,
                                    ResponseCode = responseCode,  // ✅ LƯU RESPONSE CODE TỪ LOG
                                    CreatedAt = DateTime.Now
                                });

                                if (isPaid)
                                    loadedPaid++;
                                else if (responseCode == "99")
                                    loadedError99++;  // ✅ ĐẾM LỖI 99
                            }
                        }
                    }

                    // ✅ LOAD CANCELLED TRANSACTIONS
                    if (entry.Contains("🚫 CANCELLED") && entry.Contains("CANCELTRANS"))
                    {
                        string partnerRef = null;
                        string responseCode = null;

                        var lines = entry.Split('\n');
                        foreach (var line in lines)
                        {
                            // Extract ResponseCode
                            if (line.Contains("ResponseCode:"))
                            {
                                var parts = line.Split(':');
                                if (parts.Length > 1)
                                {
                                    responseCode = parts[1].Trim();
                                }
                            }

                            if (line.Contains("❌ Transaction CANCELLED - PartnerRef:"))
                            {
                                var parts = line.Split(new[] { "PartnerRef:" }, StringSplitOptions.None);
                                if (parts.Length > 1)
                                {
                                    partnerRef = parts[1].Trim();
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(partnerRef) && responseCode == "00")
                        {
                            var existingTransaction = _createdTransactions
                                .FirstOrDefault(t => t.PartnerRef == partnerRef);

                            if (existingTransaction != null)
                            {
                                existingTransaction.IsCancelled = true;
                                loadedCancelled++;
                            }
                            else
                            {
                                // Nếu chưa có trong list, tạo mới với IsCancelled = true
                                _createdTransactions.Add(new TransactionInfo
                                {
                                    PartnerRef = partnerRef,
                                    IsCancelled = true,
                                    ResponseCode = responseCode,
                                    CreatedAt = DateTime.Now
                                });
                                loadedCancelled++;
                            }
                        }
                    }
                }

                if (loadedPaid > 0 || loadedCancelled > 0 || loadedError99 > 0)
                {
                    AppendResult($"[INFO] ✅ Loaded từ log: {loadedPaid} paid, {loadedCancelled} cancelled, {loadedError99} error-99 transactions\r\n");

                    // Hiển thị thống kê chi tiết hơn
                    var validTransactions = _createdTransactions.Where(t => t.ResponseCode != "99").Count();
                    var error99Transactions = _createdTransactions.Where(t => t.ResponseCode == "99").Count();

                    AppendResult($"[INFO] 📊 Transactions khả dụng: {validTransactions} | Lỗi 99 (không khả dụng): {error99Transactions}\r\n");

                    DisplayTransactionStatistics();
                }
            }
            catch (Exception ex)
            {
                AppendResult($"[ERROR] ❌ Error loading transactions: {ex.Message}\r\n");
            }
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

                        WriteApiLog("HEALTHCHECK", healthResponse.IsSuccessStatusCode ? "SUCCESS" : "FAILED",
                            (int)elapsed.TotalMilliseconds, healthResponse.IsSuccessStatusCode ? "00" : healthResponse.StatusCode.ToString(),
                            null, null, healthResult);

                        return;

                    case "transfer":
                        json = CreateTransferRequest(partnerCode, agencyCode);
                        break;

                    case "acctinq":
                        json = CreateAcctInqRequest(partnerCode);
                        break;

                    case "canceltrans":
                        json = CreateCancelTransRequest(partnerCode, agencyCode);
                        // ✅ KIỂM TRA NULL - KHÔNG GỬI NẾU KHÔNG CÓ GIAO DỊCH
                        if (string.IsNullOrEmpty(json))
                        {
                            AppendResult($"[WARNING] Bỏ qua CANCELTRANS request #{stt} - không có giao dịch khả dụng\r\n");
                            return;
                        }
                        break;

                    case "queryinfor":
                        json = CreateQueryInforRequest(partnerCode);
                        break;

                    case "transinq":
                        json = CreateTransInqRequest(partnerCode);
                        break;

                    case "updatetrans":
                        json = CreateUpdateTransRequest(partnerCode, agencyCode);
                        // ✅ KIỂM TRA NULL
                        if (string.IsNullOrEmpty(json))
                        {
                            AppendResult($"[WARNING] Bỏ qua UPDATETRANS request #{stt} - không có giao dịch khả dụng\r\n");
                            return;
                        }
                        break;

                    default:
                        AppendResult($"[ERROR] Endpoint không được hỗ trợ: {endpoint}\r\n");
                        return;
                }

                await SendPostRequest(url, json, stt, endpoint);
            }
            catch (Exception ex)
            {
                WriteApiLog(endpoint, "ERROR", 0, "", ex.Message, json, null);
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
                string partnerRef = "";

                try
                {
                    using var doc = JsonDocument.Parse(result);

                    if (doc.RootElement.TryGetProperty("response", out var responseObj) &&
                        responseObj.TryGetProperty("responseCode", out var codeProp))
                    {
                        responseCode = codeProp.GetString();
                    }

                    // ✅ LƯU TRANSFER TRANSACTION với ResponseCode
                    if (endpoint == "transfer" && responseCode == "00")
                    {
                        SaveTransactionInfo(doc.RootElement, responseCode);
                    }

                    // ✅ CẬP NHẬT CANCEL STATUS
                    // Thay thế đoạn cập nhật trạng thái CANCELLED trong SendPostRequest bằng đoạn sau
                    // (bổ sung cập nhật trạng thái vào file log nếu tìm thấy transaction)

                    if (endpoint == "canceltrans" && responseCode == "00")
                    {
                        string apiStatus = doc.RootElement.TryGetProperty("status", out var statusProp)
                            ? statusProp.GetString()
                            : "0";

                        if (doc.RootElement.TryGetProperty("partnerRef", out var prProp))
                        {
                            partnerRef = prProp.GetString();
                        }

                        if (endpoint == "canceltrans" && responseCode == "00")
                        {
                            // Use unique variable names to avoid CS0136
                            string apiStatus2 = doc.RootElement.TryGetProperty("status", out var statusProp2)
                                ? statusProp2.GetString() ?? "0"
                                : "0";

                            string partnerRef2 = partnerRef;
                            if (doc.RootElement.TryGetProperty("partnerRef", out var prProp2))
                            {
                                partnerRef2 = prProp2.GetString() ?? string.Empty;
                            }

                            if (apiStatus2 == "200" && !string.IsNullOrEmpty(partnerRef2))
                            {
                                MarkTransactionAsCancelled(partnerRef2);

                                // Cập nhật trạng thái CANCELLED vào file log nếu chưa từng có
                                try
                                {
                                    string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "re");
                                    string logPath = Path.Combine(logDir, $"logs_all_apis_{DateTime.Now:yyyyMMdd}.txt");
                                    if (File.Exists(logPath))
                                    {
                                        string logContent = File.ReadAllText(logPath, Encoding.UTF8);
                                        // Kiểm tra đã có dòng CANCELLED cho partnerRef chưa
                                        string cancelLine = $"❌ Transaction CANCELLED - PartnerRef: {partnerRef2}";
                                        if (!logContent.Contains(cancelLine))
                                        {
                                            var lines = logContent.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
                                            for (int i = 0; i < lines.Count; i++)
                                            {
                                                if (lines[i].Contains(partnerRef2) && !lines[i].Contains("CANCELLED"))
                                                {
                                                    lines.Insert(i + 1, $"🚫 [UPDATED] Transaction CANCELLED - PartnerRef: {partnerRef2}");
                                                    break;
                                                }
                                            }
                                            File.WriteAllText(logPath, string.Join(Environment.NewLine, lines), Encoding.UTF8);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    AppendResult($"[LOG UPDATE ERROR] ❌ {ex.Message}\r\n");
                                }
                            }
                        }
                    }
                }
                catch
                {
                    responseCode = "(parse error)";
                }

                string formattedRequest = FormatJsonForLog(json);
                string formattedResponse = FormatJsonForLog(result);

                string status = responseCode == "00" ? "SUCCESS" : "FAILED";
                int durationMs = (int)elapsed.TotalMilliseconds;
                WriteApiLog(endpoint, status, durationMs, responseCode, null, formattedRequest, formattedResponse);
            }
            catch (Exception ex)
            {
                WriteApiLog(endpoint, "ERROR", 0, "", ex.Message, json, null);
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

        private void SaveTransactionInfo(JsonElement root, string responseCode = "00")
        {
            try
            {
                string refNo = root.GetProperty("refNo").GetString();
                string partnerRef = root.GetProperty("partnerRef").GetString();
                string partnerCode = root.GetProperty("partnerCode").GetString();
                string transactionRef = root.TryGetProperty("transactionRef", out var trElement)
                    ? trElement.GetString()
                    : null;
                string status = root.TryGetProperty("status", out var statusElement)
                    ? statusElement.GetString()
                    : "0";

                bool isPaid = status == "200" && !string.IsNullOrEmpty(transactionRef);

                var info = new TransactionInfo
                {
                    RefNo = refNo,
                    PartnerRef = partnerRef,
                    PartnerCode = partnerCode,
                    TransactionRef = transactionRef,
                    IsPaid = isPaid,
                    IsCancelled = false,
                    ResponseCode = responseCode  // ✅ LƯU RESPONSE CODE
                };

                _createdTransactions.Add(info);

                if (_createdTransactions.Count > 100)
                {
                    _createdTransactions.RemoveAt(0);
                }

                if (isPaid)
                {
                    Console.WriteLine($"✅ Transaction PAID: {partnerRef} (TransactionRef: {transactionRef})");
                }
                else
                {
                    Console.WriteLine($"⏳ Transaction PENDING: {partnerRef} (TransactionRef: {transactionRef ?? "null"})");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error saving transaction: {ex.Message}");
            }
        }



        private void MarkTransactionAsCancelled(string partnerRef)
        {
            try
            {
                var transaction = _createdTransactions.FirstOrDefault(t => t.PartnerRef == partnerRef);
                if (transaction != null)
                {
                    transaction.IsCancelled = true;
                    Console.WriteLine($"🚫 Transaction marked as CANCELLED: {partnerRef}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error marking transaction as cancelled: {ex.Message}");
            }
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

            // ✅ CHỈ LẤY GIAO DỊCH: 
            // - Cùng partnerCode
            // - Chưa PAID
            // - Chưa CANCELLED  
            // - ResponseCode != "99" (không vào hệ thống)
            var availableTransactions = _createdTransactions
                .Where(t => t.PartnerCode == partnerCode &&
                            !t.IsPaid &&
                            !t.IsCancelled &&
                            t.ResponseCode != "00"&&
                            t.ResponseCode != "99")
                .ToList();

            if (availableTransactions.Count > 0)
            {
                var trans = availableTransactions[rnd.Next(availableTransactions.Count)];
                partnerRef = trans.PartnerRef;
                pin = "PIN-" + agencyCode + GenerateRandomNumber(6);

                Console.WriteLine($"📤 Cancel request for available transaction: {partnerRef}");
            }
            else
            {
                // ✅ KHÔNG RANDOM - BÁO LỖI
                AppendResult($"[ERROR] ⚠️ Không có giao dịch khả dụng để cancel cho partner {partnerCode}\r\n");
                Console.WriteLine($"⚠️ No available transactions for partner {partnerCode}");
                return null; // Trả về null để không gửi request
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

            // ✅ CHỈ LẤY GIAO DỊCH: cùng partner, chưa paid, chưa cancelled, responseCode != "99"
            var availableTransactions = _createdTransactions
                .Where(t => t.PartnerCode == partnerCode &&
                            !t.IsPaid &&
                            !t.IsCancelled &&
                            t.ResponseCode != "00" &&
                            t.ResponseCode != "99")
                .ToList();

            if (availableTransactions.Count > 0)
            {
                var trans = availableTransactions[rnd.Next(availableTransactions.Count)];
                partnerRef = trans.PartnerRef;
                pin = "PIN-" + agencyCode + GenerateRandomNumber(6);

                Console.WriteLine($"📤 Update request for available transaction: {partnerRef}");
            }
            else
            {
                // ✅ KHÔNG RANDOM - BÁO LỖI
                AppendResult($"[ERROR] ⚠️ Không có giao dịch khả dụng để update cho partner {partnerCode}\r\n");
                Console.WriteLine($"⚠️ No available transactions for partner {partnerCode}");
                return null;
            }

            string receiverName = GenerateRandomName();
            string phoneNumber = "0" + GenerateRandomNumber(9);
            var bank = _banks[rnd.Next(_banks.Count)];
            var listProvince = _fieldsConfig.UseBlackListOnly ? _provincesBlackList : _provinces;
            string province = listProvince.Count > 0 ? listProvince[rnd.Next(listProvince.Count)] : "";

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
                    ["phoneNumber"] = phoneNumber,
                    ["address"] = "Address " + GenerateRandomNumber(3),
                    ["documentType"] = "CCCD",
                    ["idNumber"] = GenerateRandomNumber(12),
                    ["issueDate"] = RandomDate(2022, 2025),
                    ["issuer"] = "Gov",
                    ["nationality"] = "VN",
                    ["gender"] = rnd.Next(2) == 0 ? "M" : "F",
                    ["doB"] = RandomDate(1960, 2020),
                    ["ethnicity"] = "Kinh",
                    ["occupation"] = "Engineer",
                    ["province"] = province,
                    ["ward"] = "",
                    ["transferPurpose"] = "Gift",
                    ["senderRelationship"] = "Friend",
                    ["accountNumber"] = GenerateRandomNumber(16),
                    ["bankCode"] = bank.Code,
                    ["bankBranchCode"] = bank.Branch
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

            var paidTrans = _createdTransactions.Where(t => t.IsPaid).ToList();
            var cancelledTrans = _createdTransactions.Where(t => t.IsCancelled).ToList();
            var error99Trans = _createdTransactions.Where(t => t.ResponseCode == "99").ToList();
            var availableTrans = _createdTransactions.Where(t => !t.IsPaid && !t.IsCancelled && t.ResponseCode != "99").ToList();

            sb.AppendLine($"💰 PAID TRANSACTIONS ({paidTrans.Count}):");
            sb.AppendLine("=".PadRight(70, '='));
            for (int i = 0; i < paidTrans.Count; i++)
            {
                var t = paidTrans[i];
                sb.AppendLine($"#{i + 1}:");
                sb.AppendLine($"  ✅ PartnerRef: {t.PartnerRef}");
                sb.AppendLine($"  🆔 TransactionRef: {t.TransactionRef}");
                sb.AppendLine($"  📅 Created: {t.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine();
            }

            sb.AppendLine($"\n🚫 CANCELLED TRANSACTIONS ({cancelledTrans.Count}):");
            sb.AppendLine("=".PadRight(70, '='));
            for (int i = 0; i < cancelledTrans.Count; i++)
            {
                var t = cancelledTrans[i];
                sb.AppendLine($"#{i + 1}:");
                sb.AppendLine($"  ❌ PartnerRef: {t.PartnerRef}");
                sb.AppendLine($"  🆔 TransactionRef: {t.TransactionRef ?? "null"}");
                sb.AppendLine($"  📅 Created: {t.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine();
            }

            // ✅ THÊM SECTION MỚI CHO ERROR 99
            sb.AppendLine($"\n❌ ERROR 99 TRANSACTIONS ({error99Trans.Count}) - KHÔNG KHẢ DỤNG:");
            sb.AppendLine("=".PadRight(70, '='));
            for (int i = 0; i < error99Trans.Count; i++)
            {
                var t = error99Trans[i];
                sb.AppendLine($"#{i + 1}:");
                sb.AppendLine($"  🚨 PartnerRef: {t.PartnerRef}");
                sb.AppendLine($"  ⚠️ ResponseCode: {t.ResponseCode}");
                sb.AppendLine($"  📅 Created: {t.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine($"  ℹ️ Status: Lỗi không vào hệ thống - không thể cancel/update");
                sb.AppendLine();
            }

            sb.AppendLine($"\n✅ AVAILABLE TRANSACTIONS ({availableTrans.Count}) - CÓ THỂ CANCEL/UPDATE:");
            sb.AppendLine("=".PadRight(70, '='));
            for (int i = 0; i < availableTrans.Count; i++)
            {
                var t = availableTrans[i];
                sb.AppendLine($"#{i + 1}:");
                sb.AppendLine($"  ⏳ PartnerRef: {t.PartnerRef}");
                sb.AppendLine($"  🆔 TransactionRef: {t.TransactionRef ?? "null"}");
                sb.AppendLine($"  💬 ResponseCode: {t.ResponseCode}");
                sb.AppendLine($"  📅 Created: {t.CreatedAt:yyyy-MM-dd HH:mm:ss}");
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

        private void WriteApiLog(string endpoint, string status, int duration,
     string responseCode, string error, string requestJson, string responseJson)
        {
            try
            {
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "re");
                if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

                string logPath = Path.Combine(logDir, $"logs_all_apis_{DateTime.Now:yyyyMMdd}.txt");

                var sb = new StringBuilder();
                sb.AppendLine("----------------------------------------------------");

                string statusMarker = "";
                string partnerRef = "";
                string transactionRef = "";
                string transactionStatus = "";

                // ✅ XỬ LÝ CHO TRANSFER - PAID MARKER
                if (endpoint.ToUpper() == "TRANSFER" && responseCode == "00")
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(responseJson);

                        string apiStatus = doc.RootElement.TryGetProperty("status", out var statusProp)
                            ? statusProp.GetString()
                            : "0";

                        if (doc.RootElement.TryGetProperty("transactionRef", out var trProp))
                        {
                            transactionRef = trProp.GetString();
                        }

                        if (doc.RootElement.TryGetProperty("partnerRef", out var prProp))
                        {
                            partnerRef = prProp.GetString();
                        }

                        if (apiStatus == "200" && !string.IsNullOrEmpty(transactionRef))
                        {
                            statusMarker = "💰 PAID";
                            transactionStatus = "PAID";
                        }
                        else
                        {
                            transactionStatus = "PENDING";
                        }
                    }
                    catch { }
                }

                // ✅ XỬ LÝ CHO CANCELTRANS - CANCELLED MARKER
                if (endpoint.ToUpper() == "CANCELTRANS" && responseCode == "00")
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(responseJson);

                        string apiStatus = doc.RootElement.TryGetProperty("status", out var statusProp)
                            ? statusProp.GetString()
                            : "0";

                        if (doc.RootElement.TryGetProperty("partnerRef", out var prProp))
                        {
                            partnerRef = prProp.GetString();
                        }

                        if (apiStatus == "200")
                        {
                            statusMarker = "🚫 CANCELLED";
                            transactionStatus = "CANCELLED";
                        }
                    }
                    catch { }
                }

                // ✅ XỬ LÝ CHO TRANSINQ - XÁC ĐỊNH TRANSACTION STATUS
                if (endpoint.ToUpper() == "TRANSINQ" && responseCode == "00")
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(responseJson);

                        if (doc.RootElement.TryGetProperty("partnerRef", out var prProp))
                        {
                            partnerRef = prProp.GetString();
                        }

                        if (doc.RootElement.TryGetProperty("transactionRef", out var trProp))
                        {
                            transactionRef = trProp.GetString();
                        }

                        // ✅ TÌM TRANSACTION STATUS TỪ MEMORY
                        var transaction = _createdTransactions.FirstOrDefault(t => t.PartnerRef == partnerRef);
                        if (transaction != null)
                        {
                            if (transaction.IsPaid)
                            {
                                statusMarker = "💰 PAID";
                                transactionStatus = "PAID";
                            }
                            else if (transaction.IsCancelled)
                            {
                                statusMarker = "🚫 CANCELLED";
                                transactionStatus = "CANCELLED";
                            }
                            else
                            {
                                statusMarker = "⏳ PENDING";
                                transactionStatus = "PENDING";
                            }
                        }
                    }
                    catch { }
                }

                sb.AppendLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - {endpoint.ToUpper()} {statusMarker}");
                sb.AppendLine($"Status: {status}");
                sb.AppendLine($"Duration: {duration} ms");
                sb.AppendLine($"ResponseCode: {responseCode}");

                // ✅ THÔNG TIN CHO PAID
                if (statusMarker == "💰 PAID")
                {
                    sb.AppendLine($"🔒 Transaction PAID - PartnerRef: {partnerRef}");
                    sb.AppendLine($"🆔 TransactionRef: {transactionRef}");
                }

                // ✅ THÔNG TIN CHO CANCELLED
                if (statusMarker == "🚫 CANCELLED")
                {
                    sb.AppendLine($"❌ Transaction CANCELLED - PartnerRef: {partnerRef}");
                }

                // ✅ THÔNG TIN CHO PENDING (TRANSINQ)
                if (statusMarker == "⏳ PENDING")
                {
                    sb.AppendLine($"⏳ Transaction PENDING - PartnerRef: {partnerRef}");
                }

                if (!string.IsNullOrEmpty(error))
                    sb.AppendLine($"Error: {error}");
                sb.AppendLine("REQUEST:");
                sb.AppendLine(string.IsNullOrWhiteSpace(requestJson) ? "No data" : requestJson);
                sb.AppendLine("RESPONSE:");
                sb.AppendLine(string.IsNullOrWhiteSpace(responseJson) ? "No data" : responseJson);
                sb.AppendLine("----------------------------------------------------");

                File.AppendAllText(logPath, sb.ToString(), Encoding.UTF8);
                AppendResult(sb.ToString());
            }
            catch { }
        }
        
        
        private async void btnAutoTest_Click(object sender, EventArgs e)
        {
            if (_isAutoTesting)
            {
                _isAutoTesting = false;
                btnAutoTest.Text = "🤖 Auto Test";
                btnAutoTest.BackColor = SystemColors.Control;
                AppendResult("[AUTO TEST] ⏹️ Đã dừng auto test\r\n");
                return;
            }

            if (string.IsNullOrEmpty(txtPartnerCode.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập Partner Code");
                return;
            }
            if (string.IsNullOrEmpty(txtAgencyCode.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập Agency Code");
                return;
            }
            
            _isAutoTesting = true;
            _autoTestStep = 1;
            btnAutoTest.Text = "⏸️ Stop Auto Test";
            btnAutoTest.BackColor = Color.FromArgb(255, 200, 100);
            btnSendRequest.Enabled = false;
            btnStartAutoPush.Enabled = false;

            AppendResult("\r\n");
            AppendResult("╔══════════════════════════════════════════════════════════════════╗\r\n");
            AppendResult("║                  🤖 AUTO TEST STARTED                           ║\r\n");
            AppendResult("╚══════════════════════════════════════════════════════════════════╝\r\n");
            AppendResult("\r\n");

            await RunAutoTest();

            _isAutoTesting = false;
            btnAutoTest.Text = "🤖 Auto Test";
            btnAutoTest.BackColor = SystemColors.Control;
            btnSendRequest.Enabled = true;
            btnStartAutoPush.Enabled = true;

            AppendResult("\r\n");
            AppendResult("╔══════════════════════════════════════════════════════════════════╗\r\n");
            AppendResult("║                  ✅ AUTO TEST COMPLETED                         ║\r\n");
            AppendResult("╚══════════════════════════════════════════════════════════════════╝\r\n");
            AppendResult("\r\n");

            MessageBox.Show("✅ Auto Test hoàn thành!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task RunAutoTest()
        {
            string partnerCode = txtPartnerCode.Text.Trim();
            string agencyCode = txtAgencyCode.Text.Trim();

            try
            {
                // ======================================================================
                // STEP 1: HEALTHCHECK - Gọi đến khi thành công tối đa 5 lần
                // ======================================================================
                AppendResult("[STEP 1] 🏥 Testing HEALTHCHECK (max 5 success calls)...\r\n");
                int healthAttempts = 0;

                while ( healthAttempts < 10 && _isAutoTesting)
                {
                    healthAttempts++;
                    bool isSuccess = await SendHealthCheckRequest(partnerCode, agencyCode, healthAttempts);
                    await Task.Delay(100);

                    if (isSuccess)
                        break;
                }

                AppendResult($"[STEP 1] ✅ HEALTHCHECK completed: {healthAttempts}/10 success\r\n\r\n");

                if (healthAttempts >= 10 && !_createdTransactions.Any() && !_isAutoTesting)
                    return;

                // Nếu sau 10 lần không có lần nào thành công thì dừng luôn
                if (healthAttempts >= 10 && !_isAutoTesting)
                {
                    AppendResult("[STEP 1] ❌ HEALTHCHECK failed 10 times, auto test stopped.\r\n\r\n");
                    return;
                }

                if (!_isAutoTesting) return;

                // ======================================================================
                // STEP 2: TRANSFER - Tối thiểu 5 pending, tối đa 150
                // ======================================================================
                AppendResult("[STEP 2] 💸 Testing TRANSFER (min 5 pending, max 150)...\r\n");
                int transferCount = 0;
                int maxTransfers = 150;

                while (transferCount < maxTransfers && _isAutoTesting)
                {
                    await SendApiRequest("transfer", partnerCode, agencyCode, transferCount + 1);
                    transferCount++;
                    await Task.Delay(50);

                    // Kiểm tra số pending (không paid, không cancelled)
                    var pendingCount = _createdTransactions.Count(t => !t.IsPaid && !t.IsCancelled);
                    if (pendingCount >= 5)
                    {
                        // Đã đủ 5 giao dịch chưa paid, chưa cancel thì dừng
                        break;
                    }
                }

                var finalPending = _createdTransactions.Count(t => !t.IsPaid && !t.IsCancelled);
                AppendResult($"[STEP 2] ✅ TRANSFER completed: {transferCount} transfers, {finalPending} pending\r\n\r\n");
                if (!_isAutoTesting) return;

                // ======================================================================
                // STEP 3: QUERYINFOR - Gọi tất cả currencies (tối đa 10)
                // ======================================================================
                AppendResult("[STEP 3] 💰 Testing QUERYINFOR (all currencies, max 10)...\r\n");
                string[] currencies = { "VND", "USD", "EUR", "JPY", "GBP", "AUD", "CAD", "CHF", "CNY", "SGD" };
                int queryCount = 0;

                foreach (var currency in currencies.Take(10))
                {
                    if (!_isAutoTesting) break;

                    // Tạm thời thay đổi comboCurrency
                    string originalCurrency = comboCurrency.SelectedItem?.ToString();
                    comboCurrency.SelectedItem = currency;

                    await SendApiRequest("queryinfor", partnerCode, agencyCode, ++queryCount);
                    await Task.Delay(100);

                    // Khôi phục
                    if (originalCurrency != null)
                        comboCurrency.SelectedItem = originalCurrency;
                }

                AppendResult($"[STEP 3] ✅ QUERYINFOR completed: {queryCount} currencies tested\r\n\r\n");

                if (!_isAutoTesting) return;

                // ======================================================================
                // STEP 4: CANCELTRANS - Cancel 3 giao dịch pending
                // ======================================================================
                AppendResult("[STEP 4] 🚫 Testing CANCELTRANS (3 pending transactions)...\r\n");
                int cancelCount = 0;
                int cancelTarget = 3;

                for (int i = 0; i < cancelTarget; i++)
                {
                    if (!_isAutoTesting) break;

                    var availablePending = _createdTransactions
                        .Where(t => t.PartnerCode == partnerCode &&
                           !t.IsPaid &&
                           !t.IsCancelled &&
                           t.ResponseCode != "00" &&
                           t.ResponseCode != "99")
               .ToList();
               
                    if (availablePending.Count == 0)
                    {
                        AppendResult($"[STEP 4] ⚠️ Hết giao dịch pending để cancel sau {cancelCount} attempts\r\n");
                        break;
                    }

                    await SendApiRequest("canceltrans", partnerCode, agencyCode, i + 1);
                    cancelCount++;
                    await Task.Delay(100);
                }

                AppendResult($"[STEP 4] ✅ CANCELTRANS completed: {cancelCount}/{cancelTarget} cancelled\r\n\r\n");

                if (!_isAutoTesting) return;

                // ======================================================================
                // STEP 5: ACCTINQ - Random 10 lần
                // ======================================================================
                AppendResult("[STEP 5] 🏦 Testing ACCTINQ (10 random calls)...\r\n");

                for (int i = 0; i < 10; i++)
                {
                    if (!_isAutoTesting) break;

                    await SendApiRequest("acctinq", partnerCode, agencyCode, i + 1);
                    await Task.Delay(100);
                }

                AppendResult($"[STEP 5] ✅ ACCTINQ completed: 10/10 calls\r\n\r\n");

                if (!_isAutoTesting) return;

                // ======================================================================
                // STEP 6: TRANSINQ - 4 paid, 3 cancelled, 3 pending
                // ======================================================================
                AppendResult("[STEP 6] 🔍 Testing TRANSINQ (4 paid + 3 cancelled + 3 pending)...\r\n");

                // Test PAID transactions
                var paidTrans = _createdTransactions.Where(t => t.IsPaid).Take(4).ToList();
                AppendResult($"[STEP 6.1] Testing {paidTrans.Count} PAID transactions...\r\n");
                foreach (var trans in paidTrans)
                {
                    if (!_isAutoTesting) break;
                    await SendTransInqRequest(partnerCode, trans.PartnerRef);
                    await Task.Delay(100);
                }

                // Test CANCELLED transactions
                var cancelledTrans = _createdTransactions.Where(t => t.IsCancelled).Take(3).ToList();
                AppendResult($"[STEP 6.2] Testing {cancelledTrans.Count} CANCELLED transactions...\r\n");
                foreach (var trans in cancelledTrans)
                {
                    if (!_isAutoTesting) break;
                    await SendTransInqRequest(partnerCode, trans.PartnerRef);
                    await Task.Delay(100);
                }

                // Test PENDING transactions
                var pendingTrans = _createdTransactions
                    .Where(t => !t.IsPaid && !t.IsCancelled)
                    .Take(3)
                    .ToList();
                AppendResult($"[STEP 6.3] Testing {pendingTrans.Count} PENDING transactions...\r\n");
                foreach (var trans in pendingTrans)
                {
                    if (!_isAutoTesting) break;
                    await SendTransInqRequest(partnerCode, trans.PartnerRef);
                    await Task.Delay(100);
                }

                AppendResult($"[STEP 6] ✅ TRANSINQ completed: {paidTrans.Count + cancelledTrans.Count + pendingTrans.Count} calls\r\n\r\n");

            }
            catch (Exception ex)
            {
                AppendResult($"[AUTO TEST ERROR] ❌ {ex.Message}\r\n");
            }
        }
        #region auto test
        private async Task<bool> SendHealthCheckRequest(string partnerCode, string agencyCode, int stt)
        {
            string url = $"https://58.186.16.67/api/partner/healthcheck";
            try
            {
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

                WriteApiLog("HEALTHCHECK", healthResponse.IsSuccessStatusCode ? "SUCCESS" : "FAILED",
                    (int)elapsed.TotalMilliseconds, healthResponse.IsSuccessStatusCode ? "00" : healthResponse.StatusCode.ToString(),
                    null, null, healthResult);

                // Chỉ trả về true nếu HTTP 200
                return healthResponse.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                WriteApiLog("HEALTHCHECK", "ERROR", 0, "", ex.Message, null, null);
                return false;
            }
        }
        #endregion




        // Helper method cho TRANSINQ
        private async Task SendTransInqRequest(string partnerCode, string partnerRef)
        {
            string json = CreateTransInqRequestWithPartnerRef(partnerCode, partnerRef);
            string url = $"https://58.186.16.67/api/partner/transinq";

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
                }
                catch
                {
                    responseCode = "(parse error)";
                }

                string formattedRequest = FormatJsonForLog(json);
                string formattedResponse = FormatJsonForLog(result);

                string status = responseCode == "00" ? "SUCCESS" : "FAILED";
                int durationMs = (int)elapsed.TotalMilliseconds;
                WriteApiLog("transinq", status, durationMs, responseCode, null, formattedRequest, formattedResponse);
            }
            catch (Exception ex)
            {
                WriteApiLog("transinq", "ERROR", 0, "", ex.Message, json, null);
            }
        }

        private string CreateTransInqRequestWithPartnerRef(string partnerCode, string partnerRef)
        {
            var root = new Dictionary<string, object>
            {
                ["refNo"] = Guid.NewGuid().ToString(),
                ["partnerCode"] = partnerCode,
                ["partnerRef"] = partnerRef,
                ["pin"] = "PIN-" + GenerateRandomNumber(6)
            };

            return JsonSerializer.Serialize(root, new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }
        // 5. HÀM MỚI: DISPLAY TRANSACTION STATISTICS
        private void DisplayTransactionStatistics()
        {
            var totalTransactions = _createdTransactions.Count;
            var paidTransactions = _createdTransactions.Count(t => t.IsPaid);
            var cancelledTransactions = _createdTransactions.Count(t => t.IsCancelled);
            var error99Transactions = _createdTransactions.Count(t => t.ResponseCode == "99");
            var availableTransactions = _createdTransactions.Count(t => !t.IsPaid && !t.IsCancelled && t.ResponseCode != "99");

            var statsText = $@"
╔══════════════════════════════════════════╗
║     TRANSACTION STATISTICS              ║
╠══════════════════════════════════════════╣
║ Total Transactions: {totalTransactions,17} ║
║ 💰 Paid:            {paidTransactions,17} ║
║ 🚫 Cancelled:       {cancelledTransactions,17} ║
║ ❌ Error 99:        {error99Transactions,17} ║
║ ✅ Available:       {availableTransactions,17} ║
╚══════════════════════════════════════════╝
";

            AppendResult(statsText);
        }
        public class TransactionInfo
        {
            public string RefNo { get; set; }
            public string PartnerRef { get; set; }
            public string PartnerCode { get; set; }
            public string TransactionRef { get; set; }
            public bool IsPaid { get; set; }
            public bool IsCancelled { get; set; }
            public string ResponseCode { get; set; }  // ✅ THÊM MỚI
            public DateTime CreatedAt { get; set; }

            public TransactionInfo()
            {
                CreatedAt = DateTime.Now;
                IsPaid = false;
                IsCancelled = false;
                ResponseCode = "00";  // ✅ DEFAULT
            }
        }
    }

    };
