using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public EventApiController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<EventViewModel> Get()
        {
            return _mediator.Send(new EventsWithUnlockedCampaignsQuery());
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
        public ActionResult GetEventsByDateRange(DateTimeOffset start, DateTimeOffset end)
        {
            var events = _mediator.Send(new EventByDateRangeQuery { EndDate = end, StartDate = start });
            if (events == null)
            {
                return NoContent();
            }
            return Json(events);
        }

        [Route("search")]
        public IEnumerable<EventViewModel> GetEventsByPostalCode(string zip, int miles)
        {
            var model = new List<EventViewModel>();

            var campaignEvents = _mediator.Send(new EventsByPostalCodeQuery { PostalCode = zip, Distance = miles });
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

        [HttpGet("{id}/qrcode")]
        public ActionResult GetQrCode(int id)
        {
            var barcodeWriter = new ZXing.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = { Width = 200, Height = 200 }
            };

            // The QR code should point users to /api/event/{id}/checkin
            var path = Request.Path.ToString();
            var checkinPath = path.Substring(0, path.Length - "qrcode".Length) + "checkin";

            var bitmap = barcodeWriter.Write($"https://{Request.Host}/{checkinPath}");
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return File(stream.ToArray(), "image/png");
            }
        }

        [HttpGet("{id}/checkin")]
        public async Task<ActionResult> GetCheckin(int id)
        {
            var campaignEvent = await GetEventBy(id);
            if (campaignEvent == null)
            {
                return NotFound();
            }

            return View("NoUserCheckin", campaignEvent);
        }

        private async Task<Event> GetEventBy(int eventId) => await _mediator.SendAsync(new EventByEventIdQueryAsync { EventId = eventId });
    }
}