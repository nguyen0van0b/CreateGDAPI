using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace CreateGDAPI
{
    public partial class DatabaseHelper
    {
        public async Task<int> GetApiLogsCountAsync(DateTime? fromDate = null, DateTime? toDate = null, string endpoint = null, string partnerRef = null)
        {
            var sql = @"SELECT COUNT(1) FROM ApiRequestLogs WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (fromDate.HasValue)
            {
                sql += " AND Timestamp >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate.Value });
            }
            if (toDate.HasValue)
            {
                sql += " AND Timestamp <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = toDate.Value });
            }
            if (!string.IsNullOrEmpty(endpoint))
            {
                sql += " AND UPPER(Endpoint) = @Endpoint";
                parameters.Add(new SqlParameter("@Endpoint", SqlDbType.NVarChar, 100) { Value = endpoint.ToUpper() });
            }
            if (!string.IsNullOrEmpty(partnerRef))
            {
                sql += " AND PartnerRef LIKE @PartnerRef";
                parameters.Add(new SqlParameter("@PartnerRef", SqlDbType.NVarChar, 200) { Value = $"%{partnerRef}%" });
            }

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters.ToArray());
            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<List<ApiRequestLog>> GetApiLogsPageAsync(int pageNumber = 1, int pageSize = 100, DateTime? fromDate = null, DateTime? toDate = null, string endpoint = null, string partnerRef = null, string sortColumn = "Timestamp", string sortDirection = "DESC")
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 100;

            var allowedCols = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Timestamp", "Endpoint", "Duration", "Status", "ResponseCode", "PartnerRef" };
            if (!allowedCols.Contains(sortColumn)) sortColumn = "Timestamp";
            sortDirection = sortDirection.Equals("ASC", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";

            var sql = $@"
SELECT Timestamp, Endpoint, ResponseCode, Status, Duration, RefNo, PartnerRef, TransactionRef, TransactionStatus, ErrorMessage, RequestJson, ResponseJson, Balance, Currency, DebugDesc
FROM ApiRequestLogs
WHERE 1=1
";
            var parameters = new List<SqlParameter>();

            if (fromDate.HasValue)
            {
                sql += " AND Timestamp >= @FromDate";
                parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime) { Value = fromDate.Value });
            }
            if (toDate.HasValue)
            {
                sql += " AND Timestamp <= @ToDate";
                parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime) { Value = toDate.Value });
            }
            if (!string.IsNullOrEmpty(endpoint))
            {
                sql += " AND UPPER(Endpoint) = @Endpoint";
                parameters.Add(new SqlParameter("@Endpoint", SqlDbType.NVarChar, 100) { Value = endpoint.ToUpper() });
            }
            if (!string.IsNullOrEmpty(partnerRef))
            {
                sql += " AND PartnerRef LIKE @PartnerRef";
                parameters.Add(new SqlParameter("@PartnerRef", SqlDbType.NVarChar, 200) { Value = $"%{partnerRef}%" });
            }

            sql += $" ORDER BY {sortColumn} {sortDirection} OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
            parameters.Add(new SqlParameter("@Offset", SqlDbType.Int) { Value = (pageNumber - 1) * pageSize });
            parameters.Add(new SqlParameter("@Limit", SqlDbType.Int) { Value = pageSize });

            var results = new List<ApiRequestLog>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddRange(parameters.ToArray());
            await conn.OpenAsync();
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                results.Add(MapReaderToLog(reader));
            }
            return results;
        }
    }
}