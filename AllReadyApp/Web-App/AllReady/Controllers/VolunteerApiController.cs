using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AllReady.Features.Events;
using AllReady.ViewModels.Event;
using MediatR;
using AllReady.Features.Volunteers;

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
            var t = new List<VolunteerProjection>();
            var viewModel = await _mediator.SendAsync(new GetVolunteerEventsQuery());

            foreach (var model in viewModel.PastEvents)
            {
                t.Add(new VolunteerProjection { WorkflowState="Past", Name = model.EventName, Location = string.Format("{0:g}", model.StartDate) + " to " + string.Format("{0:g}", model.EndDate) + "(" + model.TimeZone + ")", NumberOfTasks = model.Tasks.Count, NumberOfVolunteers = model.VolunteerCount });
            }
            foreach (var model in viewModel.CurrentEvents)
            {
                t.Add(new VolunteerProjection { WorkflowState = "Current", Name = model.EventName, Location = string.Format("{0:g}", model.StartDate) + " to " + string.Format("{0:g}", model.EndDate) + "(" + model.TimeZone + ")", NumberOfTasks = model.Tasks.Count, NumberOfVolunteers = model.VolunteerCount });
            }
            foreach (var model in viewModel.FutureEvents)
            {
                t.Add(new VolunteerProjection { WorkflowState = "Future", Name = model.EventName, Location = string.Format("{0:g}", model.StartDate) + " to " + string.Format("{0:g}", model.EndDate) + "(" + model.TimeZone + ")", NumberOfTasks = model.Tasks.Count, NumberOfVolunteers = model.VolunteerCount });
            }
            return Json(t);
        }

        public class VolunteerProjection
        {
            public string WorkflowState { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }

            public int NumberOfTasks { get; set; }

            public int NumberOfVolunteers { get; set; }
        }

    }
}
