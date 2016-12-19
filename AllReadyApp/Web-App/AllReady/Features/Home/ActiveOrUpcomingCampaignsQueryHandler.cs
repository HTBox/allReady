﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Models;
using AllReady.ViewModels.Home;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Features.Home
{
    public class ActiveOrUpcomingCampaignsQueryHandler : IAsyncRequestHandler<ActiveOrUpcomingCampaignsQuery, List<ActiveOrUpcomingCampaign>>
    {
        private readonly AllReadyContext _context;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public ActiveOrUpcomingCampaignsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<List<ActiveOrUpcomingCampaign>> Handle(ActiveOrUpcomingCampaignsQuery message)
        {
            var now = DateTimeUtcNow();

            return await _context.Campaigns
                .AsNoTracking()
                .Where(campaign => campaign.EndDateTime.UtcDateTime.Date >= now.Date && !campaign.Locked && campaign.Published)
                .Select(campaign => new ActiveOrUpcomingCampaign
                {
                    Id = campaign.Id,
                    ImageUrl = campaign.ImageUrl,
                    Name = campaign.Name,
                    Description = campaign.Description,
                    StartDate = campaign.StartDateTime,
                    EndDate = campaign.EndDateTime
                })
                .OrderBy(campaign => campaign.EndDate)
                .ToListAsync();
        }
    }
}