using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ActivitySignupCommand : IRequest
    {
        public ActivitySignupViewModel ActivitySignup { get; set; }
    }
}
