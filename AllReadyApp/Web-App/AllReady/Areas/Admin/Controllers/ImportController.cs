using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AllReady.Areas.Admin.Features.Requests;
using CsvHelper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using AllReady.Areas.Admin.ViewModels.Import;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Features.Requests;

namespace AllReady.Areas.Admin.Controllers
{
    //assumptions made:
    //this is an all or nothing operation, meaning that if one or more rows of the import fails, the entire import fails
    //this is not an add or update operation. we only handle new requests (inserts) via this UI
    [Area("Admin")]
    [Authorize("SiteAdmin")]
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> logger;
        private readonly IMediator mediator;

        public ImportController(IMediator mediator, ILogger<ImportController> logger)
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View(new IndexViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(IndexViewModel viewModel)
        {
            //so far, tested:
            //happy path
            //still need to test all validations then make a decision on whether or not to still use the index view or create a new view for the "confirmation"

            List<ImportRequestViewModel> requestsToImport;

            //if (file == null)
            if (viewModel.File == null)
            {
                //validationErrors.Add(new ValidationResult("please select a file to upload."));
                //ViewBag.ImportErrors = validationErrors;
                viewModel.ImportErrors.Add(new ValidationResult("please select a file to upload."));
                return View(viewModel);
            }

            //using (var stream = file.OpenReadStream())
            using (var stream = viewModel.File.OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var csvReader = new CsvReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    csvReader.Configuration.RegisterClassMap<RedCrossRequestMap>();
                    requestsToImport = csvReader.GetRecords<ImportRequestViewModel>().ToList();

                    //field validations
                    if (requestsToImport.Count == 0)
                    {
                        //validationErrors.Add(new ValidationResult("you uploaded and empty file. Please try again"));
                        //ViewBag.ImportErrors = validationErrors;
                        viewModel.ImportErrors.Add(new ValidationResult("you uploaded and empty file. Please try again"));
                        return View();
                    }

                    foreach (var reqeustToImport in requestsToImport)
                    {
                        //Validator.TryValidateObject(reqeustToImport, new ValidationContext(reqeustToImport, null, null), validationErrors, true);
                        Validator.TryValidateObject(reqeustToImport, new ValidationContext(reqeustToImport, null, null), viewModel.ImportErrors, true);
                    }

                    //business validations
                    var duplicateProviderRequestIds = mediator.Send(new DuplicateProviderRequestIdsQuery { ProviderRequestIds = requestsToImport.Select(x => x.ProviderRequestId).ToList() });
                    if (duplicateProviderRequestIds.Count > 0)
                    {
                        //validationErrors.Add(new ValidationResult($"These ProviderRequestIds already exist in the system. Please remove them from the CSV and try again: {string.Join(", ", duplicateProviderRequestIds)}"));
                        viewModel.ImportErrors.Add(new ValidationResult($"These ProviderRequestIds already exist in the system. Please remove them from the CSV and try again: {string.Join(", ", duplicateProviderRequestIds)}"));
                    }
                }
            }

            //if (validationErrors.Count == 0)
            //{
            //    mediator.Send(new ImportRequestsCommand { ImportRequestViewModels = requestsToImport.ToList() });
            //    //logger.LogDebug($"{User.Identity.Name} imported file {file.Name}");
            //    logger.LogDebug($"{User.Identity.Name} imported file {viewModel.File.Name}");
            //    ViewBag.ImportSuccess = true;
            //    ViewBag.ImportErrors = validationErrors;
            //}
            //else
            //{
            //    ViewBag.ImportErrors = validationErrors; 
            //}

            if (viewModel.ImportErrors.Count == 0)
            {
                mediator.Send(new ImportRequestsCommand { ImportRequestViewModels = requestsToImport.ToList() });
                //logger.LogDebug($"{User.Identity.Name} imported file {file.Name}");
                logger.LogDebug($"{User.Identity.Name} imported file {viewModel.File.Name}");
                //ViewBag.ImportSuccess = true;
                //ViewBag.ImportErrors = validationErrors;
                viewModel.ImportSuccess = true;
            }
            else
            {
                //ViewBag.ImportErrors = validationErrors;
            }

            return View(viewModel);
        }

        //[HttpGet]
        //public IActionResult Confirmation(IFormFile file)
        //{
        //    return View();
        //<div class="alert alert-success">
        //    <strong>Success!</strong> Your file has been uploaded.
        //</div>
        //}
    }
}