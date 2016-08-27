using AllReady.Models;
using MediatR;

namespace AllReady.Features.Events
{
    public class AddEventSignupCommandAsync : IAsyncRequest
    {
        public EventSignup EventSignup { get; set; }
    }
}
