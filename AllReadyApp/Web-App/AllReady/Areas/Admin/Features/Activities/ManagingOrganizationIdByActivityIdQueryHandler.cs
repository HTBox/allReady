using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class ManagingOrganizationIdByActivityIdQueryHandler : IRequestHandler<ManagingOrganizationIdByActivityIdQuery, int>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public ManagingOrganizationIdByActivityIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public int Handle(ManagingOrganizationIdByActivityIdQuery message)
        {
            return dataAccess.GetManagingOrganizationId(message.ActivityId);
        }
    }
}
