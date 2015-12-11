﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        IEnumerable<Campaign> IAllReadyDataAccess.Campaigns
        {
            get
            {
                return _dbContext.Campaigns
                   .Include(x => x.ManagingOrganization)
                   .Include(x => x.Activities)
                   .Include(x => x.ParticipatingOrganizations)
                   .ToList();
            }
        }

        Campaign IAllReadyDataAccess.GetCampaign(int campaignId)
        {
            return _dbContext.Campaigns
                .Include(x => x.ManagingOrganization)
                .Include(x => x.CampaignImpact)
                .Include(x => x.Activities)
                .Include(x => x.ParticipatingOrganizations)
                .SingleOrDefault(x => x.Id == campaignId);
        }

    }
}
