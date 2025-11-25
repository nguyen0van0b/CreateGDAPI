using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient; // <-- Replace System.Data.SqlClient with Microsoft.Data.SqlClient

namespace CreateGDAPI
{
    public partial class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Test database connection
        /// </summary>
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Insert API request log to database
        /// </summary>
        public async Task<long> InsertApiRequestLogAsync(ApiRequestLog log)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sp_InsertApiRequestLog", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@Timestamp", log.Timestamp);
                cmd.Parameters.AddWithValue("@Endpoint", log.Endpoint ?? "");
                cmd.Parameters.AddWithValue("@ResponseCode", (object)log.ResponseCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Status", log.Status ?? "");
                cmd.Parameters.AddWithValue("@Duration", log.Duration);
                cmd.Parameters.AddWithValue("@RefNo", (object)log.RefNo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PartnerRef", (object)log.PartnerRef ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TransactionRef", (object)log.TransactionRef ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsPaid", log.IsPaid);
                cmd.Parameters.AddWithValue("@IsCancelled", log.IsCancelled);
                cmd.Parameters.AddWithValue("@TransactionStatus", (object)log.TransactionStatus ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ErrorMessage", (object)log.ErrorMessage ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@RequestJson", (object)log.RequestJson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ResponseJson", (object)log.ResponseJson ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Balance", (object)log.Balance ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Currency", (object)log.Currency ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DebugDesc", (object)log.DebugDesc ?? DBNull.Value);

                var result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inserting API log: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Insert or update transaction info
        /// </summary>
        public async Task<bool> UpsertTransactionInfoAsync(TransactionInfo trans)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"
                    MERGE TransactionInfos AS target
                    USING (SELECT @PartnerRef AS PartnerRef) AS source
                    ON target.PartnerRef = source.PartnerRef
                    WHEN MATCHED THEN
                        UPDATE SET 
                            TransactionRef = ISNULL(@TransactionRef, target.TransactionRef),
                            IsPaid = @IsPaid,
                            IsCancelled = @IsCancelled,
                            ResponseCode = @ResponseCode,
                            UpdatedAt = GETDATE()
                    WHEN NOT MATCHED THEN
                        INSERT (RefNo, PartnerRef, PartnerCode, TransactionRef, IsPaid, IsCancelled, ResponseCode, CreatedAt, UpdatedAt)
                        VALUES (@RefNo, @PartnerRef, @PartnerCode, @TransactionRef, @IsPaid, @IsCancelled, @ResponseCode, @CreatedAt, GETDATE());";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@RefNo", trans.RefNo);
                cmd.Parameters.AddWithValue("@PartnerRef", trans.PartnerRef);
                cmd.Parameters.AddWithValue("@PartnerCode", trans.PartnerCode);
                cmd.Parameters.AddWithValue("@TransactionRef", (object)trans.TransactionRef ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsPaid", trans.IsPaid);
                cmd.Parameters.AddWithValue("@IsCancelled", trans.IsCancelled);
                cmd.Parameters.AddWithValue("@ResponseCode", trans.ResponseCode);
                cmd.Parameters.AddWithValue("@CreatedAt", trans.CreatedAt);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error upserting transaction: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get API logs with filters
        /// </summary>
        public async Task<List<ApiRequestLog>> GetApiLogsAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string endpoint = null,
            string partnerRef = null,
            int maxRecords = 10000)
        {
            var logs = new List<ApiRequestLog>();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"
                    SELECT TOP (@MaxRecords) *
                    FROM ApiRequestLogs
                    WHERE (@FromDate IS NULL OR Timestamp >= @FromDate)
                      AND (@ToDate IS NULL OR Timestamp <= @ToDate)
                      AND (@Endpoint IS NULL OR Endpoint = @Endpoint)
                      AND (@PartnerRef IS NULL OR PartnerRef LIKE '%' + @PartnerRef + '%')
                    ORDER BY Timestamp DESC";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@MaxRecords", maxRecords);
                cmd.Parameters.AddWithValue("@FromDate", (object)fromDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ToDate", (object)toDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Endpoint", (object)endpoint ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@PartnerRef", (object)partnerRef ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    logs.Add(MapReaderToLog(reader));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting API logs: {ex.Message}");
            }

            return logs;
        }

        /// <summary>
        /// Get transaction infos
        /// </summary>
        public async Task<List<TransactionInfo>> GetTransactionInfosAsync(
            string partnerCode = null,
            bool? isPaid = null,
            bool? isCancelled = null)
        {
            var transactions = new List<TransactionInfo>();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"
                    SELECT *
                    FROM TransactionInfos
                    WHERE (@PartnerCode IS NULL OR PartnerCode = @PartnerCode)
                      AND (@IsPaid IS NULL OR IsPaid = @IsPaid)
                      AND (@IsCancelled IS NULL OR IsCancelled = @IsCancelled)
                    ORDER BY CreatedAt DESC";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@PartnerCode", (object)partnerCode ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsPaid", (object)isPaid ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsCancelled", (object)isCancelled ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    transactions.Add(new TransactionInfo
                    {
                        RefNo = reader["RefNo"].ToString(),
                        PartnerRef = reader["PartnerRef"].ToString(),
                        PartnerCode = reader["PartnerCode"].ToString(),
                        TransactionRef = reader["TransactionRef"] as string,
                        IsPaid = Convert.ToBoolean(reader["IsPaid"]),
                        IsCancelled = Convert.ToBoolean(reader["IsCancelled"]),
                        ResponseCode = reader["ResponseCode"].ToString(),
                        CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting transactions: {ex.Message}");
            }

            return transactions;
        }

        /// <summary>
        /// Update transaction status
        /// </summary>
        public async Task<bool> UpdateTransactionStatusAsync(
            string partnerRef,
            bool? isPaid = null,
            bool? isCancelled = null,
            string transactionRef = null)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                using var cmd = new SqlCommand("sp_UpdateTransactionStatus", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@PartnerRef", partnerRef);
                cmd.Parameters.AddWithValue("@IsPaid", (object)isPaid ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@IsCancelled", (object)isCancelled ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TransactionRef", (object)transactionRef ?? DBNull.Value);

                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error updating transaction status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get statistics
        /// </summary>
        public async Task<List<ApiStatistics>> GetStatisticsAsync(
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string endpoint = null)
        {
            var stats = new List<ApiStatistics>();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"
                    SELECT 
                        Endpoint,
                        COUNT(*) AS TotalRequests,
                        SUM(CASE WHEN Status = 'SUCCESS' THEN 1 ELSE 0 END) AS SuccessCount,
                        SUM(CASE WHEN Status = 'FAILED' THEN 1 ELSE 0 END) AS FailedCount,
                        CAST(SUM(CASE WHEN Status = 'SUCCESS' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) AS SuccessRate,
                        AVG(Duration) AS AvgDuration,
                        MIN(Duration) AS MinDuration,
                        MAX(Duration) AS MaxDuration
                    FROM ApiRequestLogs
                    WHERE (@FromDate IS NULL OR Timestamp >= @FromDate)
                      AND (@ToDate IS NULL OR Timestamp <= @ToDate)
                      AND (@Endpoint IS NULL OR Endpoint = @Endpoint)
                    GROUP BY Endpoint
                    ORDER BY TotalRequests DESC";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@FromDate", (object)fromDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ToDate", (object)toDate ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Endpoint", (object)endpoint ?? DBNull.Value);

                using var reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    stats.Add(new ApiStatistics
                    {
                        Endpoint = reader["Endpoint"].ToString(),
                        TotalRequests = Convert.ToInt32(reader["TotalRequests"]),
                        SuccessCount = Convert.ToInt32(reader["SuccessCount"]),
                        FailedCount = Convert.ToInt32(reader["FailedCount"]),
                        SuccessRate = Convert.ToDouble(reader["SuccessRate"]),
                        AvgDuration = Convert.ToInt32(reader["AvgDuration"]),
                        MinDuration = Convert.ToInt32(reader["MinDuration"]),
                        MaxDuration = Convert.ToInt32(reader["MaxDuration"])
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error getting statistics: {ex.Message}");
            }

            return stats;
        }

        /// <summary>
        /// Clear old logs (older than X days)
        /// </summary>
        public async Task<int> ClearOldLogsAsync(int daysToKeep = 30)
        {
            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"
                    DELETE FROM ApiRequestLogs
                    WHERE Timestamp < DATEADD(DAY, -@DaysToKeep, GETDATE())";

                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@DaysToKeep", daysToKeep);

                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error clearing old logs: {ex.Message}");
                return 0;
            }
        }

        private ApiRequestLog MapReaderToLog(SqlDataReader reader)
        {
            return new ApiRequestLog
            {
                Timestamp = Convert.ToDateTime(reader["Timestamp"]),
                Endpoint = reader["Endpoint"].ToString(),
                ResponseCode = reader["ResponseCode"] as string,
                Status = reader["Status"].ToString(),
                Duration = Convert.ToInt32(reader["Duration"]),
                RefNo = reader["RefNo"] as string,
                PartnerRef = reader["PartnerRef"] as string,
                TransactionRef = reader["TransactionRef"] as string,
                IsPaid = Convert.ToBoolean(reader["IsPaid"]),
                IsCancelled = Convert.ToBoolean(reader["IsCancelled"]),
                TransactionStatus = reader["TransactionStatus"] as string,
                ErrorMessage = reader["ErrorMessage"] as string,
                RequestJson = reader["RequestJson"] as string,
                ResponseJson = reader["ResponseJson"] as string,
                Balance = reader["Balance"] != DBNull.Value ? Convert.ToDecimal(reader["Balance"]) : (decimal?)null,
                Currency = reader["Currency"] as string,
                DebugDesc = reader["DebugDesc"] as string
            };
        }
    }

    // Helper class for TransactionInfo (nếu chưa có trong SharedModels.cs)
    public class TransactionInfo
    {
        public string RefNo { get; set; } = string.Empty;
        public string PartnerRef { get; set; } = string.Empty;
        public string PartnerCode { get; set; } = string.Empty;
        public string? TransactionRef { get; set; }
        public bool IsPaid { get; set; }
        public bool IsCancelled { get; set; }
        public string ResponseCode { get; set; } = "00";
        public DateTime CreatedAt { get; set; }

        public TransactionInfo()
        {
            CreatedAt = DateTime.Now;
            IsPaid = false;
            IsCancelled = false;
        }
    }
}