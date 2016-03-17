using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ActivitySignupByActivityIdAndUserIdQuery : IRequest<ActivitySignup>
    {
        public int ActivityId { get; set; }
        public string UserId { get; set; }
    }
}
