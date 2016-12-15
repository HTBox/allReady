using System.Collections.Generic;
using System.IO;
using AllReady.Areas.Admin.Controllers;
using AllReady.Areas.Admin.ViewModels.Import;
using AllReady.Features.Requests;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Controllers
{
    public class ImportControllerTests
    {
        [Fact]
        public void IndexGetReturnsCorrectViewAndViewModel()
        {
            var sut = new ImportController(null, null, null);
            var result = sut.Index() as ViewResult;

            Assert.IsType<IndexViewModel>(result.Model);
            Assert.Null(result.ViewName);
        }

        [Fact]
        public void IndexPostReturnsTheCorrectViewModelAndView()
        {
            var sut = new ImportController(null, null, null);
            var result = sut.Index(new IndexViewModel()) as ViewResult;

            Assert.IsType<IndexViewModel>(result.Model);
            Assert.Null(result.ViewName);
        }

        [Fact]
        public void IndexPostReturnsCorrectImportError_WhenNoFileIsUploaded()
        {
            var sut = new ImportController(null, null, null);
            var viewResult = sut.Index(new IndexViewModel()) as ViewResult;
            var result = viewResult.Model as IndexViewModel;
            Assert.True(result.ImportErrors.Contains("please select a file to upload."));
        }

        [Fact]
        public void IndexPostReturnsCorrectImportError_WhenUploadedFileIsEmpty()
        {
            var iFormFile = new Mock<IFormFile>();
            var csvFactory = new Mock<ICsvFactory>();
            var csvReader = new Mock<ICsvReader>();

            iFormFile.Setup(x => x.OpenReadStream()).Returns(new MemoryStream());
            csvFactory.Setup(x => x.CreateReader(It.IsAny<TextReader>())).Returns(csvReader.Object);
            csvReader.Setup(x => x.Configuration).Returns(new CsvConfiguration());
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(new List<ImportRequestViewModel>());

            var sut = new ImportController(null, null, csvFactory.Object);
            var viewResult = sut.Index(new IndexViewModel { File = iFormFile.Object }) as ViewResult;
            var result = viewResult.Model as IndexViewModel;
            Assert.True(result.ImportErrors.Contains("you uploaded an empty file."));
        }

        [Fact]
        public void IndexPostSendsDuplicateProviderRequestIdsQueryWithCorrectProviderRequestIds()
        {
            const string id = "id";
            var importRequestViewModels = new List<ImportRequestViewModel> { new ImportRequestViewModel { Id = id } };

            var iFormFile = new Mock<IFormFile>();
            var csvFactory = new Mock<ICsvFactory>();
            var csvReader = new Mock<ICsvReader>();
            var mediator = new Mock<IMediator>();

            iFormFile.Setup(x => x.OpenReadStream()).Returns(new MemoryStream());

            csvFactory.Setup(x => x.CreateReader(It.IsAny<TextReader>())).Returns(csvReader.Object);

            csvReader.Setup(x => x.Configuration).Returns(new CsvConfiguration());
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());

            var sut = new ImportController(mediator.Object, null, csvFactory.Object);
            sut.Index(new IndexViewModel { File = iFormFile.Object });

            mediator.Verify(x => x.Send(It.Is<DuplicateProviderRequestIdsQuery>(y => y.ProviderRequestIds[0] == id)));
        }

        [Fact]
        public void IndexPostReturnsCorrectImportError_WhenThereAreDuplicateProviderRequestIdsFound()
        {
            const string duplicateId = "id";
            var duplicateIds = new List<string> { duplicateId };
            var importRequestViewModels = new List<ImportRequestViewModel> { new ImportRequestViewModel { Id = duplicateId } };

            var iFormFile = new Mock<IFormFile>();
            var csvFactory = new Mock<ICsvFactory>();
            var csvReader = new Mock<ICsvReader>();
            var mediator = new Mock<IMediator>();

            iFormFile.Setup(x => x.OpenReadStream()).Returns(new MemoryStream());

            csvFactory.Setup(x => x.CreateReader(It.IsAny<TextReader>())).Returns(csvReader.Object);

            csvReader.Setup(x => x.Configuration).Returns(new CsvConfiguration());
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);

            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(duplicateIds);

            var sut = new ImportController(mediator.Object, null, csvFactory.Object);
            var viewResult = sut.Index(new IndexViewModel { File = iFormFile.Object }) as ViewResult;
            var result = viewResult.Model as IndexViewModel;
            Assert.True(result.ImportErrors.Contains($"These id's already exist in the system. Please remove them from the CSV and try again: {string.Join(", ", duplicateIds)}"));
        }

        [Fact]
        public void IndexPostReturnsCorrectImportError_WhenThereAreValidationErrors()
        {
        }

        [Fact]
        public void IndexPostSendsImportRequestsCommandWithTheCorrectViewModel()
        {
        }

        [Fact]
        public void IndexPostLogsCorrectMessageWhenImportSucceeds()
        {
        }

        [Fact]
        public void IndexPostAssignsImportSuccessToTrueWhenImportSucceeds()
        {
        }

        //has [HttpPost] and [ValidateAntiForgeryToken] attbributes
    }
}