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
using AllReady.Areas.Admin.Features.Import;
using AllReady.Areas.Admin.ViewModels.Import;
using AllReady.Areas.Admin.ViewModels.Request;
using AllReady.Features.Requests;
using AllReady.Security;
using System.Threading.Tasks;
using AllReady.Constants;
using AllReady.Features.Sms;
using AllReady.Models;

namespace AllReady.Areas.Admin.Controllers
{
    [Area(AreaNames.Admin)]
    [Authorize(nameof(UserType.OrgAdmin))]
    public class ImportController : Controller
    {
        private readonly ILogger<ImportController> logger;
        private readonly ICsvFactory csvFactory;
        private readonly IMediator mediator;

        public ImportController(IMediator mediator, ILogger<ImportController> logger, ICsvFactory csvFactory)
        {
            this.mediator = mediator;
            this.logger = logger;
            this.csvFactory = csvFactory;
        }

        public IActionResult Index()
        {
            var query = new IndexQuery();

            if (User.IsOrganizationAdmin())
            {
                query.OrganizationId = User.GetOrganizationId();
            }

            var viewModel = mediator.Send(query);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel viewModel)
        {
            List<ImportRequestViewModel> importRequestViewModels;

            if (viewModel.EventId == 0)
            {
                viewModel.ImportErrors.Add("please select an Event.");
            }

            if (viewModel.File == null)
            {
                viewModel.ImportErrors.Add("please select a file to upload.");
            }

            if (viewModel.ImportErrors.Count > 0)
            {
                return View(viewModel);
            }

            using (var stream = viewModel.File.OpenReadStream())
            {
                using (var reader = new StreamReader(stream))
                {
                    var csvReader = csvFactory.CreateReader(reader);
                    csvReader.Configuration.WillThrowOnMissingField = false;
                    csvReader.Configuration.RegisterClassMap<RedCrossRequestMap>();
                    importRequestViewModels = csvReader.GetRecords<ImportRequestViewModel>().ToList();

                    if (importRequestViewModels.Count == 0)
                    {
                        viewModel.ImportErrors.Add("you uploaded an empty file.");
                        return View(viewModel);
                    }

                    var duplicateProviderRequestIds = mediator.Send(new DuplicateProviderRequestIdsQuery { ProviderRequestIds = importRequestViewModels.Select(x => x.Id).ToList() });
                    if (duplicateProviderRequestIds.Count > 0)
                    {
                        viewModel.ImportErrors.Add($"These id's already exist in the system. Please remove them from the CSV and try again: {string.Join(", ", duplicateProviderRequestIds)}");
                        return View(viewModel);
                    }

                    var invalidPhoneNumbers = new List<string>();
                    foreach (var requestToImport in importRequestViewModels)
                    {
                        //validate incoming phone number on each request
                        var validatePhoneNumberResult = await mediator.SendAsync(new ValidatePhoneNumberRequestCommand { PhoneNumber = requestToImport.Phone, ValidateType = true });
                        if (!validatePhoneNumberResult.IsValid)
                        {
                            invalidPhoneNumbers.Add(requestToImport.Phone);
                        }
                        else
                        {
                            requestToImport.Phone = validatePhoneNumberResult.PhoneNumberE164;
                        }

                        //run validations on attributes on viewmodel
                        var validationResults = new List<ValidationResult>();
                        if (!Validator.TryValidateObject(requestToImport, new ValidationContext(requestToImport, null, null), validationResults, true))
                        {
                            var newValidationError = new IndexViewModel.ValidationError { ProviderRequestId = string.IsNullOrEmpty(requestToImport.Id) ? "id value is blank" : requestToImport.Id };
                            newValidationError.Errors.AddRange(validationResults);
                            viewModel.ValidationErrors.Add(newValidationError);
                        }
                    }

                    if (invalidPhoneNumbers.Count > 0)
                    {
                        viewModel.ImportErrors.Add($"These phone numbers are not valid mobile numbers: {string.Join(", ", invalidPhoneNumbers)}");
                    }
                }
            }

            if (viewModel.ImportErrors.Count == 0 && viewModel.ValidationErrors.Count == 0)
            {
                await mediator.SendAsync(new ImportRequestsCommand { EventId = viewModel.EventId, ImportRequestViewModels = importRequestViewModels.ToList() });
                logger.LogInformation($"{User.Identity.Name} imported file {viewModel.File.Name}");
                viewModel.ImportSuccess = true;
            }

            return View(viewModel);
        }
    }
}