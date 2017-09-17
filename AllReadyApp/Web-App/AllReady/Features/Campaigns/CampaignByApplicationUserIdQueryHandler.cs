using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Campaigns
{
    public class CampaignByApplicationUserIdQueryHandler : IAsyncRequestHandler<CampaignByApplicationUserIdQuery, List<Campaign>>
    {
        private readonly AllReadyContext _context;

        public CampaignByApplicationUserIdQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<List<Campaign>> Handle(CampaignByApplicationUserIdQuery message)
        {
            return await _context.Campaigns
                .Include(x => x.Organizer)
                .Where(x => x.Published && x.Organizer.Id == message.ApplicationUserId)
                .ToListAsync();
        }
    }
}
