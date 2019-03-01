using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.TransactionsExecutor;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ripple.TransactionsExecutor.Services
{
    public class AddressValidator : IAddressValidator
    {
        public AddressValidator(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task<AddressValidityResponse> ValidateAsync(string address, AddressTagType? tagType = null, string tag = null)
        {
            // TODO: validate address and, optionally, tag type and tag
            //
            // Throw
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if input parameters are invalid and result can't be mapped to
            //     any of Lykke.Bil2.Contract.TransactionsExecutor.AddressValidationResult value.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     Likely a temporary issue with infrastructure or configuration,
            //     request should be repeated later.
            //
            // For example, for shared deposit wallet and numeric tag approach (used in Ripple f.e.) validation may look like:
            //
            // if (tag == null)
            // {
            //     if (CheckIfTagIsRequiredForAddress(address))
            //     {
            //         return new AddressValidityResponse(AddressValidationResult.RequiredTagMissed);
            //     }
            // }
            // else if (tagType == AddressTagType.Number && !long.TryParse(tag, out var _))
            // {
            //     return new AddressValidityResponse(AddressValidationResult.InvalidTagFormat);
            // }
            //
            // var isAddressFormatValid = ...;
            //
            // return !isAddressFormatValid
            //     ? new AddressValidityResponse(AddressValidationResult.InvalidAddressFormat)
            //     : new AddressValidityResponse(AddressValidationResult.Valid);


            throw new NotImplementedException();
        }
    }
}
