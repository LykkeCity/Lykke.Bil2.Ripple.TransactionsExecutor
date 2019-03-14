using Lykke.Bil2.Sdk.Services;
using Lykke.Bil2.Sdk.TransactionsExecutor.Settings;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Settings
{
    /// <summary>
    /// Specific blockchain settings
    /// </summary>
    public class AppSettings : BaseTransactionsExecutorSettings<DbSettings>
    {
        public string NodeUrl { get; set; }

        [Optional]
        public string NodeRpcUsername { get; set; }

        [Optional]
        [SecureSettings]
        public string NodeRpcPassword { get; set; }

        [Optional]
        public decimal? FeeFactor { get; set; }

        [Optional]
        public decimal? MaxFee { get; set; }
    }
}
