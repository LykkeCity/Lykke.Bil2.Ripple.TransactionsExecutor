using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.AccountInfo;
using Lykke.Bil2.Ripple.Client.Api.Tx;
using Lykke.Bil2.Ripple.TransactionsExecutor.Services;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Tests
{
    public class TransactionsStateProviderTests
    {
        private Mock<IRippleApi> _rippleApi;
        private TransactionsStateProvider _transactionsStateProvider;

        [SetUp]
        public void Setup()
        {
            _rippleApi = new Mock<IRippleApi>();
            _transactionsStateProvider = new TransactionsStateProvider(_rippleApi.Object);
        }

        [TestCase(TransactionState.Unknown)]
        [TestCase(TransactionState.Mined)]
        [TestCase(TransactionState.Broadcasted)]
        public async Task ShouldReturnTransactionState(TransactionState expected)
        {
            // Arrange

            _rippleApi
                .Setup(x => x.Post(It.IsAny<TxRequest>()))
                .ReturnsAsync
                (
                    new RippleResponse<TxResult>
                    {
                        Result = new TxResult
                        {
                            Status = expected == TransactionState.Unknown ? "error" : "success",
                            Error = expected == TransactionState.Unknown ? "txnNotFound" : null,
                            Validated = expected == TransactionState.Mined
                        }
                    }
                );

            // Act

            var actual = await _transactionsStateProvider.GetStateAsync(expected.ToString());

            // Assert

            Assert.AreEqual(expected, actual);
        }
    }
}