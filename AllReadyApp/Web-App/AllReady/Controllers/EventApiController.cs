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
    [Route("api/event")]
    [Produces("application/json")]
    public class EventApiController : Controller
    {
        private readonly IMediator _mediator;

        public EventApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/values
        [HttpGet]
        public async Task<IEnumerable<EventViewModel>> Get()
        {
            return await _mediator.SendAsync(new EventsWithUnlockedCampaignsQuery());
        }

        //orginial code
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(EventViewModel))]
        public async Task<EventViewModel> Get(int id)
        {
            var campaignEvent = await GetEventBy(id);
            if (campaignEvent != null)
            {
                return new EventViewModel(campaignEvent);
            }

            // BUG Change this to IActionResult and use NotFound instead of null
            NotFound();
            return null;
        }

        //orginial code
        [HttpGet("{start}/{end}")]
        [Produces("application/json", Type = typeof(EventViewModel))]
        public async Task<IActionResult> GetEventsByDateRange(DateTimeOffset start, DateTimeOffset end)
        {
            var events = await _mediator.SendAsync(new EventByDateRangeQuery { EndDate = end, StartDate = start });
            if (events == null)
            {
                return NoContent();
            }
            return Json(events);
        }

        [Route("search")]
        public IEnumerable<EventViewModel> GetEventsByPostalCode(string postalCode, int miles)
        {
            var model = new List<EventViewModel>();

            var campaignEvents = _mediator.Send(new EventsByPostalCodeQuery { PostalCode = postalCode, Distance = miles });
            campaignEvents.ForEach(campaignEvent => model.Add(new EventViewModel(campaignEvent)));

            return model;
        }

        [Route("searchbylocation")]
        public IEnumerable<EventViewModel> GetEventsByGeography(double latitude, double longitude, int miles)
        {
            var model = new List<EventViewModel>();

            var campaignEvents = _mediator.Send(new EventsByGeographyQuery { Latitude = latitude, Longitude = longitude, Miles = miles });
            campaignEvents.ForEach(campaignEvent => model.Add(new EventViewModel(campaignEvent)));

            return model;
        }

        [HttpGet("{id}/checkin")]
        public async Task<ActionResult> GetCheckin(int id)
        {
            var @event = await GetEventBy(id);
            if (@event == null)
            {
                return NotFound();
            }

            return View("NoUserCheckin", @event);
        }

        private async Task<Event> GetEventBy(int eventId) => await _mediator.SendAsync(new EventByEventIdQuery { EventId = eventId });
    }
}