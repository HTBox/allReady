using AllReady.Models;
using MediatR;

namespace AllReady.Features.Activity
{
    public class ActivityByActivityIdQueryHandler : IRequestHandler<ActivityByActivityIdQuery, Models.Activity>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public ActivityByActivityIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Models.Activity Handle(ActivityByActivityIdQuery message)
        {
            return dataAccess.GetActivity(message.ActivityId);
        }
    }
}
