using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        Campaign IAllReadyDataAccess.GetCampaign(int campaignId)
        {
            return _dbContext.Campaigns
                .Include(x => x.ManagingOrganization)
                .Include(x => x.CampaignImpact)
                .Include(x => x.Events)
                .ThenInclude(a => a.Location)
                .Include(x => x.Location)
                .Include(x => x.ParticipatingOrganizations)
                .SingleOrDefault(x => x.Id == campaignId);
        }

    }
}
