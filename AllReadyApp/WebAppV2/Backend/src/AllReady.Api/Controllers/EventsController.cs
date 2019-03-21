using System.Collections.Generic;
using AllReady.Api.Models.Output.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using NodaTime;

namespace AllReady.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly LinkGenerator _linkGenerator;

        public EventsController(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public ActionResult<IEnumerable<EventListerOutputModel>> Get()
        {
            // todo - call a service to load this from a data store

            var events = new List<EventListerOutputModel>
            {
                new EventListerOutputModel
                {
                    Id = 1,
                    Name = "Charity Event 1",
                    ShortDescription = "Marketing activities",
                    StartDateTime = new LocalDate(2019, 01, 01),
                    EndDateTime = new LocalDate(2019, 01, 31),
                    TimeZone = DateTimeZoneProviders.Tzdb["Europe/London"],
                    Link = _linkGenerator.GetPathByAction(
                        HttpContext,
                        "Get",
                        values: new { id = 1 })
                }
            };

            return events;
        }
    }
}
