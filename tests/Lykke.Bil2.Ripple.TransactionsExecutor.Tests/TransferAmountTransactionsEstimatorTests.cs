using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.ServerState;
using Lykke.Bil2.Ripple.TransactionsExecutor.Services;
using Lykke.Numerics;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Tests
{
    public class TransferAmountTransactionsEstimatorTests
    {
        private Mock<IRippleApi> _rippleApi;
        private TransferAmountTransactionsEstimator _transferAmountTransactionsEstimator;

        [SetUp]
        public void Setup()
        {
            _rippleApi = new Mock<IRippleApi>();
            _rippleApi
                .Setup(x => x.Post(It.IsAny<ServerStateRequest>()))
                .ReturnsAsync
                (
                    new RippleResponse<ServerStateResult>
                    {
                        Result = new ServerStateResult
                        {
                            State = new State
                            {
                                LoadBase = 256,
                                LoadFactor = 256,
                                ValidatedLedger = new Ledger
                                {
                                    BaseFee = 10,
                                }
                            },
                            Status = "success"
                        }
                    }
                );
        }

        [Test]
        public async Task ShouldEstimateTransaction()
        {
            // Arrange

            _transferAmountTransactionsEstimator = new TransferAmountTransactionsEstimator(_rippleApi.Object);

            // Act

            var response = await _transferAmountTransactionsEstimator.EstimateTransferAmountAsync(null);

            // Assert

            Assert.AreEqual(UMoney.Create(0.000012M), response.EstimatedFee["XRP"]);
        }

        [Test]
        public async Task ShouldEstimateTransaction_WithFactor()
        {
            // Arrange

            _transferAmountTransactionsEstimator = new TransferAmountTransactionsEstimator(_rippleApi.Object, feeFactor: 1.5M);

            // Act

            var response = await _transferAmountTransactionsEstimator.EstimateTransferAmountAsync(null);

            // Assert

            Assert.AreEqual(UMoney.Create(0.000015M), response.EstimatedFee["XRP"]);
        }

        [Test]
        public async Task ShouldEstimateTransaction_WithMax()
        {
            // Arrange

            _transferAmountTransactionsEstimator = new TransferAmountTransactionsEstimator(_rippleApi.Object, maxFee: 0.000005M);

            // Act

            var response = await _transferAmountTransactionsEstimator.EstimateTransferAmountAsync(null);

            // Assert

            Assert.AreEqual(UMoney.Create(0.000005M), response.EstimatedFee["XRP"]);
        }
    }
}