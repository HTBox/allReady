using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.Models.RequestModels;
using AllReady.Models;
using CsvHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;

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
            using (var stream = file.OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var csvReader = new CsvReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    csvReader.Configuration.RegisterClassMap<RedCrossRequestMap>();
                    var requests = csvReader.GetRecords<Request>().ToList();

                    var errors = _mediator.Send(new AddRequestsCommand { Requests = requests });

                }
            }

            // todo: - add error handling logic/results view
            //       - proper view model
            //       - more complete result type/info

            ViewBag.ImportSuccess = true;

            return View();
        }

    }
}
