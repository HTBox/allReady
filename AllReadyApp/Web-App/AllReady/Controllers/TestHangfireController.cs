using System;
using System.Diagnostics;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace AllReady.Controllers
{
    public class TestHangfireController : Controller
    {
        private readonly IBackgroundJobClient backgroundJob;
        public TestHangfireController(IBackgroundJobClient backgroundJob)
        {
            this.backgroundJob = backgroundJob;
        }

        public IActionResult Index()
        {
            //BackgroundJob.Enqueue(() => Debug.WriteLine("Background Job completed successfully!"));

            //backgroundJob.Enqueue(() => Debug.WriteLine("Background Job completed successfully!"));
            //Debug.WriteLine("Background job has been created.");

#if Debug
            backgroundJob.Schedule(() => Debug.WriteLine("Scheduled Job completed successfully!"), TimeSpan.FromSeconds(5));
#endif
            Debug.WriteLine("Scheduled job has been created.");

            return RedirectToAction("Index", "Home");
        }
    }
}
