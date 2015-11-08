using AllReady.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc.Rendering;

namespace AllReady.Services
{
    /// <summary>
    /// Service which can be injected into views in cases where a serialized
    /// JSON collection is desired for select lists bound with javascript.
    /// </summary>
    public class SelectCollectionService : ISelectCollectionService
    {
        private AllReadyContext _context;

        public SelectCollectionService(AllReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<CampaignImpactType> GetCampaignImpactTypes()
        {
            return _context.CampaignImpactTypes.ToList();
        }
    }
}
