using System.Collections.Generic;
using AllReady.Api.Models.Output.Campaigns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NodaTime;

namespace AllReady.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignsController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;

        public CampaignsController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CampaignListerOutputModel>> Get()
        {
            // todo - call a service to load this from a data store

            var campaigns = new List<CampaignListerOutputModel>
            {
                new CampaignListerOutputModel
                {
                    Id = 1,
                    Name = "Charity Campaign 1",
                    ShortDescription = "This is a charity campaign!",
                    StartDateTime = new LocalDate(2019, 01, 01),
                    EndDateTime = new LocalDate(2019, 06, 30),
                    TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                    Link = _linkGenerator.GetPathByAction(
                        HttpContext,
                        "Get",
                        values: new { id = 1 })
                },
                new CampaignListerOutputModel
                {
                    Id = 2,
                    Name = "Charity Campaign 2",
                    ShortDescription = "This is another charity campaign!",
                    StartDateTime = new LocalDate(2019, 04, 01),
                    EndDateTime = new LocalDate(2019, 05, 31),
                    TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                    Link = _linkGenerator.GetPathByAction(
                        HttpContext,
                        "Get",
                        values: new { id = 2 })
                }
            };

            return campaigns;
        }

        [HttpGet, Route("{id}")]
        public ActionResult<CampaignOutputModel> Get(int id)
        {
            // todo - call a service to load this from a data store using the ID

            var campaign = new CampaignOutputModel
            {
                Id = 1,
                Name = "Charity Campaign 1",
                ShortDescription = "This is a charity campaign!",
                FullDesription = "This is even longer, rambling description of the campaign, which most users will not bother to read!",
                StartDateTime = new LocalDate(2019, 01, 01),
                EndDateTime = new LocalDate(2019, 06, 30),
                TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"]
            };

            return campaign;
        }
    }
}
