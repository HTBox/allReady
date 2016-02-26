using System;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.Data.Entity;

namespace AllReady.Features.Organizations
{
    public class OrganizationDetailsQueryHandlerAsync : IAsyncRequestHandler<OrganizationDetailsQueryAsync, OrganizationViewModel>
    {
        private readonly AllReadyContext _context;

        public OrganizationDetailsQueryHandlerAsync(AllReadyContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        public async Task<OrganizationViewModel> Handle(OrganizationDetailsQueryAsync message)
        {
            var result = await _context.Organizations.AsNoTracking()
                .Include(t => t.Campaigns)
                .SingleOrDefaultAsync(t => t.Id == message.Id);

            if (result == null)
                return null;

            result.Campaigns.RemoveAll(c => c.Locked);

            return new OrganizationViewModel(result);
        }
    }
}