using AllReady.Models;
using Microsoft.AspNet.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Services
{
    public interface ISelectCollectionService
    {
        IEnumerable<CampaignImpactType> GetCampaignImpactTypes();
    }
}
