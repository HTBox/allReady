using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using PrepOps.Models;
using PrepOps.Services;
using PrepOps.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrepOps.Controllers
{
    [Route("api/activity")]
    [Produces("application/json")]
    public class ActivityApiController : Controller
    {
        private const double MILES_PER_METER = 0.00062137;
        private readonly IPrepOpsDataAccess _prepOpsDataAccess;
        private readonly UserManager<ApplicationUser> _userManager;
        private IClosestLocations _closestLocations;

        public ActivityApiController(IPrepOpsDataAccess prepOpsDataAccess,
            UserManager<ApplicationUser> userManager,
            //GeoService geoService,
            IClosestLocations closestLocations)
        {
            _prepOpsDataAccess = prepOpsDataAccess;
            _userManager = userManager;
            //_geoService = geoService;
            _closestLocations = closestLocations;
        }

        // GET: api/values
        [HttpGet]
        public IEnumerable<ActivityViewModel> Get()
        {
            return _prepOpsDataAccess.Activities.Select(a => new ActivityViewModel(a));
        }

        // GET api/values/5

        [HttpGet("{id}")]
        [Produces("application/json", Type = typeof(ActivityViewModel))]
        public ActivityViewModel Get(int id)
        {
            var dbActivity = _prepOpsDataAccess.GetActivity(id);

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

            var activities = _prepOpsDataAccess.ActivitiesByPostalCode(zip, miles);

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

            var activities = _prepOpsDataAccess.ActivitiesByGeography(latitude, longitude, miles);

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
            bool alreadyExists = _prepOpsDataAccess.GetActivity(value.Id) != null;
            if (alreadyExists)
            {
                this.HttpBadRequest();
            }
            _prepOpsDataAccess.AddActivity(value.ToModel(_prepOpsDataAccess));
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]ActivityViewModel value)
        {
            var activity = _prepOpsDataAccess.GetActivity(value.Id);
            if (activity == null)
            {
                this.HttpBadRequest();
            }
            var associatedCampaign = _prepOpsDataAccess.GetCampaign(value.CampaignId);
            var tenant = _prepOpsDataAccess.GetTenant(value.TenantId);

            // TODO:  Helper method to do this kind of conversion.
            activity.Campaign = associatedCampaign;
            activity.EndDateTimeUtc = value.EndDateTime.UtcDateTime;
            activity.Location = value.Location.ToModel();
            activity.Name = value.Title;
            activity.StartDateTimeUtc = value.StartDateTime.UtcDateTime;
            activity.Tasks = value.Tasks.ToModel(_prepOpsDataAccess).ToList();
            activity.Tenant = tenant;

            _prepOpsDataAccess.UpdateActivity(activity);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _prepOpsDataAccess.DeleteActivity(id);
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
            var dbActivity = _prepOpsDataAccess.GetActivity(id);
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
            var user = await GetCurrentUserAsync();
            var dbActivity = _prepOpsDataAccess.GetActivity(id);
            if (dbActivity == null || dbActivity.UsersSignedUp == null)
            {
                return this.HttpNotFound();
            }

            var userSignup = dbActivity.UsersSignedUp.FirstOrDefault(u => u.User.Id == user.Id);
            if (userSignup != null && userSignup.CheckinDateTime == null)
            {
                userSignup.CheckinDateTime = DateTime.UtcNow;
                _prepOpsDataAccess.AddActivitySignup(userSignup);
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
            var user = await GetCurrentUserAsync();

            var activity = _prepOpsDataAccess.GetActivity(id);

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

            await _prepOpsDataAccess.UpdateActivity(activity);
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        [HttpDelete("{id}/signup")]
        [Authorize]
        public async Task<IActionResult> UnregisterActivity(int id)
        {
            var user = await GetCurrentUserAsync();
            var signedUp = _prepOpsDataAccess.GetActivitySignup(id, user.Id);
            if (signedUp == null)
            {
                return HttpNotFound();
            }
            _prepOpsDataAccess.DeleteActivitySignup(signedUp.Id);
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _userManager.FindByIdAsync(Context.User.GetUserId());
        }

    }
}
