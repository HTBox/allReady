using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AllReady.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Features.Event;
using AllReady.ViewModels.Event;
using AllReady.ViewModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AllReady.Controllers
{
    [Route("api/event")]
    [Produces("application/json")]
    public class EventApiController : Controller
    {
        private readonly IMediator _mediator;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;
        private UserManager<ApplicationUser> _userManager;

        public EventApiController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
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
        public EventViewModel Get(int id)
        {
            var campaignEvent = GetEventBy(id);
            if (campaignEvent != null)
            {
                return new EventViewModel(campaignEvent);
            }
            

            // BUG Change this to IActionResult and use NotFound instead of null
            NotFound();
            return null;
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

            var campaignEvents = _mediator.Send(new EventsByGeographyQuery { Latitude = latitude, Longitude = longitude, Miles = miles});
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
        public ActionResult GetCheckin(int id)
        {
            var campaignEvent = GetEventBy(id);
            if (campaignEvent == null)
            {
                return NotFound();
            }
            
            return View("NoUserCheckin", campaignEvent);
        }

        [HttpPut("{id}/checkin")]
        [Authorize] 
        public async Task<ActionResult> PutCheckin(int id)
        {
            var campaignEvent = GetEventBy(id);
            if (campaignEvent == null)
            {
                return NotFound();
            }
            
            var userSignup = campaignEvent.UsersSignedUp.FirstOrDefault(u => u.User.Id == _userManager.GetUserId(User));
            if (userSignup != null && userSignup.CheckinDateTime == null)
            {
                userSignup.CheckinDateTime = DateTimeUtcNow.Invoke();
                await _mediator.SendAsync(new AddEventSignupCommandAsync { EventSignup = userSignup });
                return Json(new { Event = new { campaignEvent.Name, campaignEvent.Description }});
            }

            return Json(new { NeedsSignup = true, Event = new { campaignEvent.Name, campaignEvent.Description }});
        }

        [ValidateAntiForgeryToken]
        [HttpPost("signup")]
        [Authorize]
        public async Task<object> RegisterEvent(EventSignupViewModel signupModel)
        {
            if (signupModel == null)
            {
                return BadRequest();
            }
            
            if (!ModelState.IsValid)
            {
                // this condition should never be hit because client side validation is being performed
                // but just to cover the bases, if this does happen send the erros to the client
                return Json(new { errors = ModelState.GetErrorMessages() });
            }

            await _mediator.SendAsync(new EventSignupCommand { EventSignup = signupModel });

            return new {Status = "success"};
        }

        [HttpDelete("{id}/signup")]
        [Authorize]
        public async Task<IActionResult> UnregisterEvent(int id)
        {
            var eventSignup = _mediator.Send(new EventSignupByEventIdAndUserIdQuery { EventId = id, UserId = _userManager.GetUserId(User) });
            if (eventSignup == null)
            {
                return NotFound();
            }

            await _mediator.SendAsync(new UnregisterEvent { EventSignupId = eventSignup.Id, UserId = eventSignup.User.Id});

            return Ok();
        }

        private Event GetEventBy(int eventId) => _mediator.Send(new EventByIdQuery { EventId = eventId });
    }
}