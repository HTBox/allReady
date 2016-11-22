using AllReady.ViewModels.Requests;
using MediatR;

namespace AllReady.Features.Requests
{
    public class AddApiRequestCommand : IAsyncRequest
    {
        public RequestApiViewModel ViewModel { get; set; }
    }
}
