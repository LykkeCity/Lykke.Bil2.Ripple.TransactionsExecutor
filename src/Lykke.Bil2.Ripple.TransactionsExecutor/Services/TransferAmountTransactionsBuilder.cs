using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.Common.Extensions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.AccountInfo;
using Lykke.Bil2.Ripple.Client.Api.AccountLines;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Numerics;
using Newtonsoft.Json;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransferAmountTransactionsBuilder : ITransferAmountTransactionsBuilder
    {
        private readonly IRippleApi _rippleApi;

        public TransferAmountTransactionsBuilder(IRippleApi rippleApi)
        {
            _rippleApi = rippleApi;
        }

        public async Task<BuildTransactionResponse> BuildTransferAmountAsync(BuildTransferAmountTransactionRequest request)
        {
            if (request.Transfers.Count != 1)
            {
                throw new RequestValidationException("Invalid transfers count, must be exactly 1");
            }

            var transfer = request.Transfers.First();
            var tag = (uint?)null;

            if (transfer.DestinationAddressTag != null)
            {
                tag = !uint.TryParse(transfer.DestinationAddressTag, out var value)
                    ? throw new RequestValidationException("Invalid destination tag, must be positive integer")
                    : value;
            }

            object amount = transfer.AssetId == "XRP"
                ? await CheckXrpBalance(transfer)
                : await CheckIssuedCurrencyBalance(transfer);

            var tx = JsonConvert.SerializeObject
            (
                new
                {
                    Account = transfer.SourceAddress,
                    Amount = amount,
                    Destination = transfer.DestinationAddress,
                    DestinationTag = tag,
                    Fee = UMoney.Round(request.Fee.FirstOrDefault().Value, 6),
                    Flags = 0x80000000,
                    LastLedgerSequence = request.Expiration?.AfterBlockNumber,
                    Sequence = transfer.SourceAddressNonce,
                    TransactionType = "Payment",
                }
            );

            return new BuildTransactionResponse(tx.ToBase58());
        }

        private async Task<object> CheckIssuedCurrencyBalance(Transfer transfer)
        {
            var sourceAccountLinesResponse = await _rippleApi.Post(new AccountLinesRequest(transfer.SourceAddress));

            if (sourceAccountLinesResponse.Result.Error == "actNotFound")
            {
                throw new RequestValidationException("Source account not found");
            }

            sourceAccountLinesResponse.Result.ThrowIfError();

            var line = sourceAccountLinesResponse.Result.Lines
                .FirstOrDefault(x => x.Currency == transfer.Asset.AssetId && x.Account == transfer.Asset.IssuerId);

            if (line == null ||
                Money.Create(decimal.Parse(line.Balance), 6) < transfer.Amount)
            {
                throw new TransactionBuildingException(TransactionBuildingError.NotEnoughBalance, "Not enough balance");
            }

            return new
            {
                currency = transfer.Asset.AssetId,
                issuer = transfer.Asset.IssuerId,
                value = transfer.Amount.ToString()
            };
        }

        private async Task<object> CheckXrpBalance(Transfer transfer)
        {
            var sourceAccountInfoResponse = await _rippleApi.Post(new AccountInfoRequest(transfer.SourceAddress));

            if (sourceAccountInfoResponse.Result.Error == "actNotFound")
            {
                throw new RequestValidationException("Source account not found");
            }

            sourceAccountInfoResponse.Result.ThrowIfError();

            var balance = new Money(BigInteger.Parse(sourceAccountInfoResponse.Result.AccountData.Balance), 6);

            if (balance < transfer.Amount)
            {
                throw new TransactionBuildingException(TransactionBuildingError.NotEnoughBalance, "Not enough balance");
            }

            // TODO: correct amount format
            return transfer.Amount.ToString(); ;
        }
    }
}