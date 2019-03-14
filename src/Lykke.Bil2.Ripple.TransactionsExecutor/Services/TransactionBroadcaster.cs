using System.Threading.Tasks;
using Common.Log;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.Submit;
using Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Common.Log;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class TransactionBroadcaster : ITransactionBroadcaster
    {
        private readonly IRippleApi _rippleApi;
        private readonly ILog _log;

        public TransactionBroadcaster(IRippleApi rippleApi, ILogFactory logFactory)
        {
            _rippleApi = rippleApi;
            _log = logFactory.CreateLog(this);
        }

        public async Task BroadcastAsync(BroadcastTransactionRequest request)
        {
            var submitResponse = await _rippleApi.Post(new SubmitRequest(request.SignedTransaction.DecodeToString()));

            submitResponse.Result.ThrowIfError();

            if (submitResponse.Result.EngineResult != "tesSUCCESS")
            {
                _log.Warning("Submit failed", context: submitResponse.Result);
            }

            // most of broadcasting result states are not final and even valid transaction may be not applied due to various reasons,
            // so we delegate recognizing transaction state to tracking job and return OK at the moment;
            // for details see:
            // - https://developers.ripple.com/finality-of-results.html
            // - https://developers.ripple.com/reliable-transaction-submission.html

            if (submitResponse.Result.EngineResult == "tefMAX_LEDGER" ||
                submitResponse.Result.EngineResult == "tefPAST_SEQ")
            {
                throw new TransactionBroadcastingException(TransactionBroadcastingError.RebuildRequired, submitResponse.Result.EngineResultMessage);
            }
            else if (submitResponse.Result.EngineResult.StartsWith("tem"))
            {
                throw new RequestValidationException(submitResponse.Result.EngineResultMessage);
            }
        }
    }
}