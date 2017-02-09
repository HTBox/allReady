using MediatR;

namespace AllReady.Features.Notifications
{
    public class VolunteerTaskSignupStatusChanged : IAsyncNotification
    {
        public int SignupId { get; set; }
    }
}
