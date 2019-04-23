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
using Lykke.Common.Log;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Tests
{
    public class IntegrationInfoServiceTests
    {
        private Mock<IRippleApi> _rippleApi;
        private Mock<HttpMessageHandler> _httpMessageHandler;
        private DependenciesInfoProvider _dependenciesInfoProvider;

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

            _dependenciesInfoProvider = new DependenciesInfoProvider(_rippleApi.Object, httpClientFactory.Object);
        }

        [Test]
        public async Task ShouldThrow_IfGitHubIsUnavailable()
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

            // Assert

            Assert.That(() => _dependenciesInfoProvider.GetInfoAsync().Wait(), Throws.Exception);
        }

        [Test]
        public async Task ShouldThrow_IfGitHubReturnsBrokenJson()
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

            // Assert

            Assert.That(() => _dependenciesInfoProvider.GetInfoAsync().Wait(), Throws.Exception);
        }
    }
}