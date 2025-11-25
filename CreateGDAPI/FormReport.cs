using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Text.Json;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;



namespace CreateGDAPI
{
    public class ApiRequestLog
    {
        public DateTime Timestamp { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string RefNo { get; set; } = string.Empty;
        public string PartnerRef { get; set; } = string.Empty;
        public string? TransactionRef { get; set; } // Nullable
        public bool IsPaid { get; set; }
        public bool IsCancelled { get; set; }
        public string TransactionStatus { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public string RequestJson { get; set; } = string.Empty;
        public string ResponseJson { get; set; } = string.Empty;

        // 1. Thêm 2 property mới vào ApiRequestLog để lưu balance và currency:
        public decimal? Balance { get; set; } // Số dư
        public string Currency { get; set; } = string.Empty; // Loại tiền
        // ✅ THÊM PROPERTY MỚI CHO DEBUGDESC
        public string DebugDesc { get; set; } = string.Empty;
    }

    public class ApiStatistics
    {
        public string Endpoint { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public double SuccessRate { get; set; }
        public int AvgDuration { get; set; }
        public int MinDuration { get; set; }
        public int MaxDuration { get; set; }
    }

    public partial class FormReport : Form
    {
        private List<ApiRequestLog> _logs = new List<ApiRequestLog>();
        private string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "re");
        // ✅ THÊM BIẾN CHO SORTING
        private SortOrder _currentSortOrder = SortOrder.None;
        private string _currentSortColumn = ""; 
        private DatabaseHelper _dbHelper;
        public FormReport()
        {
            InitializeComponent();
        }

        // Change the event handler to async Task
        private async void FormReport_Load(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Now.Date;
            dtpTo.Value = DateTime.Now.Date.AddDays(1).AddSeconds(-1);

            comboFilterEndpoint.Items.Add("-- All Endpoints --");
            comboFilterEndpoint.Items.AddRange(new string[]
            {
        "HEALTHCHECK", "TRANSFER", "ACCTINQ", "CANCELTRANS",
        "QUERYINFOR", "TRANSINQ", "UPDATETRANS"
            });
            comboFilterEndpoint.SelectedIndex = 0;

            // ✅ THÊM ERROR HANDLER
            dgvDetails.DataError += DgvDetails_DataError;
            dgvStatistics.DataError += DgvStatistics_DataError;
            // ✅ ENABLE SORTING CHO DATAGRIDVIEW
            dgvDetails.AllowUserToOrderColumns = true;
            dgvStatistics.AllowUserToOrderColumns = true;

            // ✅ THÊM EVENT HANDLER CHO COLUMN HEADER CLICK
            dgvDetails.ColumnHeaderMouseClick += DgvDetails_ColumnHeaderMouseClick;
            dgvStatistics.ColumnHeaderMouseClick += DgvStatistics_ColumnHeaderMouseClick;
            txtSearchPartnerRef.TextChanged += txtSearchPartnerRef_TextChanged;

            // ✅ Ensure proper DataGridView settings
            dgvDetails.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDetails.MultiSelect = false;
            dgvDetails.ReadOnly = true;
            dgvDetails.AllowUserToAddRows = false;
            dgvDetails.AllowUserToDeleteRows = false;

            // ✅ Re-bind event handler (ensures it's attached)
            dgvDetails.CellClick -= dgvDetails_CellClick;
            dgvDetails.CellClick += dgvDetails_CellClick;

            // ✅ Add visual feedback on hover
            dgvDetails.CellMouseEnter += (s, e) => {
                if (e.RowIndex >= 0) dgvDetails.Cursor = Cursors.Hand;
            };
            dgvDetails.CellMouseLeave += (s, e) => {
                dgvDetails.Cursor = Cursors.Default;
            };

            string connStr = System.Configuration.ConfigurationManager
        .ConnectionStrings["CreateGDAPI_DB"].ConnectionString;
            _dbHelper = new DatabaseHelper(connStr);

            // Load all logs from database (replaces reading from text files)
            await LoadLogsFromDatabase();

            // Initialize paging helpers (if you still want paging UI)
            InitializePagingHelpers();
        }
        private async Task LoadLogsFromDatabase()
        {
            // Load all records within the selected date range and filters directly from DB
            // Pass a large maxRecords (GetApiLogsAsync uses TOP (@MaxRecords)); if table huge, consider paging.
            _logs = await _dbHelper.GetApiLogsAsync(
                fromDate: dtpFrom.Value,
                toDate: dtpTo.Value,
                endpoint: comboFilterEndpoint.SelectedItem?.ToString() == "-- All Endpoints --" ? null : comboFilterEndpoint.SelectedItem?.ToString(),
                partnerRef: txtSearchPartnerRef?.Text?.Trim(),
                maxRecords: int.MaxValue
            );

            UpdateStatistics();
            UpdateDetailGrid();
        }
        // ✅ EVENT HANDLER CHO SORTING DETAILS GRID
        private void DgvDetails_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvDetails.Columns.Count == 0 || e.ColumnIndex < 0) return;

            string columnName = dgvDetails.Columns[e.ColumnIndex].Name;

            // Toggle sort order
            if (_currentSortColumn == columnName)
            {
                _currentSortOrder = _currentSortOrder == SortOrder.Ascending
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
            }
            else
            {
                _currentSortColumn = columnName;
                _currentSortOrder = SortOrder.Ascending;
            }

            // Sort the data
            SortDetailGrid(columnName, _currentSortOrder);
        }

        // ✅ EVENT HANDLER CHO SORTING STATISTICS GRID
        private void DgvStatistics_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (dgvStatistics.Columns.Count == 0 || e.ColumnIndex < 0) return;

            var column = dgvStatistics.Columns[e.ColumnIndex];
            var stats = dgvStatistics.DataSource as List<ApiStatistics>;

            if (stats == null) return;

            // Toggle sort order
            ListSortDirection direction = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending
                ? ListSortDirection.Descending
                : ListSortDirection.Ascending;

            // Sort based on column
            switch (column.Name)
            {
                case "Endpoint":
                    stats = direction == ListSortDirection.Ascending
                        ? stats.OrderBy(s => s.Endpoint).ToList()
                        : stats.OrderByDescending(s => s.Endpoint).ToList();
                    break;
                case "TotalRequests":
                    stats = direction == ListSortDirection.Ascending
                        ? stats.OrderBy(s => s.TotalRequests).ToList()
                        : stats.OrderByDescending(s => s.TotalRequests).ToList();
                    break;
                case "SuccessCount":
                    stats = direction == ListSortDirection.Ascending
                        ? stats.OrderBy(s => s.SuccessCount).ToList()
                        : stats.OrderByDescending(s => s.SuccessCount).ToList();
                    break;
                case "FailedCount":
                    stats = direction == ListSortDirection.Ascending
                        ? stats.OrderBy(s => s.FailedCount).ToList()
                        : stats.OrderByDescending(s => s.FailedCount).ToList();
                    break;
                case "SuccessRate":
                    stats = direction == ListSortDirection.Ascending
                        ? stats.OrderBy(s => s.SuccessRate).ToList()
                        : stats.OrderByDescending(s => s.SuccessRate).ToList();
                    break;
                case "AvgDuration":
                    stats = direction == ListSortDirection.Ascending
                        ? stats.OrderBy(s => s.AvgDuration).ToList()
                        : stats.OrderByDescending(s => s.AvgDuration).ToList();
                    break;
            }

            dgvStatistics.DataSource = stats;
            column.HeaderCell.SortGlyphDirection = direction == ListSortDirection.Ascending
                ? SortOrder.Ascending
                : SortOrder.Descending;
        }

        // ✅ HÀM SORT DETAIL GRID
        private void SortDetailGrid(string columnName, SortOrder order)
        {
            var logs = dgvDetails.DataSource as List<ApiRequestLog>;
            if (logs == null) return;

            IEnumerable<ApiRequestLog> sortedLogs = logs;

            switch (columnName)
            {
                case "Timestamp":
                    sortedLogs = order == SortOrder.Ascending
                        ? logs.OrderBy(l => l.Timestamp)
                        : logs.OrderByDescending(l => l.Timestamp);
                    break;
                case "Endpoint":
                    sortedLogs = order == SortOrder.Ascending
                        ? logs.OrderBy(l => l.Endpoint)
                        : logs.OrderByDescending(l => l.Endpoint);
                    break;
                case "ResponseCode":
                    sortedLogs = order == SortOrder.Ascending
                        ? logs.OrderBy(l => l.ResponseCode)
                        : logs.OrderByDescending(l => l.ResponseCode);
                    break;
                case "Status":
                    sortedLogs = order == SortOrder.Ascending
                        ? logs.OrderBy(l => l.Status)
                        : logs.OrderByDescending(l => l.Status);
                    break;
                case "Duration":
                    sortedLogs = order == SortOrder.Ascending
                        ? logs.OrderBy(l => l.Duration)
                        : logs.OrderByDescending(l => l.Duration);
                    break;
                case "TransactionStatus":
                    sortedLogs = order == SortOrder.Ascending
                        ? logs.OrderBy(l => l.TransactionStatus)
                        : logs.OrderByDescending(l => l.TransactionStatus);
                    break;
                case "PartnerRef":
                    sortedLogs = order == SortOrder.Ascending
                        ? logs.OrderBy(l => l.PartnerRef)
                        : logs.OrderByDescending(l => l.PartnerRef);
                    break;
                case "ErrorMessage":
                    sortedLogs = order == SortOrder.Ascending
                        ? logs.OrderBy(l => l.ErrorMessage)
                        : logs.OrderByDescending(l => l.ErrorMessage);
                    break;
            }

            dgvDetails.DataSource = sortedLogs.ToList();

            // Set sort glyph
            foreach (DataGridViewColumn col in dgvDetails.Columns)
            {
                col.HeaderCell.SortGlyphDirection = SortOrder.None;
            }

            if (dgvDetails.Columns.Contains(columnName))
            {
                dgvDetails.Columns[columnName].HeaderCell.SortGlyphDirection = order;
            }

            // Reapply formatting
            ApplyDetailGridFormatting();
        }
        
        private void DgvDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }

        private void DgvStatistics_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = true;
        }
      
        private void LoadLogs()
        {
            // Removed: reading from text files. All data now comes from database.
            // Kept method as empty placeholder in case designer wires it; prefer calling LoadLogsFromDatabase().
        }

        private ApiRequestLog ParseLogEntry(string logEntry)
        {
            // Kept ParseLogEntry logic only if you still want to parse legacy txt files.
            // For pure DB mode this method is unused.
            try
            {
                var log = new ApiRequestLog();
                var lines = logEntry.Split('\n');

                bool isInRequest = false;
                bool isInResponse = false;
                var requestBuilder = new StringBuilder();
                var responseBuilder = new StringBuilder();

                foreach (var line in lines)
                {
                    if (line.Contains("[") && line.Contains("]") && line.Contains(" - "))
                    {
                        var timestampMatch = System.Text.RegularExpressions.Regex.Match(
                            line, @"\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\]");
                        if (timestampMatch.Success &&
                            DateTime.TryParse(timestampMatch.Groups[1].Value, out var timestamp))
                        {
                            log.Timestamp = timestamp;
                        }

                        var parts = line.Split(new[] { " - " }, StringSplitOptions.None);
                        if (parts.Length > 1)
                        {
                            string endpointPart = parts[1].Trim();

                            log.Endpoint = endpointPart
                                .Replace("💰 PAID", "")
                                .Replace("🚫 CANCELLED", "")
                                .Replace("🚫 CANCELLED [UPDATED]", "")
                                .Replace("⏳ PENDING", "")
                                .Trim()
                                .ToUpper();

                            if (endpointPart.Contains("💰 PAID"))
                            {
                                log.IsPaid = true;
                                log.TransactionStatus = "PAID";
                            }
                            else if (endpointPart.Contains("🚫 CANCELLED"))
                            {
                                log.IsCancelled = true;
                                log.TransactionStatus = "CANCELLED";
                            }
                            else if (endpointPart.Contains("⏳ PENDING"))
                            {
                                log.TransactionStatus = "PENDING";
                            }
                        }
                    }

                    if (line.StartsWith("Status:"))
                    {
                        log.Status = line.Split(':')[1].Trim();
                    }
                    else if (line.StartsWith("ResponseCode:"))
                    {
                        log.ResponseCode = line.Split(':')[1].Trim();
                    }
                    // ✅ PARSE DURATION
                    else if (line.StartsWith("Duration:"))
                    {
                        var parts = line.Split(':');
                        if (parts.Length >= 2)
                        {
                            var durationText = parts[1].Trim().Replace("ms", "").Trim();
                            if (int.TryParse(durationText, out int duration))
                            {
                                log.Duration = duration;
                            }
                        }
                    }
                    else if (line.StartsWith("PartnerRef:"))
                    {
                        log.PartnerRef = line.Split(':')[1].Trim();
                    }
                    else if (line.StartsWith("TransactionRef:"))
                    {
                        log.TransactionRef = line.Split(':')[1].Trim();
                    }
                    else if (line.StartsWith("TransactionStatus:"))
                    {
                        var status = line.Split(':')[1].Trim();
                        log.TransactionStatus = status;

                        if (status == "PAID")
                            log.IsPaid = true;
                        else if (status == "CANCELLED")
                            log.IsCancelled = true;
                    }
                    else if (line.StartsWith("Error:"))
                    {
                        log.ErrorMessage = line.Substring(6).Trim();
                    }

                    if (line.Trim() == "REQUEST:")
                    {
                        isInRequest = true;
                        isInResponse = false;
                        continue;
                    }

                    if (line.Trim() == "RESPONSE:")
                    {
                        isInRequest = false;
                        isInResponse = true;
                        continue;
                    }

                    if (isInRequest)
                    {
                        requestBuilder.AppendLine(line);
                    }
                    else if (isInResponse)
                    {
                        responseBuilder.AppendLine(line);
                    }
                }

                log.RequestJson = requestBuilder.ToString().Trim();
                log.ResponseJson = responseBuilder.ToString().Trim();

                // ✅ PARSE DEBUGDESC CHO TRANSFER PENDING
                if (log.Endpoint == "TRANSFER")
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(log.ResponseJson);
                        if (doc.RootElement.TryGetProperty("response", out var responseObj) &&
                            responseObj.TryGetProperty("debugDesc", out var debugDescProp))
                        {
                            log.DebugDesc = debugDescProp.GetString() ?? "";

                            // Nếu có debugDesc thì hiển thị trong ErrorMessage
                            if (!string.IsNullOrEmpty(log.DebugDesc))
                            {
                                log.ErrorMessage = log.DebugDesc;
                            }
                        }
                    }
                    catch { }
                }

                if (log.Endpoint == "TRANSFER")
                {
                    if (log.Status == "202" || log.ResponseCode == "05" || log.ResponseCode == "98")
                    {
                        log.TransactionStatus = "PENDING";
                        log.Status = "SUCCESS";
                    }
                }

                if (log.Endpoint == "CANCELTRANS")
                {
                    if (log.ResponseCode == "07")
                    {
                        log.Status = "SUCCESS";
                        log.TransactionStatus = "NOT_ALLOWED";
                    }
                }

                if (log.ResponseCode == "99")
                {
                    log.TransactionStatus = "ERROR_99";
                }

                if (log.Endpoint == "QUERYINFOR")
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(log.ResponseJson);
                        if (doc.RootElement.TryGetProperty("response", out var responseArray)
                            && responseArray.ValueKind == JsonValueKind.Array
                            && responseArray.GetArrayLength() > 0)
                        {
                            var firstItem = responseArray[0];
                            if (firstItem.TryGetProperty("balance", out var balanceProp))
                            {
                                if (balanceProp.TryGetDecimal(out var balance))
                                    log.Balance = balance;
                            }
                            if (firstItem.TryGetProperty("currency", out var currencyProp))
                            {
                                log.Currency = currencyProp.GetString() ?? "";
                            }
                            log.Status = "SUCCESS";
                        }
                    }
                    catch { }
                }

                if (log.Endpoint == "TRANSINQ")
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(log.ResponseJson);
                        if (doc.RootElement.TryGetProperty("response", out var responseObj)
                            && responseObj.ValueKind == JsonValueKind.Object)
                        {
                            if (responseObj.TryGetProperty("responseCode", out var respCodeProp)
                                && respCodeProp.GetString() == "07")
                            {
                                log.TransactionStatus = "Payable";
                                log.Status = "SUCCESS";
                            }
                        }
                    }
                    catch { }
                }

                return log;
            }
            catch
            {
                return null;
            }
        }
        private void UpdateStatistics()
        {
            var filteredLogs = _logs.Where(l =>
                l.Timestamp >= dtpFrom.Value &&
                l.Timestamp <= dtpTo.Value).ToList();

            string selectedEndpoint = comboFilterEndpoint.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedEndpoint) && selectedEndpoint != "-- All Endpoints --")
            {
                filteredLogs = filteredLogs.Where(l => l.Endpoint == selectedEndpoint).ToList();
            }

            var stats = filteredLogs
                .GroupBy(l => l.Endpoint ?? "UNKNOWN")
                .Select(g => new ApiStatistics
                {
                    Endpoint = g.Key,
                    TotalRequests = g.Count(),
                    SuccessCount = g.Count(l => l.Status == "SUCCESS"),
                    FailedCount = g.Key == "TRANSFER"
                        ? g.Count(l => l.Status == "FAILED" && l.TransactionStatus != "PENDING")
                        : g.Count(l => l.Status == "FAILED"),
                    SuccessRate = g.Count() > 0 ? (g.Count(l => l.Status == "SUCCESS") * 100.0 / g.Count()) : 0,
                    AvgDuration = g.Where(l => l.Duration > 0).Any() ? (int)g.Where(l => l.Duration > 0).Average(l => l.Duration) : 0,
                    MinDuration = g.Where(l => l.Duration > 0).Any() ? g.Where(l => l.Duration > 0).Min(l => l.Duration) : 0,
                    MaxDuration = g.Where(l => l.Duration > 0).Any() ? g.Where(l => l.Duration > 0).Max(l => l.Duration) : 0
                })
                .OrderByDescending(s => s.TotalRequests)
                .ToList();

            dgvStatistics.DataSource = stats;

            if (dgvStatistics.Columns.Count > 0)
            {
                dgvStatistics.Columns["Endpoint"].HeaderText = "API Endpoint";
                dgvStatistics.Columns["Endpoint"].Width = 150;
                dgvStatistics.Columns["TotalRequests"].HeaderText = "Total";
                dgvStatistics.Columns["TotalRequests"].Width = 80;
                dgvStatistics.Columns["SuccessCount"].HeaderText = "Success";
                dgvStatistics.Columns["SuccessCount"].Width = 80;
                dgvStatistics.Columns["FailedCount"].HeaderText = "Failed";
                dgvStatistics.Columns["FailedCount"].Width = 80;
                dgvStatistics.Columns["SuccessRate"].HeaderText = "Success Rate %";
                dgvStatistics.Columns["SuccessRate"].Width = 120;
                dgvStatistics.Columns["SuccessRate"].DefaultCellStyle.Format = "N2";
                dgvStatistics.Columns["AvgDuration"].HeaderText = "Avg (ms)";
                dgvStatistics.Columns["AvgDuration"].Width = 90;
                dgvStatistics.Columns["MinDuration"].HeaderText = "Min (ms)";
                dgvStatistics.Columns["MinDuration"].Width = 90;
                dgvStatistics.Columns["MaxDuration"].HeaderText = "Max (ms)";
                dgvStatistics.Columns["MaxDuration"].Width = 90;

                foreach (DataGridViewRow row in dgvStatistics.Rows)
                {
                    if (row.Cells["SuccessRate"].Value != null)
                    {
                        double rate = Convert.ToDouble(row.Cells["SuccessRate"].Value);
                        if (rate >= 90)
                            row.Cells["SuccessRate"].Style.BackColor = Color.LightGreen;
                        else if (rate >= 70)
                            row.Cells["SuccessRate"].Style.BackColor = Color.LightYellow;
                        else
                            row.Cells["SuccessRate"].Style.BackColor = Color.LightPink;
                    }
                }
            }

            int totalSuccess = filteredLogs.Count(l => l.Status == "SUCCESS");
            int totalFailed = filteredLogs.Count(l => l.Status == "FAILED");
            int totalRequests = filteredLogs.Count;

            lblTotalRequests.Text = $"📊 Tổng số requests: {totalRequests}";
            lblSuccessCount.Text = $"✅ Thành công: {totalSuccess}";
            lblFailedCount.Text = $"❌ Thất bại: {totalFailed}";

            if (totalRequests > 0)
            {
                double overallRate = (totalSuccess * 100.0 / totalRequests);
                lblSuccessRate.Text = $"📈 Tỷ lệ thành công: {overallRate:N2}%";

                if (overallRate >= 90)
                    lblSuccessRate.ForeColor = Color.Green;
                else if (overallRate >= 70)
                    lblSuccessRate.ForeColor = Color.Orange;
                else
                    lblSuccessRate.ForeColor = Color.Red;
            }
            else
            {
                lblSuccessRate.Text = "📈 Tỷ lệ thành công: N/A";
            }
        }

        // 1. Sửa hàm UpdateDetailGrid để nhận thêm tham số endpoint (string? endpoint = null)
        private void UpdateDetailGrid(string? SEndpoint = null)
        {
            var filteredLogs = _logs.Where(l =>
                l.Timestamp >= dtpFrom.Value &&
                l.Timestamp <= dtpTo.Value).ToList();

            string selectedEndpoint = SEndpoint ?? comboFilterEndpoint.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedEndpoint) && selectedEndpoint != "-- All Endpoints --")
            {
                filteredLogs = filteredLogs.Where(l => l.Endpoint == selectedEndpoint).ToList();
            }
            // ✅ THÊM MỚI: Filter theo PartnerRef
            string searchPartnerRef = txtSearchPartnerRef?.Text?.Trim();
            if (!string.IsNullOrEmpty(searchPartnerRef))
            {
                // Tìm kiếm không phân biệt hoa thường, có thể tìm một phần
                filteredLogs = filteredLogs.Where(l =>
                    !string.IsNullOrEmpty(l.PartnerRef) &&
                    l.PartnerRef.IndexOf(searchPartnerRef, StringComparison.OrdinalIgnoreCase) >= 0
                ).ToList();
            }
            filteredLogs = filteredLogs.OrderByDescending(l => l.Timestamp).ToList();
            dgvDetails.DataSource = filteredLogs;

            if (dgvDetails.Columns.Count > 0)
            {
                ConfigureDetailGridColumns();
                ApplyDetailGridFormatting();
            }
        }

        // ✅ TÁCH RA HÀM RIÊNG ĐỂ TÁI SỬ DỤNG
        private void ConfigureDetailGridColumns()
        {
            dgvDetails.Columns["Timestamp"].HeaderText = "Thời gian";
            dgvDetails.Columns["Timestamp"].Width = 150;
            dgvDetails.Columns["Timestamp"].DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";

            dgvDetails.Columns["Endpoint"].HeaderText = "API";
            dgvDetails.Columns["Endpoint"].Width = 120;

            dgvDetails.Columns["ResponseCode"].HeaderText = "Response Code";
            dgvDetails.Columns["ResponseCode"].Width = 120;

            dgvDetails.Columns["Status"].HeaderText = "Status";
            dgvDetails.Columns["Status"].Width = 100;

            dgvDetails.Columns["Duration"].HeaderText = "Duration (ms)";
            dgvDetails.Columns["Duration"].Width = 100;

            if (dgvDetails.Columns.Contains("IsPaid"))
                dgvDetails.Columns["IsPaid"].Visible = false;

            if (dgvDetails.Columns.Contains("IsCancelled"))
                dgvDetails.Columns["IsCancelled"].Visible = false;

            if (dgvDetails.Columns.Contains("TransactionStatus"))
            {
                dgvDetails.Columns["TransactionStatus"].HeaderText = "Transaction Status";
                dgvDetails.Columns["TransactionStatus"].Width = 150;
                dgvDetails.Columns["TransactionStatus"].DisplayIndex = 4;
            }

            if (dgvDetails.Columns.Contains("TransactionRef"))
            {
                dgvDetails.Columns["TransactionRef"].HeaderText = "Transaction Ref";
                dgvDetails.Columns["TransactionRef"].Width = 180;
            }

            dgvDetails.Columns["RefNo"].HeaderText = "RefNo";
            dgvDetails.Columns["RefNo"].Width = 200;

            dgvDetails.Columns["PartnerRef"].HeaderText = "PartnerRef";
            dgvDetails.Columns["PartnerRef"].Width = 200;

            // ✅ CẬP NHẬT CỘT ERROR MESSAGE
            dgvDetails.Columns["ErrorMessage"].HeaderText = "Error / DebugDesc";
            dgvDetails.Columns["ErrorMessage"].Width = 350;

            // ✅ ẨN CỘT DEBUGDESC RIÊNG (ĐÃ MERGE VÀO ERROR MESSAGE)
            if (dgvDetails.Columns.Contains("DebugDesc"))
                dgvDetails.Columns["DebugDesc"].Visible = false;

            if (dgvDetails.Columns.Contains("RequestJson"))
                dgvDetails.Columns["RequestJson"].Visible = false;
            if (dgvDetails.Columns.Contains("ResponseJson"))
                dgvDetails.Columns["ResponseJson"].Visible = false;

            if (!dgvDetails.Columns.Contains("Balance"))
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = "Balance",
                    HeaderText = "Balance",
                    Width = 260,
                    DataPropertyName = "Balance",
                    DefaultCellStyle = { Format = "N2" }
                };
                dgvDetails.Columns.Add(col);
            }
            dgvDetails.Columns["Balance"].DisplayIndex = dgvDetails.Columns.Count - 2;

            if (!dgvDetails.Columns.Contains("Currency"))
            {
                var col = new DataGridViewTextBoxColumn
                {
                    Name = "Currency",
                    HeaderText = "Currency",
                    Width = 150,
                    DataPropertyName = "Currency"
                };
                dgvDetails.Columns.Add(col);
            }
            dgvDetails.Columns["Currency"].DisplayIndex = dgvDetails.Columns.Count - 1;
        }

        // ✅ TÁCH RA HÀM RIÊNG ĐỂ TÁI SỬ DỤNG SAU KHI SORT
        private void ApplyDetailGridFormatting()
        {
            foreach (DataGridViewRow row in dgvDetails.Rows)
            {
                if (row.Cells["Status"].Value != null)
                {
                    string status = row.Cells["Status"].Value.ToString();
                    if (status == "SUCCESS")
                        row.Cells["Status"].Style.BackColor = Color.LightGreen;
                    else if (status == "FAILED")
                        row.Cells["Status"].Style.BackColor = Color.LightPink;
                }

                if (row.Cells["ResponseCode"].Value != null && row.Cells["Endpoint"].Value != null)
                {
                    string respCode = row.Cells["ResponseCode"].Value.ToString();
                    string endpointValue = row.Cells["Endpoint"].Value.ToString();
                    if (respCode == "99" && endpointValue == "TRANSFER")
                    {
                        row.Cells["ResponseCode"].Style.BackColor = Color.Red;
                        row.Cells["ResponseCode"].Style.ForeColor = Color.White;
                        row.Cells["ResponseCode"].Style.Font = new Font(dgvDetails.Font, FontStyle.Bold);
                    }
                    else
                    {
                        row.Cells["ResponseCode"].Style.BackColor = dgvDetails.DefaultCellStyle.BackColor;
                        row.Cells["ResponseCode"].Style.ForeColor = dgvDetails.DefaultCellStyle.ForeColor;
                        row.Cells["ResponseCode"].Style.Font = dgvDetails.Font;
                    }
                }

                string endpointCellValue = row.Cells["Endpoint"].Value?.ToString() ?? "";
                if ((endpointCellValue == "TRANSFER" || endpointCellValue == "TRANSINQ" ||
                     endpointCellValue == "CANCELTRANS" || endpointCellValue == "UPDATETRANS") &&
                    row.Cells["TransactionStatus"].Value != null)
                {
                    string txStatus = row.Cells["TransactionStatus"].Value?.ToString() ??"";

                    if (txStatus == "PAID")
                    {
                        row.Cells["TransactionStatus"].Value = "💰 PAID";
                        row.Cells["TransactionStatus"].Style.BackColor = Color.Gold;
                        row.Cells["TransactionStatus"].Style.ForeColor = Color.DarkGreen;
                        row.Cells["TransactionStatus"].Style.Font = new Font(dgvDetails.Font, FontStyle.Bold);
                    }
                    else if (txStatus == "CANCELLED")
                    {
                        row.Cells["TransactionStatus"].Value = "🚫 CANCELLED";
                        row.Cells["TransactionStatus"].Style.BackColor = Color.LightCoral;
                        row.Cells["TransactionStatus"].Style.ForeColor = Color.DarkRed;
                        row.Cells["TransactionStatus"].Style.Font = new Font(dgvDetails.Font, FontStyle.Bold);
                    }
                    else if (txStatus == "ERROR_99")
                    {
                        row.Cells["TransactionStatus"].Value = "❌ ERROR 99";
                        row.Cells["TransactionStatus"].Style.BackColor = Color.Red;
                        row.Cells["TransactionStatus"].Style.ForeColor = Color.White;
                        row.Cells["TransactionStatus"].Style.Font = new Font(dgvDetails.Font, FontStyle.Bold);
                    }
                    else if (txStatus == "PENDING")
                    {
                        row.Cells["TransactionStatus"].Value = "⏳ PENDING";
                        row.Cells["TransactionStatus"].Style.BackColor = Color.LightYellow;

                        // ✅ HIGHLIGHT ERROR MESSAGE CHO PENDING
                        if (row.Cells["ErrorMessage"].Value != null &&
                            !string.IsNullOrEmpty(row.Cells["ErrorMessage"].Value.ToString()))
                        {
                            row.Cells["ErrorMessage"].Style.BackColor = Color.LightYellow;
                            row.Cells["ErrorMessage"].Style.ForeColor = Color.DarkRed;
                            row.Cells["ErrorMessage"].Style.Font = new Font(dgvDetails.Font, FontStyle.Bold);
                        }
                    }
                }
                else
                {
                    if (row.Cells["TransactionStatus"] != null)
                    {
                        row.Cells["TransactionStatus"].Value = "";
                    }
                }

                string rowEndpoint = row.Cells["Endpoint"].Value?.ToString() ?? "";
                bool isQueryInfor = rowEndpoint == "QUERYINFOR";

                if (row.Cells.Contains(row.Cells["Balance"]))
                {
                    row.Cells["Balance"].ReadOnly = true;
                    row.Cells["Balance"].Style.BackColor = isQueryInfor ? Color.LightYellow : dgvDetails.DefaultCellStyle.BackColor;
                    row.Cells["Balance"].Style.ForeColor = isQueryInfor ? Color.DarkBlue : dgvDetails.DefaultCellStyle.ForeColor;
                    if (!isQueryInfor)
                    {
                        row.Cells["Balance"].Value = null;
                    }
                }

                if (row.Cells.Contains(row.Cells["Currency"]))
                {
                    row.Cells["Currency"].ReadOnly = true;
                    row.Cells["Currency"].Style.BackColor = isQueryInfor ? Color.LightYellow : dgvDetails.DefaultCellStyle.BackColor;
                    row.Cells["Currency"].Style.ForeColor = isQueryInfor ? Color.DarkBlue : dgvDetails.DefaultCellStyle.ForeColor;
                    if (!isQueryInfor)
                    {
                        row.Cells["Currency"].Value = "";
                    }
                }
            }

            foreach (DataGridViewRow row in dgvDetails.Rows)
            {
                string endpoint = row.Cells["Endpoint"].Value?.ToString() ?? "";
                bool isQueryInfor = endpoint == "QUERYINFOR";

                if (dgvDetails.Columns.Contains("RefNo"))
                    dgvDetails.Columns["RefNo"].Visible = !isQueryInfor;
                if (dgvDetails.Columns.Contains("PartnerRef"))
                    dgvDetails.Columns["PartnerRef"].Visible = !isQueryInfor;
                if (dgvDetails.Columns.Contains("TransactionStatus"))
                    dgvDetails.Columns["TransactionStatus"].Visible = !isQueryInfor;
                if (dgvDetails.Columns.Contains("TransactionRef"))
                    dgvDetails.Columns["TransactionRef"].Visible = !isQueryInfor;
                if (isQueryInfor && dgvDetails.Columns.Contains("ErrorMessage"))
                    dgvDetails.Columns["ErrorMessage"].Width = 150;

                if (dgvDetails.Columns.Contains("Balance"))
                    dgvDetails.Columns["Balance"].Visible = isQueryInfor;
                if (dgvDetails.Columns.Contains("Currency"))
                    dgvDetails.Columns["Currency"].Visible = isQueryInfor;
            }
        }
        private void dgvDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Ignore header clicks
                if (e.RowIndex < 0) return;

                // Ensure row selection
                dgvDetails.ClearSelection();
                dgvDetails.Rows[e.RowIndex].Selected = true;

                // Get log data directly from DataSource
                var logs = dgvDetails.DataSource as List<ApiRequestLog>;
                if (logs == null || e.RowIndex >= logs.Count)
                {
                    MessageBox.Show("Không thể lấy thông tin log này.",
                        "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var selectedLog = logs[e.RowIndex];
                ShowRequestResponseDetail(selectedLog);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị chi tiết: {ex.Message}\n\n" +
                    $"RowIndex: {e.RowIndex}\n" +
                    $"DataSource: {(dgvDetails.DataSource != null ? "OK" : "NULL")}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ShowRequestResponseDetail(ApiRequestLog log)
        {
            var detailForm = new Form
            {
                Text = $"📋 Request/Response Details - {log.Endpoint}",
                Width = 1200,
                Height = 800,
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.Sizable
            };

            var splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 350
            };

            string formattedRequest = FormatJson(log.RequestJson);
            string formattedResponse = FormatJson(log.ResponseJson);

            var groupRequest = new GroupBox
            {
                Text = "📤 REQUEST",
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var txtRequest = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                Text = formattedRequest,
                ReadOnly = true,
                WordWrap = false,
                BorderStyle = BorderStyle.None
            };

            HighlightJson(txtRequest);
            groupRequest.Controls.Add(txtRequest);
            splitContainer.Panel1.Controls.Add(groupRequest);

            var groupResponse = new GroupBox
            {
                Text = "📥 RESPONSE",
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            var txtResponse = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White,
                Text = formattedResponse,
                ReadOnly = true,
                WordWrap = false,
                BorderStyle = BorderStyle.None
            };

            HighlightJson(txtResponse);
            groupResponse.Controls.Add(txtResponse);
            splitContainer.Panel2.Controls.Add(groupResponse);

            var panelInfo = new Panel
            {
                Dock = DockStyle.Top,
                Height = 140,
                BackColor = Color.FromArgb(245, 245, 245),
                Padding = new Padding(15)
            };

            var lblInfo = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f),
                Text = BuildInfoText(log)
            };

            panelInfo.Controls.Add(lblInfo);

            var panelButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(10)
            };

            var btnCopyAll = new Button
            {
                Text = "📑 Copy All (Log Format)",
                Width = 180,
                Height = 40,
                Location = new Point(15, 10),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCopyAll.FlatAppearance.BorderSize = 0;
            btnCopyAll.Click += (s, e) =>
            {
                string logFormat = BuildLogFormat(log, formattedRequest, formattedResponse);
                Clipboard.SetText(logFormat);
                MessageBox.Show("✅ Đã copy log theo format chuẩn vào clipboard!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            var btnCopyRequest = new Button
            {
                Text = "📋 Copy Request",
                Width = 140,
                Height = 40,
                Location = new Point(205, 10),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCopyRequest.FlatAppearance.BorderSize = 0;
            btnCopyRequest.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(formattedRequest))
                {
                    Clipboard.SetText(formattedRequest);
                    MessageBox.Show("✅ Request đã được copy vào clipboard!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            var btnCopyResponse = new Button
            {
                Text = "📋 Copy Response",
                Width = 140,
                Height = 40,
                Location = new Point(355, 10),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCopyResponse.FlatAppearance.BorderSize = 0;
            btnCopyResponse.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(formattedResponse))
                {
                    Clipboard.SetText(formattedResponse);
                    MessageBox.Show("✅ Response đã được copy vào clipboard!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            var btnClose = new Button
            {
                Text = "❌ Close",
                Width = 120,
                Height = 40,
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            btnClose.Location = new Point(panelButtons.Width - 135, 10);
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => detailForm.Close();

            panelButtons.Controls.AddRange(new System.Windows.Forms.Control[] {
                btnCopyAll, btnCopyRequest, btnCopyResponse, btnClose
            });

            detailForm.Controls.Add(splitContainer);
            detailForm.Controls.Add(panelInfo);
            detailForm.Controls.Add(panelButtons);

            detailForm.ShowDialog();
        }
        private string BuildLogFormat(ApiRequestLog log, string requestJson, string responseJson)
        {
            var sb = new StringBuilder();
            sb.AppendLine("----------------------------------------------------");

            string statusMarker = log.TransactionStatus switch
            {
                "PAID" => "💰 PAID",
                "CANCELLED" => "🚫 CANCELLED",
                "PENDING" => "⏳ PENDING",
                "ERROR_99" => "❌ ERROR_99",
                _ => ""
            };

            sb.AppendLine($"[{log.Timestamp:yyyy-MM-dd HH:mm:ss}] - {log.Endpoint} {statusMarker}");
            sb.AppendLine($"Status: {log.Status}");
            sb.AppendLine($"Duration: {log.Duration} ms");
            sb.AppendLine($"ResponseCode: {log.ResponseCode}");

            if (!string.IsNullOrEmpty(log.PartnerRef))
                sb.AppendLine($"PartnerRef: {log.PartnerRef}");

            if (!string.IsNullOrEmpty(log.TransactionRef))
                sb.AppendLine($"TransactionRef: {log.TransactionRef}");

            if (!string.IsNullOrEmpty(log.TransactionStatus))
                sb.AppendLine($"TransactionStatus: {log.TransactionStatus}");

            if (!string.IsNullOrEmpty(log.ErrorMessage))
                sb.AppendLine($"Error: {log.ErrorMessage}");

            sb.AppendLine("REQUEST:");
            sb.AppendLine(requestJson);
            sb.AppendLine("RESPONSE:");
            sb.AppendLine(responseJson);
            sb.AppendLine("----------------------------------------------------");

            return sb.ToString();
        }
        private string BuildInfoText(ApiRequestLog log)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"⏰ Timestamp: {log.Timestamp:yyyy-MM-dd HH:mm:ss}     🚀 Endpoint: {log.Endpoint}     ⏱️ Duration: {log.Duration} ms");
            sb.AppendLine();
            sb.AppendLine($"📊 Status: {log.Status}     💬 Response Code: {log.ResponseCode}");
            sb.AppendLine();

            if (log.Endpoint == "QUERYINFOR" && log.Balance.HasValue)
            {
                sb.AppendLine($"💵 Balance: {log.Balance:N2} {log.Currency}");
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(log.TransactionStatus))
            {
                string statusIcon = log.TransactionStatus switch
                {
                    "PAID" => "💰",
                    "CANCELLED" => "🚫",
                    "ERROR_99" => "❌",
                    "PENDING" => "⏳",
                    _ => "📋"
                };
                sb.AppendLine($"{statusIcon} Transaction Status: {log.TransactionStatus}");
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(log.PartnerRef))
            {
                sb.AppendLine($"🔗 PartnerRef: {log.PartnerRef}");
            }

            if (!string.IsNullOrEmpty(log.TransactionRef))
            {
                sb.AppendLine($"🆔 TransactionRef: {log.TransactionRef}");
            }

            if (!string.IsNullOrEmpty(log.RefNo))
            {
                sb.AppendLine($"🔖 RefNo: {log.RefNo}");
            }

            if (!string.IsNullOrEmpty(log.ErrorMessage))
            {
                sb.AppendLine();
                sb.AppendLine($"❌ Error/DebugDesc: {log.ErrorMessage}");
            }

            return sb.ToString();
        }


        private string FormatJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return "No data available";

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

        private void HighlightJson(RichTextBox rtb)
        {
            if (string.IsNullOrWhiteSpace(rtb.Text))
                return;

            try
            {
                int originalIndex = rtb.SelectionStart;
                int originalLength = rtb.SelectionLength;

                rtb.SelectAll();
                rtb.SelectionColor = Color.White;

                string text = rtb.Text;

                var keyMatches = System.Text.RegularExpressions.Regex.Matches(text, @"""([^""]+)""\s*:");
                foreach (System.Text.RegularExpressions.Match match in keyMatches)
                {
                    rtb.Select(match.Index, match.Length - 1);
                    rtb.SelectionColor = Color.FromArgb(86, 156, 214);
                    rtb.SelectionFont = new Font(rtb.Font, FontStyle.Bold);
                }

                var stringMatches = System.Text.RegularExpressions.Regex.Matches(text, @":\s*""([^""]*)""");
                foreach (System.Text.RegularExpressions.Match match in stringMatches)
                {
                    int startIndex = match.Index + match.Value.IndexOf('"');
                    int length = match.Value.LastIndexOf('"') - match.Value.IndexOf('"') + 1;
                    rtb.Select(startIndex, length);
                    rtb.SelectionColor = Color.FromArgb(206, 145, 120);
                }

                var numberMatches = System.Text.RegularExpressions.Regex.Matches(text, @":\s*(\d+\.?\d*)");
                foreach (System.Text.RegularExpressions.Match match in numberMatches)
                {
                    int startIndex = match.Index + match.Value.IndexOf(match.Groups[1].Value);
                    rtb.Select(startIndex, match.Groups[1].Length);
                    rtb.SelectionColor = Color.FromArgb(181, 206, 168);
                }

                var boolMatches = System.Text.RegularExpressions.Regex.Matches(text, @"\b(true|false|null)\b");
                foreach (System.Text.RegularExpressions.Match match in boolMatches)
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.FromArgb(86, 156, 214);
                }

                foreach (char c in new[] { '{', '}', '[', ']' })
                {
                    int index = 0;
                    while ((index = text.IndexOf(c, index)) != -1)
                    {
                        rtb.Select(index, 1);
                        rtb.SelectionColor = Color.FromArgb(255, 215, 0);
                        rtb.SelectionFont = new Font(rtb.Font, FontStyle.Bold);
                        index++;
                    }
                }

                rtb.Select(originalIndex, originalLength);
                rtb.SelectionColor = Color.White;
            }
            catch
            {
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            UpdateStatistics();
            UpdateDetailGrid();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            // Reload all data from database
            await LoadLogsFromDatabase();
            MessageBox.Show("Đã refresh dữ liệu từ database!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var ws1 = workbook.Worksheets.Add("Statistics");

                    ws1.Cell(1, 1).Value = "Endpoint";
                    ws1.Cell(1, 2).Value = "Total Requests";
                    ws1.Cell(1, 3).Value = "Success";
                    ws1.Cell(1, 4).Value = "Failed";
                    ws1.Cell(1, 5).Value = "Success Rate %";
                    ws1.Cell(1, 6).Value = "Avg Duration (ms)";
                    ws1.Cell(1, 7).Value = "Min Duration (ms)";
                    ws1.Cell(1, 8).Value = "Max Duration (ms)";

                    ws1.Range(1, 1, 1, 8).Style.Font.Bold = true;
                    ws1.Range(1, 1, 1, 8).Style.Fill.BackgroundColor = XLColor.LightGray;

                    var stats = (List<ApiStatistics>)dgvStatistics.DataSource;
                    if (stats != null)
                    {
                        int row = 2;
                        foreach (var stat in stats)
                        {
                            ws1.Cell(row, 1).Value = stat.Endpoint;
                            ws1.Cell(row, 2).Value = stat.TotalRequests;
                            ws1.Cell(row, 3).Value = stat.SuccessCount;
                            ws1.Cell(row, 4).Value = stat.FailedCount;
                            ws1.Cell(row, 5).Value = stat.SuccessRate;
                            ws1.Cell(row, 6).Value = stat.AvgDuration;
                            ws1.Cell(row, 7).Value = stat.MinDuration;
                            ws1.Cell(row, 8).Value = stat.MaxDuration;
                            row++;
                        }
                    }

                    ws1.Columns().AdjustToContents();

                    var ws2 = workbook.Worksheets.Add("Details");

                    ws2.Cell(1, 1).Value = "Timestamp";
                    ws2.Cell(1, 2).Value = "Endpoint";
                    ws2.Cell(1, 3).Value = "Response Code";
                    ws2.Cell(1, 4).Value = "Status";
                    ws2.Cell(1, 5).Value = "Duration (ms)";
                    ws2.Cell(1, 6).Value = "Transaction Status";
                    ws2.Cell(1, 7).Value = "TransactionRef";
                    ws2.Cell(1, 8).Value = "RefNo";
                    ws2.Cell(1, 9).Value = "PartnerRef";
                    ws2.Cell(1, 10).Value = "Error / DebugDesc";

                    ws2.Range(1, 1, 1, 10).Style.Font.Bold = true;
                    ws2.Range(1, 1, 1, 10).Style.Fill.BackgroundColor = XLColor.LightGray;

                    int detailRow = 2;
                    var details = (List<ApiRequestLog>)dgvDetails.DataSource;
                    if (details != null)
                    {
                        foreach (var log in details)
                        {
                            ws2.Cell(detailRow, 1).Value = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                            ws2.Cell(detailRow, 2).Value = log.Endpoint;
                            ws2.Cell(detailRow, 3).Value = log.ResponseCode;
                            ws2.Cell(detailRow, 4).Value = log.Status;
                            ws2.Cell(detailRow, 5).Value = log.Duration;
                            ws2.Cell(detailRow, 6).Value = log.TransactionStatus ?? "";
                            ws2.Cell(detailRow, 7).Value = log.TransactionRef ?? "";
                            ws2.Cell(detailRow, 8).Value = log.RefNo;
                            ws2.Cell(detailRow, 9).Value = log.PartnerRef;
                            ws2.Cell(detailRow, 10).Value = log.ErrorMessage;

                            if (log.Status == "SUCCESS")
                                ws2.Cell(detailRow, 4).Style.Fill.BackgroundColor = XLColor.LightGreen;
                            else if (log.Status == "FAILED")
                                ws2.Cell(detailRow, 4).Style.Fill.BackgroundColor = XLColor.LightPink;

                            if (log.TransactionStatus == "PAID")
                                ws2.Cell(detailRow, 6).Style.Fill.BackgroundColor = XLColor.Gold;
                            else if (log.TransactionStatus == "CANCELLED")
                                ws2.Cell(detailRow, 6).Style.Fill.BackgroundColor = XLColor.LightCoral;
                            else if (log.TransactionStatus == "ERROR_99")
                                ws2.Cell(detailRow, 6).Style.Fill.BackgroundColor = XLColor.Red;
                            else if (log.TransactionStatus == "PENDING")
                                ws2.Cell(detailRow, 6).Style.Fill.BackgroundColor = XLColor.LightYellow;
                            else if (log.TransactionStatus == "Payable")
                                ws2.Cell(detailRow, 6).Style.Fill.BackgroundColor = XLColor.LightSkyBlue;

                            detailRow++;
                        }
                    }

                    ws2.Columns().AdjustToContents();

                    string fileName = $"API_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    string savePath = Path.Combine(logDirectory, fileName);
                    workbook.SaveAs(savePath);

                    MessageBox.Show($"Đã xuất báo cáo thành công!\n{savePath}",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = savePath,
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn xóa tất cả dữ liệu báo cáo?",
                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _logs.Clear();
                UpdateStatistics();
                UpdateDetailGrid();
            }
        }

        private void dgvStatistics_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var endpoint = dgvStatistics.Rows[e.RowIndex].Cells["Endpoint"].Value?.ToString();
            if (string.IsNullOrEmpty(endpoint)) return;

            // Set combobox to selected endpoint (if present in list)
            for (int i = 0; i < comboFilterEndpoint.Items.Count; i++)
            {
                if (string.Equals(comboFilterEndpoint.Items[i]?.ToString(), endpoint, StringComparison.OrdinalIgnoreCase))
                {
                    comboFilterEndpoint.SelectedIndex = i;
                    break;
                }
            }

            // Clear partner ref and refNo search boxes to focus on endpoint filter
            if (txtSearchPartnerRef != null) txtSearchPartnerRef.Clear();
            var refBox = this.Controls.Find("txtSearchRefNo", true);
            if (refBox.Length > 0 && refBox[0] is TextBox tbRef) tbRef.Clear();

            // Load details filtered by the endpoint
            UpdateDetailGrid(endpoint);

            // Optional: show count (calculate using current _logs filtered)
            var filtered = _logs.Where(l =>
                l.Endpoint == endpoint &&
                l.Timestamp >= dtpFrom.Value &&
                l.Timestamp <= dtpTo.Value)
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            MessageBox.Show($"Đã lọc {filtered.Count} requests cho endpoint: {endpoint}",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            // Reset combobox endpoint về "All"
            if (comboFilterEndpoint.Items.Count > 0)
            {
                comboFilterEndpoint.SelectedIndex = 0; // "-- All Endpoints --"
            }

            // Reset partnerRef and refNo textboxes (if exists)
            if (txtSearchPartnerRef != null)
            {
                txtSearchPartnerRef.Clear();
            }
            var refBox = this.Controls.Find("txtSearchRefNo", true);
            if (refBox.Length > 0 && refBox[0] is TextBox tbRef)
            {
                tbRef.Clear();
            }

            // Cập nhật lại grid (will pick up cleared filters)
            UpdateDetailGrid();
        }
        private void txtSearchPartnerRef_TextChanged(object sender, EventArgs e)
        {
            // Debounce already exists elsewhere; this keeps immediate filtering behavior.
            UpdateDetailGrid();
        }

        // If you add a TextBox named txtSearchRefNo on the form, wire its TextChanged to this handler:
        private void txtSearchRefNo_TextChanged(object sender, EventArgs e)
        {
            UpdateDetailGrid();
        }

        // Paging members
        // 1. Thêm các biến thành viên private cho paging:
        private int _currentPage = 1;
        private int _pageSize = 200; // tunable
        private int _totalRecords = 0;
        private int _totalPages = 0;

        // Cancellation + debounce
        private CancellationTokenSource _cts;
        private System.Windows.Forms.Timer _searchDebounceTimer;

        // 2. Tạo phương thức khởi tạo các biến paging (paging helpers)
        private void InitializePagingHelpers()
        {
            // Debounce timer for search box
            _searchDebounceTimer = new System.Windows.Forms.Timer();
            _searchDebounceTimer.Interval = 350; // ms
            _searchDebounceTimer.Tick += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _ = LoadPageAsync(1);
            };

            // Wire text changed to debounce
            txtSearchPartnerRef.TextChanged -= txtSearchPartnerRef_TextChanged;
            txtSearchPartnerRef.TextChanged += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _searchDebounceTimer.Start();
            };

            // You can wire next/prev buttons if present (example names)
            // btnNextPage.Click += async (s,e) => await LoadPageAsync(_currentPage + 1);
            // btnPrevPage.Click += async (s,e) => await LoadPageAsync(_currentPage - 1);
        }

        // 3. Tạo phương thức LoadPageAsync để tải dữ liệu cho một trang
        // Gọi phương thức này từ FormReport_Load sau khi _dbHelper được khởi tạo
        private async Task LoadPageAsync(int pageNumber)
        {
            try
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                var token = _cts.Token;

                if (pageNumber < 1) pageNumber = 1;
                _currentPage = pageNumber;

                string partnerFilter = txtSearchPartnerRef?.Text?.Trim();
                string endpointFilter = comboFilterEndpoint?.SelectedItem?.ToString();
                if (endpointFilter == "-- All Endpoints --") endpointFilter = null;

                // 1) get total count
                var countTask = _dbHelper.GetApiLogsCountAsync(
                    fromDate: dtpFrom.Value,
                    toDate: dtpTo.Value,
                    endpoint: endpointFilter,
                    partnerRef: partnerFilter
                );

                // 2) get page
                var pageTask = _dbHelper.GetApiLogsPageAsync(
                    pageNumber: _currentPage,
                    pageSize: _pageSize,
                    fromDate: dtpFrom.Value,
                    toDate: dtpTo.Value,
                    endpoint: endpointFilter,
                    partnerRef: partnerFilter,
                    sortColumn: "Timestamp",
                    sortDirection: "DESC"
                );

                await Task.WhenAll(countTask, pageTask);

                token.ThrowIfCancellationRequested();

                _totalRecords = countTask.Result;
                _totalPages = Math.Max(1, (int)Math.Ceiling(_totalRecords / (double)_pageSize));

                _logs = pageTask.Result ?? new List<ApiRequestLog>();

                // Update UI on UI thread
                dgvDetails.Invoke(() =>
                {
                    UpdateStatistics(); // optionally update stats from server or compute from current page
                    dgvDetails.DataSource = new List<ApiRequestLog>(_logs); // bind page
                    if (dgvDetails.Columns.Count > 0)
                    {
                        ConfigureDetailGridColumns();
                        ApplyDetailGridFormatting();
                    }
                });

                UpdatePagingInfo();
            }
            catch (OperationCanceledException) { /* ignore */ }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải trang dữ liệu: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePagingInfo()
        {
            // If you have a label to show paging, update it; otherwise debug
            // Example: lblPageInfo.Text = $"Page {_currentPage}/{_totalPages} ({_totalRecords} rows)";
            // If label doesn't exist, ignore silently
            try
            {
                var lbl = this.Controls.Find("lblPageInfo", true);
                if (lbl.Length > 0 && lbl[0] is Label pageLabel)
                {
                    pageLabel.Text = $"Page {_currentPage}/{_totalPages} ({_totalRecords} rows)";
                }
            }
            catch { }
        }

        // Simple helpers to go prev/next
        private async Task GoNextPageAsync()
        {
            if (_currentPage < _totalPages)
                await LoadPageAsync(_currentPage + 1);
        }
        private async Task GoPrevPageAsync()
        {
            if (_currentPage > 1)
                await LoadPageAsync(_currentPage - 1);
        }
    }
}