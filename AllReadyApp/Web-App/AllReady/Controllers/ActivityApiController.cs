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

        // GET api/values/5
        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(ActivityViewModel))]
        public ActivityViewModel Get(int id)
        {
            var dbActivity = _allReadyDataAccess.GetActivity(id);
            if (dbActivity == null)
            {
                HttpNotFound();
                return null;
            }

            return new ActivityViewModel(dbActivity);
        }

        [Route("search")]
        public IEnumerable<ActivityViewModel> GetActivitiesByZip(string zip, int miles)
        {
            var ret = new List<ActivityViewModel>();

            var activities = _allReadyDataAccess.ActivitiesByPostalCode(zip, miles);

            foreach (var activity in activities)
            {
                ret.Add(new ActivityViewModel(activity));
            }

            return ret;
        }

        [Route("searchbylocation")]
        public IEnumerable<ActivityViewModel> GetActivitiesByLocation(double latitude, double longitude, int miles)
        {
            var ret = new List<ActivityViewModel>();

            var activities = _allReadyDataAccess.ActivitiesByGeography(latitude, longitude, miles);

            foreach (var activity in activities)
            {
                ret.Add(new ActivityViewModel(activity));
            }

            return ret;
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

        [HttpGet("{id}/checkin")]
        public ActionResult GetCheckin(int id)
        {
            var dbActivity = _allReadyDataAccess.GetActivity(id);
            if (dbActivity == null)
            {
                return HttpNotFound();
            }

            return View("NoUserCheckin", (dbActivity));
        }

        [HttpPut("{id}/checkin")]
        [Authorize] 
        public async Task<ActionResult> PutCheckin(int id)
        {
            var userId = User.GetUserId();
            var dbActivity = _allReadyDataAccess.GetActivity(id);
            if (dbActivity?.UsersSignedUp == null)
            {
                return HttpNotFound();
            }

            var userSignup = dbActivity.UsersSignedUp.FirstOrDefault(u => u.User.Id == userId);
            if (userSignup != null && userSignup.CheckinDateTime == null)
            {
                userSignup.CheckinDateTime = DateTime.UtcNow;
                await _allReadyDataAccess.AddActivitySignupAsync(userSignup);
                return Json(new { Activity = new { dbActivity.Name, dbActivity.Description } });
            }

            return Json(new { NeedsSignup = true, Activity = new { dbActivity.Name, dbActivity.Description } });
        }

        [ValidateAntiForgeryToken]
        [HttpPost("signup")]
        [Authorize]
        public async Task<IActionResult> RegisterActivity(ActivitySignupViewModel signupModel)
        {
            if (signupModel == null)
            {
                return HttpBadRequest();
            }

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
        public async Task<IActionResult> UnregisterActivity(int id)
        {
            var signedUp = _allReadyDataAccess.GetActivitySignup(id, User.GetUserId());
            if (signedUp == null)
            {
                return HttpNotFound();
            }

            //Notify admins & volunteer
            await _mediator.PublishAsync(new UserUnenrolls {ActivityId = signedUp.Activity.Id, UserId = signedUp.User.Id});

            await _allReadyDataAccess.DeleteActivityAndTaskSignupsAsync(signedUp.Id);
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

    }
}
