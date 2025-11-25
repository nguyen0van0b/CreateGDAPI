using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace CreateGDAPI
{
    public partial class DatabaseHelper
    {
        /// <summary>
        /// Update ApiRequestLogs rows that match partnerRef: set TransactionStatus and IsCancelled.
        /// Returns true if any rows were updated.
        /// </summary>
        public async Task<bool> UpdateApiRequestLogStatusAsync(string partnerRef, string transactionStatus, bool isCancelled)
        {
            if (string.IsNullOrWhiteSpace(partnerRef))
                return false;

            try
            {
                using var conn = new SqlConnection(_connectionString);
                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
UPDATE ApiRequestLogs
SET TransactionStatus = @transactionStatus,
    IsCancelled = @isCancelled
WHERE PartnerRef = @partnerRef;
";
                cmd.Parameters.AddWithValue("@transactionStatus", (object)transactionStatus ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@isCancelled", isCancelled);
                cmd.Parameters.AddWithValue("@partnerRef", partnerRef);

                await conn.OpenAsync();
                int rows = await cmd.ExecuteNonQueryAsync();
                return rows > 0;
            }
            catch (Exception)
            {
                // swallow here; caller will log if needed
                return false;
            }
        }
    }
}