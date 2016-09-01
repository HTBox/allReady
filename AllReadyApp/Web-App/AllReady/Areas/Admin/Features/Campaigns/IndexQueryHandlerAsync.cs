using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class IndexQueryHandlerAsync : IAsyncRequestHandler<IndexQueryAsync, IndexViewModel>
    {
        private readonly AllReadyContext _context;

        public IndexQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IndexViewModel> Handle(IndexQueryAsync message)
        {
            var campaignsQuery = _context.Campaigns.Include(c => c.ManagingOrganization).AsNoTracking();
            if (message.OrganizationId.HasValue)
            {
                campaignsQuery = campaignsQuery.Where(c => c.ManagingOrganizationId == message.OrganizationId);
            }

            var viewModel = campaignsQuery.Select(c => new IndexViewModel
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

            return viewModel;
        }
    }
}
