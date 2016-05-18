using AllReady.Models;
using MediatR;
using System.Linq;

namespace AllReady.Areas.Admin.Features.Organizations
{

    public class OrganizationByIdQueryHandler : IRequestHandler<OrganizationByIdQuery, Organization>
    {
        private readonly AllReadyContext _context;

        public OrganizationByIdQueryHandler(AllReadyContext context)
        {
            this._context = context;
        }

        public Organization Handle(OrganizationByIdQuery message)
        {
            return _context.Organizations.FirstOrDefault(o => o.Id == message.OrganizationId);
        }
    }
}
