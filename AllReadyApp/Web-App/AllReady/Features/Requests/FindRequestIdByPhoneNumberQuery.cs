using MediatR;
using System;

namespace AllReady.Features.Requests
{
    /// <summary>
    /// Returns an Id for the latest request with a matching contact phone number
    /// </summary>
    public class FindRequestIdByPhoneNumberQuery : IAsyncRequest<Guid>
    {
        public FindRequestIdByPhoneNumberQuery(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            { 
                throw new ArgumentException(nameof(phoneNumber));
            }

            PhoneNumber = phoneNumber;
        }

        public string PhoneNumber { get; private set; }
    }
}
