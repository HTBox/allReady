using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class AddActivitySignupCommandAsync : IAsyncRequest
    {
        public ActivitySignup ActivitySignup { get; set; }
    }
}
