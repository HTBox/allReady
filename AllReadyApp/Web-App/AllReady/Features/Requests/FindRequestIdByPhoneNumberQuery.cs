using MediatR;
using System;

namespace AllReady.Features.Requests
{
    /// <summary>
    /// Returns 
    /// </summary>
    public class FindRequestIdByPhoneNumberQuery : IAsyncRequest<Guid>
    {
        public FindRequestIdByPhoneNumberQuery(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            { 
                throw new ArgumentOutOfRangeException(nameof(phoneNumber));
            }

            PhoneNumber = phoneNumber;
        }

        public string PhoneNumber { get; private set; }
    }
}
