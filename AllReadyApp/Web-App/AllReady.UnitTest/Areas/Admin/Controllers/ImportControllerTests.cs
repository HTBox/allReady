﻿using System;
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
        public void IndexPostReturnsCorrectImportError_WhenEventIsNotPicked()
        {
            var sut = new ImportController(null, null, null);
            var result = (IndexViewModel)((ViewResult)sut.Index(new IndexViewModel())).Model;

            Assert.True(result.ImportErrors.Contains("please select an Event."));
        }

        [Fact]
        public void IndexPostReturnsCorrectImportError_WhenNoFileIsUploaded()
        {
            var sut = new ImportController(null, null, null);
            var result = (IndexViewModel)((ViewResult)sut.Index(new IndexViewModel())).Model;

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
            var result = (IndexViewModel)((ViewResult)sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object })).Model;

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
            sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object });

            mediator.Verify(x => x.Send(It.Is<DuplicateProviderRequestIdsQuery>(y => y.ProviderRequestIds[0] == id)), Times.Once);
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
            var result = (IndexViewModel)((ViewResult)sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object })).Model;

            Assert.True(result.ImportErrors.Contains($"These id's already exist in the system. Please remove them from the CSV and try again: {string.Join(", ", duplicateIds)}"));
        }

        [Fact]
        public void IndexPostReturnsCorrectImportError_WhenThereAreValidationErrors()
        {
            var importRequestViewModels = new List<ImportRequestViewModel>
            {
                new ImportRequestViewModel { Id =  "Id", Name = null, Address = string.Empty, Email = "InvalidEmail" }
            };

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
            var viewResult = sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object }) as ViewResult;
            var result = viewResult.Model as IndexViewModel;

            Assert.Equal(result.ValidationErrors[0].ProviderRequestId, importRequestViewModels[0].Id);
            Assert.Equal(result.ValidationErrors[0].Errors[0].ErrorMessage, "The Name field is required.");
            Assert.Equal(result.ValidationErrors[0].Errors[1].ErrorMessage, "The Address field is required.");
            Assert.Equal(result.ValidationErrors[0].Errors[2].ErrorMessage, "The City field is required.");
            Assert.Equal(result.ValidationErrors[0].Errors[3].ErrorMessage, "The State field is required.");
            Assert.Equal(result.ValidationErrors[0].Errors[4].ErrorMessage, "The Zip field is required.");
            Assert.Equal(result.ValidationErrors[0].Errors[5].ErrorMessage, "The Phone field is required.");
            Assert.Equal(result.ValidationErrors[0].Errors[6].ErrorMessage, "Invalid Email Address");
        }

        [Fact]
        public void IndexPostSendsImportRequestsCommandWithTheCorrectViewModel()
        {
            var importRequestViewModels = new List<ImportRequestViewModel>
            {
                new ImportRequestViewModel { Id =  "Id", Name = "Name", Address = "Address", City = "City", Email = "email@email.com", Phone = "111-111-1111", State = "State", Zip = "Zip" }
            };

            var iFormFile = new Mock<IFormFile>();
            var csvFactory = new Mock<ICsvFactory>();
            var csvReader = new Mock<ICsvReader>();
            var mediator = new Mock<IMediator>();

            iFormFile.Setup(x => x.OpenReadStream()).Returns(new MemoryStream());
            iFormFile.Setup(x => x.Name).Returns(It.IsAny<string>());
            csvFactory.Setup(x => x.CreateReader(It.IsAny<TextReader>())).Returns(csvReader.Object);
            csvReader.Setup(x => x.Configuration).Returns(new CsvConfiguration());
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());

            var sut = new ImportController(mediator.Object, Mock.Of<ILogger<ImportController>>(), csvFactory.Object);
            sut.SetFakeUserName("UserName");
            sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object });

            mediator.Verify(x => x.Send(It.Is<ImportRequestsCommand>(y => 
                y.ImportRequestViewModels[0].Id == importRequestViewModels[0].Id &&
                y.ImportRequestViewModels[0].Name == importRequestViewModels[0].Name &&
                y.ImportRequestViewModels[0].Address == importRequestViewModels[0].Address &&
                y.ImportRequestViewModels[0].City == importRequestViewModels[0].City &&
                y.ImportRequestViewModels[0].Email == importRequestViewModels[0].Email &&
                y.ImportRequestViewModels[0].Phone == importRequestViewModels[0].Phone &&
                y.ImportRequestViewModels[0].State == importRequestViewModels[0].State &&
                y.ImportRequestViewModels[0].Zip == importRequestViewModels[0].Zip &&
                y.ImportRequestViewModels[0].Longitude == 0 &&
                y.ImportRequestViewModels[0].Latitude == 0 &&
                y.ImportRequestViewModels[0].ProviderData == null)), Times.Once);
        }

        [Fact]
        public void IndexPostLogsCorrectMessage_WhenImportSucceeds()
        {
            const string userName = "UserName";
            const string fileName = "FileName";

            var importRequestViewModels = new List<ImportRequestViewModel>
            {
                new ImportRequestViewModel { Id =  "Id", Name = "Name", Address = "Address", City = "City", Email = "email@email.com", Phone = "111-111-1111", State = "State", Zip = "Zip" }
            };

            var iFormFile = new Mock<IFormFile>();
            var csvFactory = new Mock<ICsvFactory>();
            var csvReader = new Mock<ICsvReader>();
            var mediator = new Mock<IMediator>();
            var logger = new Mock<ILogger<ImportController>>();

            iFormFile.Setup(x => x.OpenReadStream()).Returns(new MemoryStream());
            iFormFile.Setup(x => x.Name).Returns(fileName);
            csvFactory.Setup(x => x.CreateReader(It.IsAny<TextReader>())).Returns(csvReader.Object);
            csvReader.Setup(x => x.Configuration).Returns(new CsvConfiguration());
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());

            var sut = new ImportController(mediator.Object, logger.Object, csvFactory.Object);
            sut.SetFakeUserName(userName);
            sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object });

            logger.Verify(m => m.Log(LogLevel.Information, It.IsAny<EventId>(),
            It.Is<FormattedLogValues>(v => v.ToString() == $"{userName} imported file {fileName}"),
            null, It.IsAny<Func<object, Exception, string>>()), Times.Once);
        }

        [Fact]
        public void IndexPostAssignsImportSuccessToTrue_WhenImportSucceeds()
        {
            var importRequestViewModels = new List<ImportRequestViewModel>
            {
                new ImportRequestViewModel { Id =  "Id", Name = "Name", Address = "Address", City = "City", Email = "email@email.com", Phone = "111-111-1111", State = "State", Zip = "Zip" }
            };

            var iFormFile = new Mock<IFormFile>();
            var csvFactory = new Mock<ICsvFactory>();
            var csvReader = new Mock<ICsvReader>();
            var mediator = new Mock<IMediator>();

            iFormFile.Setup(x => x.OpenReadStream()).Returns(new MemoryStream());
            iFormFile.Setup(x => x.Name).Returns(It.IsAny<string>());
            csvFactory.Setup(x => x.CreateReader(It.IsAny<TextReader>())).Returns(csvReader.Object);
            csvReader.Setup(x => x.Configuration).Returns(new CsvConfiguration());
            csvReader.Setup(x => x.GetRecords<ImportRequestViewModel>()).Returns(importRequestViewModels);
            mediator.Setup(x => x.Send(It.IsAny<DuplicateProviderRequestIdsQuery>())).Returns(new List<string>());

            var sut = new ImportController(mediator.Object, Mock.Of<ILogger<ImportController>>(), csvFactory.Object);
            sut.SetFakeUserName("UserName");
            var result = (IndexViewModel)((ViewResult)sut.Index(new IndexViewModel { EventId = 1, File = iFormFile.Object })).Model;

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
    }
}