using System.Collections.Generic;
using System.Linq;
using AllReady.Features.Events;
using AllReady.ViewModels.Event;
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
        public IEnumerable<EventViewModel> GetCampaignsByPostalCode(string postalCode, int miles)
        {
            var model = new List<EventViewModel>();

            var campaigns = mediator.Send(new EventsByPostalCodeQuery { PostalCode = postalCode, Distance =  miles})
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
