using System.Collections.Generic;
using AllReady.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class AllOrganizationsQueryHandler : IRequestHandler<AllOrganizationsQuery, IEnumerable<Organization>>
    {
        private readonly IAllReadyDataAccess dataAccess;

        public AllOrganizationsQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public IEnumerable<Organization> Handle(AllOrganizationsQuery message)
        {
            return dataAccess.Organizations;
        }
    }
}
