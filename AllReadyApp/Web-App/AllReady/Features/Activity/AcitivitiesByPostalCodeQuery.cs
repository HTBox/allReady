using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Activity
{
    public class AcitivitiesByPostalCodeQuery : IRequest<List<Models.Activity>>
    {
        public string PostalCode { get; set; }
        public int Distance { get; set; }
    }
}
