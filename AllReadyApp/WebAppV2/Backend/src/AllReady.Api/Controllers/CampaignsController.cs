using System.Collections.Generic;
using AllReady.Api.Models.Output.Campaigns;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace AllReady.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<CampaignListerViewModel>> Get()
        {
            // todo - call a service to load this from a data store

            var campaigns = new List<CampaignListerViewModel>
            {
                new CampaignListerViewModel
                {
                    Id = 1,
                    Name = "Charity Campaign 1",
                    ShortDescription = "This is a charity campaign!",
                    StartDateTime = new LocalDate(2019, 01, 01),
                    EndDateTime = new LocalDate(2019, 06, 30),
                    TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                    Hidden = false
                },
                new CampaignListerViewModel
                {
                    Id = 1,
                    Name = "Charity Campaign 2",
                    ShortDescription = "This is another charity campaign!",
                    StartDateTime = new LocalDate(2019, 04, 01),
                    EndDateTime = new LocalDate(2019, 05, 31),
                    TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                    Hidden = false
                }
            };

            return campaigns;
        }
    }
}
