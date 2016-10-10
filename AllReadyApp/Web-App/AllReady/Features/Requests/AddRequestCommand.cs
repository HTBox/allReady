using AllReady.Models;
using MediatR;

namespace AllReady.Features.Requests
{
    public class AddRequestCommand : IAsyncRequest
    {
        public Request Request { get; set; }
    }
}
