using System.Collections.Generic;
using System.Linq;
using AllReady.Features.Activity;
using AllReady.ViewModels;
using MediatR;
using Microsoft.AspNet.Mvc;

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
        public IEnumerable<ActivityViewModel> GetCampaignsByPostalCode(string zip, int miles)
        {
            var model = new List<ActivityViewModel>();

            var campaigns = mediator.Send(new AcitivitiesByPostalCodeQuery { PostalCode = zip, Distance =  miles})
                .Select(x => x.Campaign)
                .Distinct();

            var activities = campaigns
                .SelectMany(x => x.Activities)
                .ToList();

           activities.ForEach(activity => model.Add(new ActivityViewModel(activity)));

           return model;
        }
    }
}
