using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Organization;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Organizations
{
    public class OrganizationDetailsQueryHandler : IAsyncRequestHandler<OrganizationDetailsQuery, OrganizationViewModel>
    {
        private readonly AllReadyContext _context;

        public OrganizationDetailsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<OrganizationViewModel> Handle(OrganizationDetailsQuery message)
        {
            var result = await _context.Organizations.AsNoTracking()
                .Include(t => t.Campaigns)
                .Include(t => t.Location)
                .SingleOrDefaultAsync(t => t.Id == message.Id);

            if (result == null)
                return null;

            result.Campaigns.RemoveAll(c => c.Locked);

            return new OrganizationViewModel(result);
        }
    }
}