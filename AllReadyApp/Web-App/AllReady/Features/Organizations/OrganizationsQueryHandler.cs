using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Organizations
{
    public class OrganizationsQueryHandler : IRequestHandler<OrganizationsQuery, List<OrganizationViewModel>>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public OrganizationsQueryHandler(IAllReadyDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public List<OrganizationViewModel> Handle(OrganizationsQuery message)
        {
            return _dataAccess.Organizations.Select(t => new OrganizationViewModel(t)).ToList();
        }
    }
}
