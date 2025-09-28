using System;

namespace MTM_WIP_Application_Avalonia.Models
{
    /// <summary>
    /// Result model for transfer operations.
    /// Contains details about the completed transfer including split information.
    /// Returned by TransferService.ExecuteTransferAsync method.
    /// </summary>
    public class TransferResult
    {
        /// <summary>
        /// Unique transaction identifier for audit trail
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// Indicates if the transfer resulted in a quantity split
        /// </summary>
        public bool WasSplit { get; set; }

        /// <summary>
        /// Original quantity before transfer
        /// </summary>
        public int OriginalQuantity { get; set; }

        /// <summary>
        /// Actual quantity transferred (may be auto-capped)
        /// </summary>
        public int TransferredQuantity { get; set; }

        /// <summary>
        /// Quantity remaining at source location (0 for full transfer)
        /// </summary>
        public int RemainingQuantity { get; set; }

        /// <summary>
        /// Result message for user feedback
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when transfer was completed
        /// </summary>
        public DateTime CompletedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Source location for the transfer
        /// </summary>
        public string FromLocation { get; set; } = string.Empty;

        /// <summary>
        /// Destination location for the transfer
        /// </summary>
        public string ToLocation { get; set; } = string.Empty;

        /// <summary>
        /// Part identifier that was transferred
        /// </summary>
        public string PartId { get; set; } = string.Empty;

        /// <summary>
        /// Operation number for the transfer
        /// </summary>
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// User who performed the transfer
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Creates a successful transfer result
        /// </summary>
        /// <param name="transactionId">Transaction identifier</param>
        /// <param name="originalQty">Original quantity</param>
        /// <param name="transferredQty">Transferred quantity</param>
        /// <param name="remainingQty">Remaining quantity</param>
        /// <param name="message">Success message</param>
        /// <returns>TransferResult instance</returns>
        public static TransferResult Success(string transactionId, int originalQty, int transferredQty, int remainingQty, string message = "")
        {
            return new TransferResult
            {
                TransactionId = transactionId,
                OriginalQuantity = originalQty,
                TransferredQuantity = transferredQty,
                RemainingQuantity = remainingQty,
                WasSplit = remainingQty > 0,
                Message = string.IsNullOrEmpty(message) ? GenerateSuccessMessage(transferredQty, remainingQty > 0) : message,
                CompletedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Creates a failed transfer result
        /// </summary>
        /// <param name="errorMessage">Error description</param>
        /// <returns>TransferResult instance</returns>
        public static TransferResult Failure(string errorMessage)
        {
            return new TransferResult
            {
                TransactionId = string.Empty,
                Message = errorMessage,
                CompletedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Generates a standard success message based on transfer details
        /// </summary>
        /// <param name="transferredQty">Quantity transferred</param>
        /// <param name="wasSplit">Whether transfer was split</param>
        /// <returns>Formatted success message</returns>
        private static string GenerateSuccessMessage(int transferredQty, bool wasSplit)
        {
            if (wasSplit)
                return $"Transfer completed successfully. {transferredQty} units transferred, remaining quantity updated at source.";
            else
                return $"Transfer completed successfully. Full quantity of {transferredQty} units transferred.";
        }

        /// <summary>
        /// Gets a detailed summary of the transfer
        /// </summary>
        /// <returns>Formatted summary string</returns>
        public string GetSummary()
        {
            var summary = $"Transaction {TransactionId}: {PartId} Op:{Operation}\n";
            summary += $"From: {FromLocation} → To: {ToLocation}\n";
            summary += $"Transferred: {TransferredQuantity} of {OriginalQuantity}";

            if (WasSplit)
                summary += $" (Remaining: {RemainingQuantity})";

            summary += $"\nCompleted: {CompletedAt:yyyy-MM-dd HH:mm:ss}";
            summary += $"\nBy: {UserId}";

            if (!string.IsNullOrEmpty(Message))
                summary += $"\nMessage: {Message}";

            return summary;
        }

        /// <summary>
        /// Gets transfer efficiency percentage
        /// </summary>
        /// <returns>Percentage of original quantity that was transferred</returns>
        public double GetTransferEfficiency()
        {
            if (OriginalQuantity == 0) return 0.0;
            return (double)TransferredQuantity / OriginalQuantity * 100.0;
        }

        /// <summary>
        /// Checks if transfer was complete (no remaining quantity)
        /// </summary>
        /// <returns>True if all quantity was transferred</returns>
        public bool IsCompleteTransfer()
        {
            return RemainingQuantity == 0;
        }

        /// <summary>
        /// String representation for logging
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            return $"TransferResult: {TransactionId} - {TransferredQuantity}/{OriginalQuantity} {(WasSplit ? "SPLIT" : "FULL")}";
        }
    }
}
