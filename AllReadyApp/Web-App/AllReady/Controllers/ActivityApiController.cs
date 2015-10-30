using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using AllReady.Models;
using AllReady.Services;
using AllReady.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AllReady.Controllers
{
    [Route("api/activity")]
    [Produces("application/json")]
    public class ActivityApiController : Controller
    {
        private const double MILES_PER_METER = 0.00062137;
        private readonly IAllReadyDataAccess _allReadyDataAccess;
        private IClosestLocations _closestLocations;

        public ActivityApiController(IAllReadyDataAccess allReadyDataAccess,
            //GeoService geoService,
            IClosestLocations closestLocations)
        {
            _allReadyDataAccess = allReadyDataAccess;
            //_geoService = geoService;
            _closestLocations = closestLocations;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<ActivityViewModel> Get()
        {
            return _allReadyDataAccess.Activities.Select(a => new ActivityViewModel(a));
        }

        // GET api/values/5

        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(ActivityViewModel))]
        public ActivityViewModel Get(int id)
        {
            var dbActivity = _allReadyDataAccess.GetActivity(id);

            if (dbActivity != null)
            {
                return new ActivityViewModel(dbActivity);
            }
            this.HttpNotFound();
            return null;
        }

        [Route("search")]
        public IEnumerable<ActivityViewModel> GetActivitiesByZip(string zip, int miles)
        {
            List<ActivityViewModel> ret = new List<ActivityViewModel>();

            var activities = _allReadyDataAccess.ActivitiesByPostalCode(zip, miles);

            foreach (Activity activity in activities)
            {
                ret.Add(new ActivityViewModel(activity));
            }

            return ret;
        }

        [Route("searchbylocation")]
        public IEnumerable<ActivityViewModel> GetActivitiesByLocation(double latitude, double longitude, int miles)
        {
            List<ActivityViewModel> ret = new List<ActivityViewModel>();

            var activities = _allReadyDataAccess.ActivitiesByGeography(latitude, longitude, miles);

            foreach (Activity activity in activities)
            {
                ret.Add(new ActivityViewModel(activity));
            }

            return ret;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public void Post([FromBody]ActivityViewModel value)
        {
            bool alreadyExists = _allReadyDataAccess.GetActivity(value.Id) != null;
            if (alreadyExists)
            {
                this.HttpBadRequest();
            }
            _allReadyDataAccess.AddActivity(value.ToModel(_allReadyDataAccess));
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]ActivityViewModel value)
        {
            var activity = _allReadyDataAccess.GetActivity(value.Id);
            if (activity == null)
            {
                this.HttpBadRequest();
            }
            var associatedCampaign = _allReadyDataAccess.GetCampaign(value.CampaignId);
            var tenant = _allReadyDataAccess.GetTenant(value.TenantId);

            // TODO:  Helper method to do this kind of conversion.
            activity.Campaign = associatedCampaign;
            activity.EndDateTimeUtc = value.EndDateTime.UtcDateTime;
            activity.Location = value.Location.ToModel();
            activity.Name = value.Title;
            activity.StartDateTimeUtc = value.StartDateTime.UtcDateTime;
            activity.Tasks = value.Tasks.ToModel(_allReadyDataAccess).ToList();
            activity.Tenant = tenant;

            _allReadyDataAccess.UpdateActivity(activity);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _allReadyDataAccess.DeleteActivity(id);
        }

        [HttpGet("{id}/qrcode")]
        public ActionResult GetQrCode(int id)
        {
            ZXing.BarcodeWriter barcodeWriter = new ZXing.BarcodeWriter { Format = ZXing.BarcodeFormat.QR_CODE };
            barcodeWriter.Options.Width = 200;
            barcodeWriter.Options.Height = 200;

            // The QR code should point users to /api/activity/{id}/checkin
            string path = this.Request.Path.ToString();
            string checkinPath = path.Substring(0, path.Length - "qrcode".Length) + "checkin";

            var bitmap = barcodeWriter.Write(String.Format("https://{0}/{1}", this.Request.Host, checkinPath));
            using (MemoryStream stream = new MemoryStream())
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
                return this.HttpNotFound();
            }
            return View("NoUserCheckin", (dbActivity));
        }

        [HttpPut("{id}/checkin")]
        [Authorize()] 
        public async Task<ActionResult> PutCheckin(int id)
        {
            var userId = User.GetUserId();
            var dbActivity = _allReadyDataAccess.GetActivity(id);
            if (dbActivity == null || dbActivity.UsersSignedUp == null)
            {
                return this.HttpNotFound();
            }

            var userSignup = dbActivity.UsersSignedUp.FirstOrDefault(u => u.User.Id == userId);
            if (userSignup != null && userSignup.CheckinDateTime == null)
            {
                userSignup.CheckinDateTime = DateTime.UtcNow;
                await _allReadyDataAccess.AddActivitySignupAsync(userSignup);
                return Json(new { Activity = new { Name = dbActivity.Name, Description = dbActivity.Description } });
            }
            else
            {
                return Json(new { NeedsSignup = true, Activity = new { Name = dbActivity.Name, Description = dbActivity.Description } });
            }
        }

        [HttpPost("{id}/signup")]
        [Authorize] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterActivity(int id)
        {
            var user = _allReadyDataAccess.GetUser(User.GetUserId());

            var activity = _allReadyDataAccess.GetActivity(id);

            if (activity == null)
            {
                return HttpNotFound();
            }

            if (activity.UsersSignedUp == null)
            {
                activity.UsersSignedUp = new List<ActivitySignup>();
            }

            activity.UsersSignedUp.Add(new ActivitySignup
            {
                Activity = activity,
                User = user,
                SignupDateTime = DateTime.UtcNow
            });

            await _allReadyDataAccess.UpdateActivity(activity);
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
            await _allReadyDataAccess.DeleteActivitySignupAsync(signedUp.Id);
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

    }
}
