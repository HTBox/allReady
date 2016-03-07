using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Features.Activity;
using AllReady.Features.Notifications;
using MediatR;

namespace AllReady.Controllers
{
    [Route("api/activity")]
    [Produces("application/json")]
    public class ActivityApiController : Controller
    {
        private readonly IAllReadyDataAccess _allReadyDataAccess;
        private readonly IMediator _mediator;
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public ActivityApiController(IAllReadyDataAccess allReadyDataAccess, IMediator mediator)
        {
            _allReadyDataAccess = allReadyDataAccess;
            _mediator = mediator;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<ActivityViewModel> Get()
        {
            return _allReadyDataAccess.Activities
                .Where(c => !c.Campaign.Locked)
                .Select(a => new ActivityViewModel(a));
        }

        //orginial code
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(ActivityViewModel))]
        public ActivityViewModel Get(int id)
        {
            var activity = _allReadyDataAccess.GetActivity(id);

            if (activity != null)
                return new ActivityViewModel(activity);

            HttpNotFound();
            return null;
        }
        //changes ReSharper prompted me to make
        //[HttpGet("{id}")]
        //[Produces("application/json", Type = typeof(ActivityViewModel))]
        //public ActivityViewModel Get(int id)
        //{
        //    var dbActivity = _allReadyDataAccess.GetActivity(id);
        //    if (dbActivity == null)
        //    {
        //        HttpNotFound();
        //        return null;
        //    }
        //    return new ActivityViewModel(dbActivity);
        //}

        [Route("search")]
        public IEnumerable<ActivityViewModel> GetActivitiesByPostalCode(string zip, int miles)
        {
            var model = new List<ActivityViewModel>();

            //TODO: refactor to mediator
            var activities = _allReadyDataAccess.ActivitiesByPostalCode(zip, miles);

            foreach (var activity in activities)
            {
                model.Add(new ActivityViewModel(activity));
            }

            return model;
        }

        //TODO: refactor to mediator
        [Route("searchbylocation")]
        public IEnumerable<ActivityViewModel> GetActivitiesByGeography(double latitude, double longitude, int miles)
        {
            var model = new List<ActivityViewModel>();
            var activities = _allReadyDataAccess.ActivitiesByGeography(latitude, longitude, miles);

            foreach (var activity in activities)
            {
                model.Add(new ActivityViewModel(activity));
            }

            return model;
        }
        
        [HttpGet("{id}/qrcode")]
        public ActionResult GetQrCode(int id)
        {
            var barcodeWriter = new ZXing.BarcodeWriter { Format = ZXing.BarcodeFormat.QR_CODE };
            barcodeWriter.Options.Width = 200;
            barcodeWriter.Options.Height = 200;

            // The QR code should point users to /api/activity/{id}/checkin
            var path = Request.Path.ToString();
            var checkinPath = path.Substring(0, path.Length - "qrcode".Length) + "checkin";

            var bitmap = barcodeWriter.Write($"https://{Request.Host}/{checkinPath}");
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return File(stream.ToArray(), "image/png");
            }
        }

        //TODO: refactor to mediator
        [HttpGet("{id}/checkin")]
        public ActionResult GetCheckin(int id)
        {
            var activity = _allReadyDataAccess.GetActivity(id);
            if (activity == null)
                return HttpNotFound();

            return View("NoUserCheckin", activity);
        }

        [HttpPut("{id}/checkin")]
        [Authorize] 
        public async Task<ActionResult> PutCheckin(int id)
        {
            var activity = _allReadyDataAccess.GetActivity(id);
            if (activity == null)
                return HttpNotFound();

            var userSignup = activity.UsersSignedUp.FirstOrDefault(u => u.User.Id == User.GetUserId());
            if (userSignup != null && userSignup.CheckinDateTime == null)
            {
                userSignup.CheckinDateTime = DateTimeUtcNow.Invoke();
                await _allReadyDataAccess.AddActivitySignupAsync(userSignup);
                return Json(new { Activity = new { activity.Name, activity.Description }});
            }

            return Json(new { NeedsSignup = true, Activity = new { activity.Name, activity.Description }});
        }

        [ValidateAntiForgeryToken]
        [HttpPost("signup")]
        [Authorize]
        public async Task<IActionResult> RegisterActivity(ActivitySignupViewModel signupModel)
        {
            if (signupModel == null)
                return HttpBadRequest();

            if (!ModelState.IsValid)
            {
                // this condition should never be hit because client side validation is being performed
                // but just to cover the bases, if this does happen send the erros to the client
                return Json(new { errors = ModelState.GetErrorMessages() });
            }

            await _mediator.SendAsync(new ActivitySignupCommand { ActivitySignup = signupModel });
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        [HttpDelete("{id}/signup")]
        [Authorize]
        //int id here is really ActivityId
        public async Task<IActionResult> UnregisterActivity(int id)
        {
            //TODO: refactor to mediator
            var activitySignup = _allReadyDataAccess.GetActivitySignup(id, User.GetUserId());
            if (activitySignup == null)
                return HttpNotFound();

            //Notify admins & volunteer
            await _mediator.PublishAsync(new UserUnenrolls { ActivityId = activitySignup.Activity.Id, UserId = activitySignup.User.Id });

            //since .GetActivitySignup is called with the ActivityId, which returns and ActivitySignup which has the same Activity entity off of it as a property, then ActivityId line below can
            //be changed from "ActivityId = activitySignup.Activity.Id" to "ActivityId = id", but I"m not going to do that right now
            //await _mediator.PublishAsync(new UserUnenrolls { ActivityId = activitySignup.Activity.Id, UserId = activitySignup.User.Id });

            //TODO: refactor to mediator
            await _allReadyDataAccess.DeleteActivityAndTaskSignupsAsync(activitySignup.Id);

            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }
    }
}
