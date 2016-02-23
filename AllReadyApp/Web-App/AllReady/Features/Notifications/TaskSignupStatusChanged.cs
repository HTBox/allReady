using MediatR;

namespace AllReady.Features.Notifications
{
    public class TaskSignupStatusChanged : INotification
    {
        public int SignupId { get; set; }
    }
}
