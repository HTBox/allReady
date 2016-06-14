using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using AllReady.ViewModels.Organization;
using MediatR;

namespace AllReady.Features.Organizations
{
    public class OrganizationsQueryHandler : IRequestHandler<OrganizationsQuery, List<OrganizationViewModel>>
    {
        private readonly AllReadyContext _context;

        public OrganizationsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public List<OrganizationViewModel> Handle(OrganizationsQuery message)
        {
            return _context.Organizations.ToList().Select(t => new OrganizationViewModel(t)).ToList();
        }
    }
}
