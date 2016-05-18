using System.Collections.Generic;
using AllReady.Models;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Organizations
{

    public class AllOrganizationsQueryHandler : IRequestHandler<AllOrganizationsQuery, IEnumerable<Organization>>
    {
        private readonly AllReadyContext _context;

        public AllOrganizationsQueryHandler(AllReadyContext context)
        {
            this._context = context;
        }

        public IEnumerable<Organization> Handle(AllOrganizationsQuery message)
        {
            return _context.Organizations.ToList();
        }
    }
}
