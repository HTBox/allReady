using System.Collections.Generic;
using System.Linq;
using AllReady.Models;
using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Campaigns
{
    public class CampaignGetQueryHandler : IRequestHandler<CampaignGetQuery, List<CampaignViewModel>>
    {
        private readonly IAllReadyDataAccess _dataAccess;

        public CampaignGetQueryHandler(IAllReadyDataAccess dataAccess)
        {
            this._dataAccess = dataAccess;
        }

        public List<CampaignViewModel> Handle(CampaignGetQuery message)
        {
            return _dataAccess.Campaigns.Where(c => !c.Locked).Select(x => new CampaignViewModel(x)).ToList();

            //this could be written like this:
            //return _dataAccess.Campaigns.Where(c => !c.Locked).ToViewModel().ToList();
            //and if that's the case, then it's teh same exact query that CampaignIndexQueryHandler is running
            //so, we I create one handler with one query message and send that message from different controller action methods
            //or, do I create a query where the query mesages contains the boolean locked field as a param and run the query that way?
        }
    }
}
