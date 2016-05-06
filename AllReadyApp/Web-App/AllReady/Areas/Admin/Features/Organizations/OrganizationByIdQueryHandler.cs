using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationByIdQueryHandler : IRequestHandler<OrganizationByIdQuery, Organization>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public OrganizationByIdQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public Organization Handle(OrganizationByIdQuery message)
        {
            return dataAccess.GetOrganization(message.OrganizationId);
        }
    }
}
