using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class ManagingOrganizationIdByEventIdQueryHandler : IRequestHandler<ManagingOrganizationIdByEventIdQuery, int>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public ManagingOrganizationIdByEventIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public int Handle(ManagingOrganizationIdByEventIdQuery message)
        {
            return dataAccess.GetManagingOrganizationId(message.EventId);
        }
    }
}
