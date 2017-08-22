using System;
using System.Collections.Generic;
using System.IO;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.Features.Requests;
using AllReady.Areas.Admin.ViewModels.Import;
using AllReady.Features.Requests;
using AllReady.UnitTest.Extensions;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using System.Linq;
using AllReady.Areas.Admin.Features.Import;
using Microsoft.Extensions.Logging.Internal;
using System.Threading.Tasks;
using AllReady.Features.Sms;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class ImportControllerTests
    {
        [Fact]
        public void IndexGetReturnsCorrectViewAndViewModel()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<IndexQuery>())).Returns(new IndexViewModel());

            var sut = new ImportController(mediator.Object, null, null);
            sut.MakeUserASiteAdmin();
            var result = sut.Index() as ViewResult;

            Assert.IsType<IndexViewModel>(result.Model);
            Assert.Null(result.ViewName);
        }

        [Fact]
        public void IndexGetSendsIndexQuery_WithCorrectOrganizationIdWhenUserIsAnOrgAdmin()
        {
            const int organizationId = 99;
            var mediator = new Mock<IMediator>();

            var sut = new ImportController(mediator.Object, null, null);
            sut.MakeUserAnOrgAdmin(organizationId.ToString());
            sut.Index();

            mediator.Verify(x => x.Send(It.Is<IndexQuery>(y => y.OrganizationId == organizationId)), Times.Once);
        }

        [Fact]
        public void IndexGetSendsIndexQuery_WithCorrectOrganizationIdWhenUserIsNotanOrgAdmin()
        {
            var mediator = new Mock<IMediator>();

            var sut = new ImportController(mediator.Object, null, null);
            sut.MakeUserASiteAdmin();
            sut.Index();

            mediator.Verify(x => x.Send(It.Is<IndexQuery>(y => y.OrganizationId == null)), Times.Once);
        }

        [Fact]
        public async Task IndexPostReturnsTheCorrectViewModelAndView()
        {
            var sut = new ImportController(null, null, null);
            var result = await sut.Index(new IndexViewModel()) as ViewResult;

            Assert.IsType<IndexViewModel>(result.Model);
            Assert.Null(result.ViewName);
        }

        [Fact]
        public async Task IndexPostReturnsCorrectImportError_WhenEventIsNotPicked()
        {
            var sut = new ImportController(null, null, null);
            var result = (IndexViewModel)((ViewResult) await sut.Index(new IndexViewModel())).Model;

            Assert.Contains("please select an Event.", result.ImportErrors);
        }

        [Fact]
        public async Task IndexPostReturnsCorrectImportError_WhenNoFileIsUploaded()
        {
            var sut = new ImportController(null, null, null);
            var result = (IndexViewModel)((ViewResult) await sut.Index(new IndexViewModel())).Model;

            Assert.Contains("please select a file to upload.", result.ImportErrors);
        }

        [Fact]
        public async Task IndexPostReturnsCorrectImportError_WhenUploadedFileIsEmpty()
        {
            Mock<IFormFile> iFormFile;
            Mock<ICsvFactory> csvFactory;
            Mock<ICsvReader> csvReader;
            CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out iFormFile, out csvFactory, out csvReader);

            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(new List<ImportRequestViewModel>());

            var sut = new ImportController(null, null, csvFactory.Object);
            var result = (IndexViewModel)((ViewResult) await sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object })).Model;

            Assert.Contains("you uploaded an empty file.", result.ImportErrors);
        }

        [Fact]
        public async Task IndexPostSendsDuplicateProviderRequestIdsQueryWithCorrectProviderRequestIds()
        {
            const string id = "id";
            var importRequestViewModels = new List<ImportRequestViewModel> { new ImportRequestViewModel { Id = id } };

            Mock<IFormFile> iFormFile;
            Mock<ICsvFactory> csvFactory;
            Mock<ICsvReader> csvReader;
            CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out iFormFile, out csvFactory, out csvReader);
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());
            mediator.Setup(x => x.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = true, PhoneNumberE164 = importRequestViewModels[0].Phone });

            var sut = new ImportController(mediator.Object, null, csvFactory.Object);
            await sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object });

            mediator.Verify(x => x.Send(It.Is<DuplicateProviderRequestIdsQuery>(y => y.ProviderRequestIds[0] == id)), Times.Once);
        }

        [Fact]
        public async Task IndexPostReturnsCorrectImportError_WhenThereAreDuplicateProviderRequestIdsFound()
        {
            const string duplicateId = "id";
            var duplicateIds = new List<string> { duplicateId };
            var importRequestViewModels = new List<ImportRequestViewModel> { new ImportRequestViewModel { Id = duplicateId } };

            Mock<IFormFile> iFormFile;
            Mock<ICsvFactory> csvFactory;
            Mock<ICsvReader> csvReader;
            CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out iFormFile, out csvFactory, out csvReader);
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(duplicateIds);

            var sut = new ImportController(mediator.Object, null, csvFactory.Object);
            var result = (IndexViewModel)((ViewResult) await sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object })).Model;

            Assert.Contains($"These id's already exist in the system. Please remove them from the CSV and try again: {string.Join(", ", duplicateIds)}", result.ImportErrors);
        }

        [Fact]
        public async Task IndexPostSendsValidatePhoneNumberRequestComamndWithCorrectData()
        {
            const string id = "id";
            var importRequestViewModels = new List<ImportRequestViewModel> { new ImportRequestViewModel { Id = id } };

            Mock<IFormFile> iFormFile;
            Mock<ICsvFactory> csvFactory;
            Mock<ICsvReader> csvReader;
            CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out iFormFile, out csvFactory, out csvReader);
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());
            mediator.Setup(x => x.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = true, PhoneNumberE164 = importRequestViewModels[0].Phone });

            var sut = new ImportController(mediator.Object, null, csvFactory.Object);
            await sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object });

            mediator.Verify(x => x.SendAsync(It.Is<ValidatePhoneNumberRequestCommand>(y => y.PhoneNumber == importRequestViewModels[0].Phone && y.ValidateType)));
        }

        [Fact]
        public async Task IndexPostReturnsCorrectImportError_WhenPhoneNumbersAreInvalid()
        {
            var importRequestViewModels = new List<ImportRequestViewModel>
            {
                new ImportRequestViewModel { Id =  "Id", Phone = "InvalidPhoneNumber" }
            };

            Mock<IFormFile> iFormFile;
            Mock<ICsvFactory> csvFactory;
            Mock<ICsvReader> csvReader;
            CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out iFormFile, out csvFactory, out csvReader);
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());
            mediator.Setup(x => x.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = false, PhoneNumberE164 = null });

            var sut = new ImportController(mediator.Object, null, csvFactory.Object);
            var viewResult = await sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object }) as ViewResult;
            var result = viewResult.Model as IndexViewModel;

            Assert.Equal(result.ImportErrors[0], $"These phone numbers are not valid mobile numbers: {importRequestViewModels[0].Phone}");
        }

        [Fact]
        public async Task IndexPostReturnsCorrectValidationErrors_WhenThereAreValidationErrors()
        {
            var importRequestViewModels = new List<ImportRequestViewModel>
            {
                new ImportRequestViewModel { Id =  "Id", Name = null, Address = string.Empty, Email = "InvalidEmail" }
            };

            Mock<IFormFile> iFormFile;
            Mock<ICsvFactory> csvFactory;
            Mock<ICsvReader> csvReader;
            CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out iFormFile, out csvFactory, out csvReader);
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());
            mediator.Setup(x => x.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = true, PhoneNumberE164 = importRequestViewModels[0].Phone });

            var sut = new ImportController(mediator.Object, null, csvFactory.Object);
            var viewResult = await sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object }) as ViewResult;
            var result = viewResult.Model as IndexViewModel;

            Assert.Equal(result.ValidationErrors[0].ProviderRequestId, importRequestViewModels[0].Id);
            Assert.Equal("The Name field is required.", result.ValidationErrors[0].Errors[0].ErrorMessage);
            Assert.Equal("The Address field is required.", result.ValidationErrors[0].Errors[1].ErrorMessage);
            Assert.Equal("The City field is required.", result.ValidationErrors[0].Errors[2].ErrorMessage);
            Assert.Equal("The State field is required.", result.ValidationErrors[0].Errors[3].ErrorMessage);
            Assert.Equal("The PostalCode field is required.", result.ValidationErrors[0].Errors[4].ErrorMessage);
            Assert.Equal("The Phone field is required.", result.ValidationErrors[0].Errors[5].ErrorMessage);
            Assert.Equal("Invalid Email Address", result.ValidationErrors[0].Errors[6].ErrorMessage);
        }

        [Fact]
        public async Task IndexPostSendsImportRequestsCommandWithTheCorrectViewModel()
        {
            var importRequestViewModels = new List<ImportRequestViewModel>
            {
                new ImportRequestViewModel { Id =  "Id", Name = "Name", Address = "Address", City = "City", Email = "email@email.com", Phone = "111-111-1111", State = "State", PostalCode = "PostalCode" }
            };

            Mock<IFormFile> iFormFile;
            Mock<ICsvFactory> csvFactory;
            Mock<ICsvReader> csvReader;
            CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out iFormFile, out csvFactory, out csvReader);
            iFormFile.Setup(x => x.Name).Returns(It.IsAny<string>());
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());
            mediator.Setup(x => x.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = true, PhoneNumberE164 = importRequestViewModels[0].Phone });

            var sut = new ImportController(mediator.Object, Mock.Of<ILogger<ImportController>>(), csvFactory.Object);
            sut.SetFakeUserName("UserName");
            await sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object });

            mediator.Verify(x => x.SendAsync(It.Is<ImportRequestsCommand>(y => 
                y.ImportRequestViewModels[0].Id == importRequestViewModels[0].Id &&
                y.ImportRequestViewModels[0].Name == importRequestViewModels[0].Name &&
                y.ImportRequestViewModels[0].Address == importRequestViewModels[0].Address &&
                y.ImportRequestViewModels[0].City == importRequestViewModels[0].City &&
                y.ImportRequestViewModels[0].Email == importRequestViewModels[0].Email &&
                y.ImportRequestViewModels[0].Phone == importRequestViewModels[0].Phone &&
                y.ImportRequestViewModels[0].State == importRequestViewModels[0].State &&
                y.ImportRequestViewModels[0].PostalCode == importRequestViewModels[0].PostalCode &&
                y.ImportRequestViewModels[0].Longitude == 0 &&
                y.ImportRequestViewModels[0].Latitude == 0 &&
                y.ImportRequestViewModels[0].ProviderData == null)), Times.Once);
        }

        [Fact]
        public async Task IndexPostLogsCorrectMessage_WhenImportSucceeds()
        {
            const string userName = "UserName";
            const string fileName = "FileName";

            var importRequestViewModels = new List<ImportRequestViewModel>
            {
                new ImportRequestViewModel { Id =  "Id", Name = "Name", Address = "Address", City = "City", Email = "email@email.com", Phone = "111-111-1111", State = "State", PostalCode = "PostalCode" }
            };

            Mock<IFormFile> iFormFile;
            Mock<ICsvFactory> csvFactory;
            Mock<ICsvReader> csvReader;
            CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out iFormFile, out csvFactory, out csvReader);
            iFormFile.Setup(x => x.Name).Returns(fileName);
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<ImportController>>();
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());
            mediator.Setup(x => x.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = true, PhoneNumberE164 = importRequestViewModels[0].Phone });

            var sut = new ImportController(mediator.Object, logger.Object, csvFactory.Object);
            sut.SetFakeUserName(userName);
            await sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object });

            logger.Verify(m => m.Log(LogLevel.Information, It.IsAny<EventId>(),
            It.Is<FormattedLogValues>(v => v.ToString() == $"{userName} imported file {fileName}"),
            null, It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task IndexPostAssignsImportSuccessToTrue_WhenImportSucceeds()
        {
            var importRequestViewModels = new List<ImportRequestViewModel>
            {
                new ImportRequestViewModel { Id =  "Id", Name = "Name", Address = "Address", City = "City", Email = "email@email.com", Phone = "111-111-1111", State = "State", PostalCode = "PostalCode" }
            };

            Mock<IFormFile> iFormFile;
            Mock<ICsvFactory> csvFactory;
            Mock<ICsvReader> csvReader;
            CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out iFormFile, out csvFactory, out csvReader);
            iFormFile.Setup(x => x.Name).Returns(It.IsAny<string>());
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());
            mediator.Setup(x => x.SendAsync(It.IsAny<ValidatePhoneNumberRequestCommand>())).ReturnsAsync(new ValidatePhoneNumberResult { IsValid = true, PhoneNumberE164 = importRequestViewModels[0].Phone });

            var sut = new ImportController(mediator.Object, Mock.Of<ILogger<ImportController>>(), csvFactory.Object);
            sut.SetFakeUserName("UserName");
            var result = (IndexViewModel)((ViewResult)await sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object })).Model;

            Assert.True(result.ImportSuccess);
        }

        [Fact]
        public void IndexPostHasHttpPostAttribute()
        {
            var sut = new ImportController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Index(It.IsAny<IndexViewModel>())).OfType<HttpPostAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        [Fact]
        public void IndexPostHasValidateAntiForgeryTokenAttribute()
        {
            var sut = new ImportController(null, null, null);
            var attribute = sut.GetAttributesOn(x => x.Index(It.IsAny<IndexViewModel>())).OfType<ValidateAntiForgeryTokenAttribute>().SingleOrDefault();
            Assert.NotNull(attribute);
        }

        private static void CreateMockAndSetupIFormFileCsvFactoryAndCsvReader(out Mock<IFormFile> iFormFile, out Mock<ICsvFactory> csvFactory, out Mock<ICsvReader> csvReader)
        {
            iFormFile = new Mock<IFormFile>();
            csvFactory = new Mock<ICsvFactory>();
            csvReader = new Mock<ICsvReader>();

            iFormFile.Setup(x => x.OpenReadStream()).Returns(new MemoryStream());
            csvFactory.Setup(x => x.CreateReader(It.IsAny<TextReader>())).Returns(csvReader.Object);
            csvReader.Setup(x => x.Configuration).Returns(new CsvConfiguration());
        }
    }
}
