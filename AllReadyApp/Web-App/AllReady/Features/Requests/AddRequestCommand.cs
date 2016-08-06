using System.Collections.Generic;
using AllReady.Models;
using MediatR;

namespace AllReady.Features.Requests
{
    public class AddRequestCommand : IRequest<AddRequestError>
    {
        public Request Request { get; set; }
    }
}
