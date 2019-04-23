using System.Threading.Tasks;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.ServerState;
using Lykke.Bil2.Ripple.TransactionsExecutor.Services;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Tests
{
    public class BlockchainInfoProviderTests
    {
        private Mock<IRippleApi> _rippleApi;
        private BlockchainInfoProvider _blockchainInfoProvider;

        [SetUp]
        public void Setup()
        {
            _rippleApi = new Mock<IRippleApi>();
            _blockchainInfoProvider = new BlockchainInfoProvider(_rippleApi.Object);
        }

        [Test]
        public void ShouldThrow_IfNodeReturnsNullValidatedLedger()
        {
            // Arrange

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
                                BuildVersion = "1.0.0"
                            },
                            Status = "success"
                        }
                    }
                );

            // Act

            // Assert

            Assert.That(() => _blockchainInfoProvider.GetInfoAsync().Wait(), Throws.Exception);
        }

        [Test]
        public void ShouldThrow_IfNodeReturnsError()
        {
            // Arrange

            _rippleApi
                .Setup(x => x.Post(It.IsAny<ServerStateRequest>()))
                .ReturnsAsync
                (
                    new RippleResponse<ServerStateResult>
                    {
                        Result = new ServerStateResult
                        {
                            Error = "qwertyuiop",
                            Status = "error",
                            Request = new
                            {
                                method = "server_state"
                            }
                        }
                    }
                );

            // Act

            // Assert

            Assert.That(() => _blockchainInfoProvider.GetInfoAsync().Wait(), Throws.Exception);
        }

        [Test]
        public async Task ShouldReturnBlockchainInfo()
        {
            // Arrange

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
                                BuildVersion = "1.0.0",
                                ValidatedLedger = new LedgerState
                                {
                                    CloseTime = 60,
                                    Seq = 60,
                                    Hash = "qwertyuiop"
                                }
                            },
                            Status = "success"
                        }
                    }
                );

            // Act

            var info = await _blockchainInfoProvider.GetInfoAsync();

            // Assert

            Assert.AreEqual(60, info.LatestBlockNumber);
            Assert.AreEqual(RippleDateTime.FromRippleEpoch(60), info.LatestBlockMoment);
        }
    }
}