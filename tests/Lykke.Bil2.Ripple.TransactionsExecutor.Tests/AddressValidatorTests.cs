using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.AccountInfo;
using Lykke.Bil2.Ripple.TransactionsExecutor.Services;
using Moq;
using NUnit.Framework;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Tests
{
    public class AddressValidatorTests
    {
        private Mock<IRippleApi> _rippleApi;
        private AddressValidator _addressValidator;

        [SetUp]
        public void Setup()
        {
            _rippleApi = new Mock<IRippleApi>();
            _addressValidator = new AddressValidator(_rippleApi.Object);
        }

        [TestCase(null)]
        [TestCase("")]
        public void ShouldThrowIfAddressIsNullOrEmpty(string address)
        {
            Assert.ThrowsAsync<RequestValidationException>(() => _addressValidator.ValidateAsync(address));
        }

        [TestCase("rfe8yiZUymRPx35BEwGjhfkaLmgNsTytxT", null, null, AddressValidationResult.AddressNotFound)]
        [TestCase("r9otZt3oCDL2UPioEiGduu5g5zXkqaPZt9", null, null, AddressValidationResult.InvalidAddressFormat)]
        [TestCase("qwertyuiop", null, null, AddressValidationResult.InvalidAddressFormat)]
        [TestCase("   ", null, null, AddressValidationResult.InvalidAddressFormat)]
        [TestCase("rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy", AddressTagType.Text, "1234567890", AddressValidationResult.InvalidTagFormat)]
        [TestCase("rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy", AddressTagType.Number, "   ", AddressValidationResult.InvalidTagFormat)]
        [TestCase("rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy", AddressTagType.Number, "-12", AddressValidationResult.InvalidTagFormat)]
        [TestCase("rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy", AddressTagType.Number, "qwe123", AddressValidationResult.InvalidTagFormat)]
        [TestCase("rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy", AddressTagType.Number, null, AddressValidationResult.RequiredTagMissed)]
        [TestCase("rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy", AddressTagType.Number, "", AddressValidationResult.RequiredTagMissed)]
        [TestCase("rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy", AddressTagType.Number, "1234567890", AddressValidationResult.Valid)]
        [TestCase("rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy", AddressTagType.Number, "0", AddressValidationResult.Valid)]
        public async Task ShouldReturnAddress(string address, AddressTagType? tagType, string tag, AddressValidationResult result)
        {
            // Arrange

            _rippleApi
                .Setup(x => x.Post(It.Is<AccountInfoRequest>(r => r.Params.First().Account == "rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy")))
                .ReturnsAsync
                (
                    new RippleResponse<AccountInfoResult>
                    {
                        Result = new AccountInfoResult
                        {
                            AccountData = new AccountData
                            {
                                Account = "rE6jo1LZNZeD3iexQ6DnfCREEWZ9aUweVy",
                                Balance = "0",
                                Flags = 0x00020000,
                                Sequence = 0
                            },
                            Status = "success"
                        }
                    }
                );

            _rippleApi
                .Setup(x => x.Post(It.Is<AccountInfoRequest>(r => r.Params.First().Account == "rfe8yiZUymRPx35BEwGjhfkaLmgNsTytxT")))
                .ReturnsAsync
                (
                    new RippleResponse<AccountInfoResult>
                    {
                        Result = new AccountInfoResult
                        {
                            Error = "actNotFound",
                            Status = "error"
                        }
                    }
                );

            // Act

            var response = await _addressValidator.ValidateAsync(address, tagType, tag);

            // Assert

            Assert.AreEqual(result, response.Result);
        }
    }
}