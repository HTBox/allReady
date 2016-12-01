using AllReady.ViewModels.Requests;
using MediatR;

namespace AllReady.Features.Requests
{
    public class ProcessApiRequestCommand : IAsyncRequest
    {
        public RequestApiViewModel ViewModel { get; set; }
    }
}
