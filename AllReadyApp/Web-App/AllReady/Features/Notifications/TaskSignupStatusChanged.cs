using MediatR;

namespace AllReady.Features.Notifications
{
    public class TaskSignupStatusChanged : IAsyncNotification
    {
        public int SignupId { get; set; }
    }
}
