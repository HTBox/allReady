using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AllReady.Api.Data;
using AllReady.Api.Models.Output.Campaigns;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Api.Features.Queries
{
    public class ActiveCampaignsListerQuery : IRequest<IEnumerable<CampaignListerOutputModel>>
    {

    }

    public class ActiveCampaignsListerQueryHandler : IRequestHandler<ActiveCampaignsListerQuery, IEnumerable<CampaignListerOutputModel>>
    {
        private readonly AllReadyDbContext _dbContext;

        public ActiveCampaignsListerQueryHandler(AllReadyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CampaignListerOutputModel>> Handle(ActiveCampaignsListerQuery request, CancellationToken cancellationToken)
        {
            var campaigns = await _dbContext.Campaigns.ToListAsync(); // todo - limit to those active based on current UTC time

            var outputCampaigns = campaigns.Select(c => new CampaignListerOutputModel
            {
                Id = c.Id.ToString(),
                Name = c.Name,
                ShortDescription = c.ShortDescription,
                StartDateTime = c.StartDateTime,
                EndDateTime = c.EndDateTime,
                ImageUrl = c.ImageUrl,
                TimeZone = c.TimeZone
            });

            return outputCampaigns;
        }
    }
}
