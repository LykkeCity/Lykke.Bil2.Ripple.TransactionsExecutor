using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Newtonsoft.Json;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransactionExecutor : ITransactionExecutor
    {
        public TransactionExecutor(/* TODO: Provide specific settings, if necessary */)
        {
        }

        public async Task BroadcastAsync(BroadcastTransactionRequest request)
        {
            // TODO: broadcast transaction
            //
            // Throw:
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if a transaction can’t be broadcasted with the given parameters.
            //     It should be guaranteed that the transaction is not included and will not be
            //     included to the any blockchain block.
            //
            // - Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions.TransactionBroadcastingException:
            //     if a transaction cannot be broadcasted and the reason can be
            //     mapped to the Lykke.Bil2.Contract.TransactionsExecutor.TransactionBroadcastingError.
            //     It should be guaranteed that the transaction is not included and will not be
            //     included to the any blockchain block.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     It can be thrown without guarantee that transaction was broadcasted to the
            //     blockchain or not. Request will be not retried automatically.
            //
            // For example
            //
            // SignedTransaction signed;
            //
            // try
            // {
            //     signed = JsonConvert.DeserializeObject<SignedTransaction>(request.SignedTransaction.DecodeToString());
            // }
            // catch (JsonException ex)
            // {
            //     throw new RequestValidationException("Failed to deserialize signed transaction", request.SignedTransaction, ex, nameof(request.SignedTransaction));
            // }
            //
            // var result = await RippleApi.Send(signed);
            //
            // switch (result)
            // {
            //     case 100:
            //         throw new TransactionBroadcastingException(TransactionBroadcastingError.RebuildRequired, "Transaction expired");
            //
            //     case 200:
            //         throw new TransactionBroadcastingException(TransactionBroadcastingError.RetryLater, "Node is busy");
            // }


            throw new System.NotImplementedException();
        }

        public async Task<BuildReceivingTransactionResponse> BuildReceivingAsync(BuildReceivingTransactionRequest request)
        {
            // Most of blockchains does not require receiving transactions, so it is not supported by default.
            // Replace next line with receiving transaction building code, if necessary.
            //
            // Throw:
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if a transaction can’t be built with the given parameters.
            //     The given “sending” transaction can’t be received and it will be never possible to
            //     receive the given “sending” transaction.
            //
            // - Lykke.Bil2.Sdk.Exceptions.OperationNotSupportedException:
            //     if receiving transactions are not supported by the blockchain.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     Likely a temporary issue with infrastructure or configuration,
            //     request should be repeated later.


            throw new OperationNotSupportedException("Receiving transactions are not supported");
        }

        public async Task<BuildSendingTransactionResponse> BuildSendingAsync(BuildSendingTransactionRequest request)
        {
            // TODO: prepare data required for signing, return as base58 encoded string
            //
            // Throw:
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if a transaction can’t be built with the given parameters and
            //     it will be never possible to build the transaction with exactly the same parameters.
            //
            // - Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions.SendingTransactionBuildingException:
            //     if a transaction cannot be built and the reason can be mapped
            //     to the Lykke.Bil2.Contract.TransactionsExecutor.SendingTransactionBuildingError
            //
            // - System.Exception:
            //     if there are any other errors.
            //     Likely a temporary issue with infrastructure or configuration,
            //     request should be repeated later.


            throw new System.NotImplementedException();
        }
    }
}