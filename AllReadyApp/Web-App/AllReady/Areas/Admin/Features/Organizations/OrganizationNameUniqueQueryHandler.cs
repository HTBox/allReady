using AllReady.Models;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Organizations
{
    public class OrganizationNameUniqueQueryHandler : IRequestHandler<OrganizationNameUniqueQuery, bool>
    {
        private AllReadyContext _context;

        public OrganizationNameUniqueQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        public bool Handle(OrganizationNameUniqueQuery message)
        {            
            int existingOrgCount = _context.Organizations.Where(o => o.Name == message.OrganizationName && o.Id != message.OrganizationId).ToList().Count();
            if (existingOrgCount > 0)
                return false;
            else
                return true;           
        }
    }
}
