using System.Collections.Generic;
using MediatR;

namespace AllReady.Features.Events
{
    public class EventsByPostalCodeQuery : IRequest<List<Models.Event>>
    {
        public string PostalCode { get; set; }
        public int Distance { get; set; }
    }
}
