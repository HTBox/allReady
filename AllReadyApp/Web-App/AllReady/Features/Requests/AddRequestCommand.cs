using AllReady.Models;
using AllReady.ViewModels.Requests;
using MediatR;

namespace AllReady.Features.Requests
{
    public class AddRequestCommand : IAsyncRequest<Request>
    {
        public RequestViewModel RequestViewModel { get; set; }
    }
}
