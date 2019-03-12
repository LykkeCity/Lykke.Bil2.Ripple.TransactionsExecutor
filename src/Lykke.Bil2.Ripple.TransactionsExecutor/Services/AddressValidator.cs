using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Ripple.Client;
using Lykke.Bil2.Ripple.Client.Api.AccountInfo;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Ripple.Address;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class AddressValidator : IAddressValidator
    {
        private readonly IRippleApi _rippleApi;

        public AddressValidator(IRippleApi rippleApi)
        {
            _rippleApi = rippleApi;
        }

        public async Task<AddressValidityResponse> ValidateAsync(
            string address,
            AddressTagType? tagType = null,
            string tag = null)
        {
            // address must not be empty
            if (string.IsNullOrEmpty(address))
            {
                throw new RequestValidationException("Invalid address, must be non empty", nameof(address));
            }

            // address must be a valid Ripple address
            if (!AddressCodec.IsValidAddress(address))
            {
                return new AddressValidityResponse(AddressValidationResult.InvalidAddressFormat);
            }

            // if specified, tag must be unsigned integer string
            var tagInvalid = !string.IsNullOrEmpty(tag) && !uint.TryParse(tag, out var _);

            if (tagType != null &&
                tagType != AddressTagType.Number ||
                tagInvalid)
            {
                return new AddressValidityResponse(AddressValidationResult.InvalidTagFormat);
            }

            // account must exist
            var accountInfoResponse = await _rippleApi.Post(new AccountInfoRequest(address));

            if (accountInfoResponse.Result.Error == "actNotFound")
            {
                return new AddressValidityResponse(AddressValidationResult.AddressNotFound);
            }

            // tag may be required
            if (accountInfoResponse.Result.AccountData.IsDestinationTagRequired &&
                string.IsNullOrEmpty(tag))
            {
                return new AddressValidityResponse(AddressValidationResult.RequiredTagMissed);
            }

            return new AddressValidityResponse(AddressValidationResult.Valid);
        }
    }
}
