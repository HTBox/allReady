using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.UnlinkedRequests;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.UnlinkedRequests
{
    public class UnlinkedRequestListQueryHandler : IAsyncRequestHandler<UnlinkedRequestListQuery, List<UnlinkedRequestViewModel>>
    {
        private readonly AllReadyContext _context;
        public UnlinkedRequestListQueryHandler(AllReadyContext context)
        {
            _context = context;
        }
        public async Task<List<UnlinkedRequestViewModel>> Handle(UnlinkedRequestListQuery message)
        {
            return await 
                _context.Requests
                .AsNoTracking()
                .Where(r => r.OrganizationId == message.OrganizationId && !r.EventId.HasValue)
                .Select(r => new UnlinkedRequestViewModel
                {
                    Name = r.Name,
                    Address = r.Address,
                    City = r.City,
                    DateAdded = r.DateAdded,
                    Zip = r.Zip
                })
                .ToListAsync();
        }
    }
}