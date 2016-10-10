using AllReady.Areas.Admin.Features.Requests;
using AllReady.Models;
using CsvHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using AllReady.Areas.Admin.ViewModels.Request;

namespace AllReady.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> _logger;
        private readonly IMediator _mediator;

        public ImportController(IMediator mediator, ILogger<ImportController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        } 

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(IFormFile file)
        {
            // todo: - proper view model
            //- more complete result type/info

            if (file == null)
            {
                _logger.LogInformation($"User {User.Identity.Name} attempted a file upload without specifying a file.");
                return RedirectToAction(nameof(Index));
            }

            using (var stream = file.OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var csvReader = new CsvReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    csvReader.Configuration.RegisterClassMap<RedCrossRequestMap>();
                    var requests = csvReader.GetRecords<Request>().ToList();

                    var errors = _mediator.Send(new ImportRequestsCommand { Requests = requests });
                }
            }

            _logger.LogDebug($"{User.Identity.Name} imported file {file.Name}");
            ViewBag.ImportSuccess = true;

            return View();
        }
    }
}