using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class IndexQueryHandler : IAsyncRequestHandler<IndexQuery, IEnumerable<IndexViewModel>>
    {
        private readonly AllReadyContext _context;

        public IndexQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IndexViewModel>> Handle(IndexQuery message)
        {
            var campaigns = _context.Campaigns.Include(c => c.ManagingOrganization).AsNoTracking();
            if (message.OrganizationId.HasValue)
            {
                campaigns = campaigns.Where(c => c.ManagingOrganizationId == message.OrganizationId);
            }

            var viewModel = campaigns.Select(c => new IndexViewModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                OrganizationName = c.ManagingOrganization.Name,
                StartDate = c.StartDateTime,
                EndDate = c.EndDateTime,
                Locked = c.Locked,
                Featured = c.Featured
            });

            return await viewModel.ToListAsync();
        }
    }
}