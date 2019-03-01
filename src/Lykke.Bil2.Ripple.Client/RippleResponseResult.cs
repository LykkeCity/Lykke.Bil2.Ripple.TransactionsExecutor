namespace Lykke.Bil2.Ripple.TransactionsExecutor.RippleClient
{
    public class RippleResponseResult
    {
        /// <summary>
        /// The value "success" indicates the request was successfully received and understood by the server,
        /// "error" if the request caused an error.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// A unique code for the type of error that occurred.
        /// </summary>
        public string Error { get; set; }
    }
}