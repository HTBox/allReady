using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ActivitySignupCommand : IAsyncRequest
    {
        public ActivitySignupViewModel ActivitySignup { get; set; }
    }
}
