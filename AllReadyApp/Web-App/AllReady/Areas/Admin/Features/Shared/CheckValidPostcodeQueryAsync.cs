using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Shared
{
    public class CheckValidPostcodeQueryAsync : IAsyncRequest<bool>
    {
        public PostalCodeGeo Postcode { get; set; }
    }
}