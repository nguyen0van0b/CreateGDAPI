using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
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

        public FormReport()
        {
            InitializeComponent();
        }

        private void FormReport_Load(object sender, EventArgs e)
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

            LoadLogs();
            UpdateStatistics();
            UpdateDetailGrid();
        }
        private void DgvDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Suppress the error - đã xử lý bằng cách ẩn boolean columns
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
            _logs.Clear();

            if (!Directory.Exists(logDirectory))
            {
                MessageBox.Show("Thư mục log không tồn tại!");
                return;
            }

            var logFiles = Directory.GetFiles(logDirectory, "logs_all_apis_*.txt");

            foreach (var file in logFiles)
            {
                ParseLogFile(file);
            }

            lblTotalRequests.Text = $"📊 Tổng số requests: {_logs.Count}";
        }

        private void ParseLogFile(string filePath)
        {
            try
            {
                string content = File.ReadAllText(filePath, Encoding.UTF8);
                var logBlocks = content.Split(new[] { "----------------------------------------------------" },
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var block in logBlocks)
                {
                    if (string.IsNullOrWhiteSpace(block)) continue;

                    var log = ParseLogEntry(block);
                    if (log != null)
                    {
                        _logs.Add(log);
                    }
                }
            }
            catch (Exception ex)
            {
                // Ignore parsing errors
            }
        }

        private ApiRequestLog ParseLogEntry(string logEntry)
        {
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
                    // ✅ PARSE HEADER LINE: [timestamp] - ENDPOINT STATUS
                    if (line.Contains("[") && line.Contains("]") && line.Contains(" - "))
                    {
                        // Extract timestamp
                        var timestampMatch = System.Text.RegularExpressions.Regex.Match(
                            line, @"\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\]");
                        if (timestampMatch.Success &&
                            DateTime.TryParse(timestampMatch.Groups[1].Value, out var timestamp))
                        {
                            log.Timestamp = timestamp;
                        }

                        // Extract endpoint and status markers
                        var parts = line.Split(new[] { " - " }, StringSplitOptions.None);
                        if (parts.Length > 1)
                        {
                            string endpointPart = parts[1].Trim();

                            // Remove status markers to get clean endpoint name
                            log.Endpoint = endpointPart
                                .Replace("💰 PAID", "")
                                .Replace("🚫 CANCELLED", "")
                                .Replace("🚫 CANCELLED [UPDATED]", "")
                                .Replace("⏳ PENDING", "")
                                .Trim()
                                .ToUpper();

                            // Detect transaction status from markers
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

                    // ✅ PARSE STRUCTURED FIELDS
                    if (line.StartsWith("Status:"))
                    {
                        log.Status = line.Split(':')[1].Trim();
                    }
                    else if (line.StartsWith("Duration:"))
                    {
                        var durationStr = line.Split(':')[1].Replace("ms", "").Trim();
                        if (int.TryParse(durationStr, out var duration))
                        {
                            log.Duration = duration;
                        }
                    }
                    else if (line.StartsWith("ResponseCode:"))
                    {
                        log.ResponseCode = line.Split(':')[1].Trim();
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

                        // Update flags based on status
                        if (status == "PAID")
                            log.IsPaid = true;
                        else if (status == "CANCELLED")
                            log.IsCancelled = true;
                    }
                    else if (line.StartsWith("Error:"))
                    {
                        log.ErrorMessage = line.Substring(6).Trim();
                    }

                    // ✅ PARSE REQUEST/RESPONSE SECTIONS
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

                // ✅ SET ERROR_99 STATUS
                if (log.ResponseCode == "99")
                {
                    log.TransactionStatus = "ERROR_99";
                }

                // ✅ DETERMINE LOG STATUS if not set
                if (string.IsNullOrEmpty(log.Status))
                {
                    log.Status = DetermineLogStatus(log.Endpoint, log.ResponseCode, log.ResponseJson);
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

            // Filter by endpoint if selected
            string selectedEndpoint = comboFilterEndpoint.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedEndpoint) && selectedEndpoint != "-- All Endpoints --")
            {
                filteredLogs = filteredLogs.Where(l => l.Endpoint == selectedEndpoint).ToList();
            }

            // Group by endpoint
            var stats = filteredLogs
                .GroupBy(l => l.Endpoint ?? "UNKNOWN")
                .Select(g => new ApiStatistics
                {
                    Endpoint = g.Key,
                    TotalRequests = g.Count(),
                    SuccessCount = g.Count(l => l.Status == "SUCCESS"),
                    FailedCount = g.Count(l => l.Status == "FAILED"),
                    SuccessRate = g.Count() > 0 ? (g.Count(l => l.Status == "SUCCESS") * 100.0 / g.Count()) : 0,
                    AvgDuration = g.Where(l => l.Duration > 0).Any() ? (int)g.Where(l => l.Duration > 0).Average(l => l.Duration) : 0,
                    MinDuration = g.Where(l => l.Duration > 0).Any() ? g.Where(l => l.Duration > 0).Min(l => l.Duration) : 0,
                    MaxDuration = g.Where(l => l.Duration > 0).Any() ? g.Where(l => l.Duration > 0).Max(l => l.Duration) : 0
                })
                .OrderByDescending(s => s.TotalRequests)
                .ToList();

            // Bind to grid
            dgvStatistics.DataSource = stats;

            // Format columns
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

                // Color coding for success rate
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

            // Update summary labels
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

        private void UpdateDetailGrid()
        {
            var filteredLogs = _logs.Where(l =>
                l.Timestamp >= dtpFrom.Value &&
                l.Timestamp <= dtpTo.Value).ToList();

            string selectedEndpoint = comboFilterEndpoint.SelectedItem?.ToString();
            if (!string.IsNullOrEmpty(selectedEndpoint) && selectedEndpoint != "-- All Endpoints --")
            {
                filteredLogs = filteredLogs.Where(l => l.Endpoint == selectedEndpoint).ToList();
            }

            filteredLogs = filteredLogs.OrderByDescending(l => l.Timestamp).ToList();

            dgvDetails.DataSource = filteredLogs;

            if (dgvDetails.Columns.Count > 0)
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

                // ✅ ẨN BOOLEAN COLUMNS
                if (dgvDetails.Columns.Contains("IsPaid"))
                    dgvDetails.Columns["IsPaid"].Visible = false;

                if (dgvDetails.Columns.Contains("IsCancelled"))
                    dgvDetails.Columns["IsCancelled"].Visible = false;

                // ✅ HIỂN THỊ TransactionStatus
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

                dgvDetails.Columns["ErrorMessage"].HeaderText = "Error";
                dgvDetails.Columns["ErrorMessage"].Width = 250;

                if (dgvDetails.Columns.Contains("RequestJson"))
                    dgvDetails.Columns["RequestJson"].Visible = false;
                if (dgvDetails.Columns.Contains("ResponseJson"))
                    dgvDetails.Columns["ResponseJson"].Visible = false;

                // ✅ COLOR CODING
                foreach (DataGridViewRow row in dgvDetails.Rows)
                {
                    // Color for Status column
                    if (row.Cells["Status"].Value != null)
                    {
                        string status = row.Cells["Status"].Value.ToString();
                        if (status == "SUCCESS")
                            row.Cells["Status"].Style.BackColor = Color.LightGreen;
                        else if (status == "FAILED")
                            row.Cells["Status"].Style.BackColor = Color.LightPink;
                    }

                    // ✅ COLOR FOR RESPONSE CODE 99
                    if (row.Cells["ResponseCode"].Value != null)
                    {
                        string respCode = row.Cells["ResponseCode"].Value.ToString();
                        if (respCode == "99")
                        {
                            row.Cells["ResponseCode"].Style.BackColor = Color.Red;
                            row.Cells["ResponseCode"].Style.ForeColor = Color.White;
                            row.Cells["ResponseCode"].Style.Font = new Font(dgvDetails.Font, FontStyle.Bold);
                        }
                    }

                    // ✅ HIGHLIGHT TRANSACTION STATUS
                    string endpoint = row.Cells["Endpoint"].Value?.ToString() ?? "";
                    if ((endpoint == "TRANSFER" || endpoint == "TRANSINQ" || endpoint == "CANCELTRANS" || endpoint == "UPDATETRANS") &&
                        row.Cells["TransactionStatus"].Value != null)
                    {
                        string txStatus = row.Cells["TransactionStatus"].Value.ToString();

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
                        }
                    }
                    else
                    {
                        // ✅ CÁC API KHÁC: ẨN TRANSACTION STATUS
                        if (row.Cells["TransactionStatus"] != null)
                        {
                            row.Cells["TransactionStatus"].Value = "";
                        }
                    }
                }
            }
        }
        private void dgvDetails_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var selectedLog = dgvDetails.Rows[e.RowIndex].DataBoundItem as ApiRequestLog;
            if (selectedLog == null) return;

            // Show detail in popup form
            ShowRequestResponseDetail(selectedLog);
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

            // Top panel - Request
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

            // Bottom panel - Response
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

            // Info panel at top
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

            // Button panel
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
                // ✅ COPY THEO FORMAT LOG CHUẨN
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

            // Header with status marker
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

            // ✅ TRANSACTION STATUS với icon
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
                sb.AppendLine($"❌ Error: {log.ErrorMessage}");
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
                // Nếu không parse được, trả về raw text
                return json;
            }
        }

        private void HighlightJson(RichTextBox rtb)
        {
            if (string.IsNullOrWhiteSpace(rtb.Text))
                return;

            try
            {
                // Save current selection
                int originalIndex = rtb.SelectionStart;
                int originalLength = rtb.SelectionLength;

                // Reset color
                rtb.SelectAll();
                rtb.SelectionColor = Color.White;

                // Highlight các phần tử JSON
                string text = rtb.Text;

                // Highlight keys (trong dấu ngoạc kép trước dấu :)
                var keyMatches = System.Text.RegularExpressions.Regex.Matches(text, @"""([^""]+)""\s*:");
                foreach (System.Text.RegularExpressions.Match match in keyMatches)
                {
                    rtb.Select(match.Index, match.Length - 1);
                    rtb.SelectionColor = Color.FromArgb(86, 156, 214); // Blue
                    rtb.SelectionFont = new Font(rtb.Font, FontStyle.Bold);
                }

                // Highlight string values (trong dấu ngoạc kép sau dấu :)
                var stringMatches = System.Text.RegularExpressions.Regex.Matches(text, @":\s*""([^""]*)""");
                foreach (System.Text.RegularExpressions.Match match in stringMatches)
                {
                    int startIndex = match.Index + match.Value.IndexOf('"');
                    int length = match.Value.LastIndexOf('"') - match.Value.IndexOf('"') + 1;
                    rtb.Select(startIndex, length);
                    rtb.SelectionColor = Color.FromArgb(206, 145, 120); // Orange
                }

                // Highlight numbers
                var numberMatches = System.Text.RegularExpressions.Regex.Matches(text, @":\s*(\d+\.?\d*)");
                foreach (System.Text.RegularExpressions.Match match in numberMatches)
                {
                    int startIndex = match.Index + match.Value.IndexOf(match.Groups[1].Value);
                    rtb.Select(startIndex, match.Groups[1].Length);
                    rtb.SelectionColor = Color.FromArgb(181, 206, 168); // Light green
                }

                // Highlight booleans and null
                var boolMatches = System.Text.RegularExpressions.Regex.Matches(text, @"\b(true|false|null)\b");
                foreach (System.Text.RegularExpressions.Match match in boolMatches)
                {
                    rtb.Select(match.Index, match.Length);
                    rtb.SelectionColor = Color.FromArgb(86, 156, 214); // Blue
                }

                // Highlight braces and brackets
                foreach (char c in new[] { '{', '}', '[', ']' })
                {
                    int index = 0;
                    while ((index = text.IndexOf(c, index)) != -1)
                    {
                        rtb.Select(index, 1);
                        rtb.SelectionColor = Color.FromArgb(255, 215, 0); // Gold
                        rtb.SelectionFont = new Font(rtb.Font, FontStyle.Bold);
                        index++;
                    }
                }

                // Restore selection
                rtb.Select(originalIndex, originalLength);
                rtb.SelectionColor = Color.White;
            }
            catch
            {
                // If highlighting fails, keep the text as is
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            UpdateStatistics();
            UpdateDetailGrid();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogs();
            UpdateStatistics();
            UpdateDetailGrid();
            MessageBox.Show("Đã refresh dữ liệu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    // Sheet 1: Statistics
                    var ws1 = workbook.Worksheets.Add("Statistics");

                    // Add headers
                    ws1.Cell(1, 1).Value = "Endpoint";
                    ws1.Cell(1, 2).Value = "Total Requests";
                    ws1.Cell(1, 3).Value = "Success";
                    ws1.Cell(1, 4).Value = "Failed";
                    ws1.Cell(1, 5).Value = "Success Rate %";
                    ws1.Cell(1, 6).Value = "Avg Duration (ms)";
                    ws1.Cell(1, 7).Value = "Min Duration (ms)";
                    ws1.Cell(1, 8).Value = "Max Duration (ms)";

                    // Format headers
                    ws1.Range(1, 1, 1, 8).Style.Font.Bold = true;
                    ws1.Range(1, 1, 1, 8).Style.Fill.BackgroundColor = XLColor.LightGray;

                    // Add data from dgvStatistics
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

                    // Sheet 2: Details
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
                    ws2.Cell(1, 10).Value = "Error Message";

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

                            // Color coding for Status
                            if (log.Status == "SUCCESS")
                                ws2.Cell(detailRow, 4).Style.Fill.BackgroundColor = XLColor.LightGreen;
                            else if (log.Status == "FAILED")
                                ws2.Cell(detailRow, 4).Style.Fill.BackgroundColor = XLColor.LightPink;

                            // Color coding for Transaction Status
                            if (log.TransactionStatus == "PAID")
                                ws2.Cell(detailRow, 6).Style.Fill.BackgroundColor = XLColor.Gold;
                            else if (log.TransactionStatus == "CANCELLED")
                                ws2.Cell(detailRow, 6).Style.Fill.BackgroundColor = XLColor.LightCoral;
                            else if (log.TransactionStatus == "ERROR_99")
                                ws2.Cell(detailRow, 6).Style.Fill.BackgroundColor = XLColor.Red;
                            else if (log.TransactionStatus == "PENDING")
                                ws2.Cell(detailRow, 6).Style.Fill.BackgroundColor = XLColor.LightYellow;

                            detailRow++;
                        }
                    }

                    ws2.Columns().AdjustToContents();

                    // Save
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
                ;
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

            // Filter details by endpoint
            var filtered = _logs.Where(l =>
                l.Endpoint == endpoint &&
                l.Timestamp >= dtpFrom.Value &&
                l.Timestamp <= dtpTo.Value)
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            dgvDetails.DataSource = filtered;

            MessageBox.Show($"Đã lọc {filtered.Count} requests cho endpoint: {endpoint}",
                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            UpdateDetailGrid();
        }

        // ✅ SỬA HÀM XÁC ĐỊNH STATUS
        private string DetermineLogStatus(string endpoint, string responseCode, string responseJson)
        {
            // ✅ SPECIAL CASE: QUERYINFOR với status=400 nhưng có response hợp lệ
            if (endpoint == "QUERYINFOR")
            {
                try
                {
                    using var doc = JsonDocument.Parse(responseJson);

                    // Kiểm tra có field "response" là array và có data không
                    if (doc.RootElement.TryGetProperty("response", out var responseArray)
                        && responseArray.ValueKind == JsonValueKind.Array
                        && responseArray.GetArrayLength() > 0)
                    {
                        // Có response data hợp lệ -> SUCCESS
                        var firstItem = responseArray[0];
                        if (firstItem.TryGetProperty("balance", out var balance))
                        {
                            return "SUCCESS";  // ✅ Có balance -> thành công
                        }
                    }
                }
                catch { }
            }

            // Logic cũ cho các endpoint khác
            if (responseCode == "00")
                return "SUCCESS";
            else if (!string.IsNullOrEmpty(responseCode))
                return "FAILED";
            else
                return "UNKNOWN";
        }
    }
}