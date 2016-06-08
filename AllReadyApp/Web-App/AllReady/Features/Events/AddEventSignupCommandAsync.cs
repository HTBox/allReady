using AllReady.Models;
using MediatR;

namespace AllReady.Features.Event
{
    public class AddEventSignupCommandAsync : IAsyncRequest
    {
        public EventSignup EventSignup { get; set; }
    }
}
