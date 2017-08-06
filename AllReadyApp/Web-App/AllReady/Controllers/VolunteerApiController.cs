using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Events;
using AllReady.ViewModels.Event;
using MediatR;

namespace AllReady.Controllers
{
    [Route("api/volunteer")]
    [Produces("application/json")]
    public class VolunteerApiController : Controller
    {
        private readonly IMediator _mediator;

        public VolunteerApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Produces("application/json", Type = typeof(VolunteerProjection))]
        public async Task<IActionResult> GetVolunteerEvents()
        {
            var t = new List<VolunteerProjection>()
            {
                new VolunteerProjection { Name="Test Event", Location="Test Location", NumberOfTasks=1, NumberOfVolunteers=10},

            };                    
            return Json(t);
        }

        public class VolunteerProjection
        {
            public string Name { get; set; }
            public string Location { get; set; }

            public int NumberOfTasks { get; set; }

            public int NumberOfVolunteers { get; set; }
        }

    }
}
