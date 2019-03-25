using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.ServerState;
using Lykke.Bil2.Ripple.TransactionsExecutor.Services;
using Lykke.Bil2.Sdk.Exceptions;
using Lykke.Common.Log;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Tests
{
    public class IntegrationInfoServiceTests
    {
        private Mock<IRippleApi> _rippleApi;
        private Mock<HttpMessageHandler> _httpMessageHandler;
        private IntegrationInfoService _integrationInfoService;

        [SetUp]
        public void Setup()
        {
            _rippleApi = new Mock<IRippleApi>();
            _httpMessageHandler = new Mock<HttpMessageHandler>();

            var httpClientFactory = new Mock<IHttpClientFactory>();

            httpClientFactory
                .Setup(x => x.CreateClient(Startup.RippleGitHubRepository))
                .Returns(new HttpClient(_httpMessageHandler.Object)
                {
                    BaseAddress = new Uri("http://test.xyz")
                });

            var logFactory = new Mock<ILogFactory>();
            var log = new Mock<ILog>();

            logFactory
                .Setup(x => x.CreateLog(It.IsAny<object>()))
                .Returns(log.Object);

            _integrationInfoService = new IntegrationInfoService(_rippleApi.Object, httpClientFactory.Object, logFactory.Object);
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

            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync
                (
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{ \"tag_name\": \"2.0.0\" }")
                    }
                );

            // Act

            // Assert

            Assert.That(() => _integrationInfoService.GetInfoAsync().Wait(), Throws.Exception);
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

            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync
                (
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{ \"tag_name\": \"2.0.0\" }")
                    }
                );

            // Act

            // Assert

            Assert.That(() => _integrationInfoService.GetInfoAsync().Wait(), Throws.Exception);
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

            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync
                (
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{ \"tag_name\": \"2.0.0\" }")
                    }
                );

            // Act

            var info = await _integrationInfoService.GetInfoAsync();

            // Assert

            Assert.AreEqual(60, info.Blockchain.LatestBlockNumber);
            Assert.AreEqual(RippleDateTime.FromRippleEpoch(60), info.Blockchain.LatestBlockMoment);
            Assert.IsNotEmpty(info.Dependencies);
            Assert.IsTrue(info.Dependencies.Any(x =>
                x.Key == "node" && x.Value.RunningVersion == new Version(1, 0, 0) && x.Value.LatestAvailableVersion == new Version(2, 0, 0)
            ));
        }

        [Test]
        public async Task ShouldReturnNullLatestVersion_IfGitHubIsUnavailable()
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

            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.InternalServerError));

            // Act

            var info = await _integrationInfoService.GetInfoAsync();

            // Assert

            Assert.IsTrue(info.Dependencies.Any(x => x.Key == "node" && x.Value.LatestAvailableVersion == null));
        }

        [Test]
        public async Task ShouldReturnNullLatestVersion_IfGitHubReturnsBrokenJson()
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

            _httpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync
                (
                    new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("qwertyuiop1234567890")
                    }
                );

            // Act

            var info = await _integrationInfoService.GetInfoAsync();

            // Assert

            Assert.IsTrue(info.Dependencies.Any(x => x.Key == "node" && x.Value.LatestAvailableVersion == null));
        }
    }
}