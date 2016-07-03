using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.Models.RequestModels;
using AllReady.Models;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
