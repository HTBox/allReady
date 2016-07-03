using System.Collections.Generic;
using System.Linq;
using AllReady.Features.Event;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Controllers
{
    [Route("api/campaign")]
    [Produces("application/json")]
    public class CampaignApiController : Controller
    {
        private readonly IMediator mediator;

        public CampaignApiController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [Route("search")]
        public IEnumerable<EventViewModel> GetCampaignsByPostalCode(string zip, int miles)
        {
            var model = new List<EventViewModel>();

            var campaigns = mediator.Send(new EventsByPostalCodeQuery { PostalCode = zip, Distance =  miles})
                .Select(x => x.Campaign)
                .Distinct();

            var campaignEvents = campaigns
                .SelectMany(x => x.Events)
                .ToList();

           campaignEvents.ForEach(campaignEvent => model.Add(new EventViewModel(campaignEvent)));

           return model;
        }
    }
}
