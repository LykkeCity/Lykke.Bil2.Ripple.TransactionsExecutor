using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Common;
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
                throw new RequestValidationException("Invalid transfers count, must be exactly 1");

            if (request.Fees.Count != 1)
                throw new RequestValidationException("Invalid fees count, must be exactly 1");

            var transfer = request.Transfers.First();
            var fee = request.Fees.First();
            var tag = (uint?)null;

            if (transfer.DestinationAddressTag != null)
            {
                tag = !uint.TryParse(transfer.DestinationAddressTag, out var value)
                    ? throw new RequestValidationException("Invalid destination tag, must be positive integer")
                    : value;
            }

            if (fee.Asset.Id != "XRP")
            {
                throw new RequestValidationException("Invalid fee asset, must be XRP");
            }

            object amount = transfer.Asset.Id == "XRP"
                ? await CheckXrpBalance(transfer)
                : await CheckIssuedCurrencyBalance(transfer);

            var tx = new Dictionary<string, object>
            {
                ["Account"] = transfer.SourceAddress,
                ["Amount"] = amount,
                ["Destination"] = transfer.DestinationAddress,
                ["Fee"] = UMoney.Denominate(fee.Amount, 6).Significand.ToString(),
                ["Flags"] = 0x80000000,
                ["TransactionType"] = "Payment"
            };

            // nullable fields must be either not null or absent at all

            if (transfer.DestinationAddressTag != null)
                tx["DestinationTag"] = tag;

            if (request.Expiration?.AfterBlockNumber != null)
                tx["LastLedgerSequence"] = request.Expiration?.AfterBlockNumber;

            if (transfer.SourceAddressNonce != null)
                tx["Sequence"] = transfer.SourceAddressNonce;

            return new BuildTransactionResponse(tx.ToJson().ToBase58());
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
                .FirstOrDefault(x => x.Currency == transfer.Asset.Id && x.Account == transfer.Asset.Address);

            if (line == null ||
                Money.Create(decimal.Parse(line.Balance)) < transfer.Amount)
            {
                throw new TransactionBuildingException(TransactionBuildingError.NotEnoughBalance, "Not enough balance");
            }

            return new
            {
                currency = transfer.Asset.Id,
                issuer = transfer.Asset.Address,
                value = transfer.Amount
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

            // XRP must be in drops, as integer string
            return UMoney.Denominate(transfer.Amount, 6).Significand.ToString();
        }
    }
}