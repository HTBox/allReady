using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class PublishViewModelQueryHandler : IAsyncRequestHandler<PublishViewModelQuery, PublishViewModel>
    {
        private readonly AllReadyContext context;

        public PublishViewModelQueryHandler(AllReadyContext context)
        {
            this.context = context;
        }

        public async Task<PublishViewModel> Handle(PublishViewModelQuery message)
        {
            return await context.Campaigns.AsNoTracking()
                .Include(c => c.ManagingOrganization)
                .Select(c => new PublishViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    OrganizationId = c.ManagingOrganization.Id,
                    OrganizationName = c.ManagingOrganization.Name
                })
                .SingleOrDefaultAsync(c => c.Id == message.CampaignId);
        }
    }
}