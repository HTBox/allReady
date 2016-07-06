using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AllReady.Extensions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace AllReady.UnitTest.Extensions
{
    public class FormFileExtensionTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("image/pjpeg")]
        [InlineData("image/x-png")]
        [InlineData("image/tiff")]
        [InlineData("image/bmp")]
        [InlineData("image/x-xbitmap")]
        [InlineData("image/x-jg")]
        [InlineData("image/x-emf")]
        [InlineData("image/x-wmf")]
        public void UnacceptableImageFileFormat(string fileType)
        {
            var file = FormFile(fileType);
            Assert.False(file.IsAcceptableImageContentType());
        }

        [Theory]
        [InlineData("image/png")]
        [InlineData("image/jpeg")]
        [InlineData("image/gif")]
        public void AcceptableImageFileFormat(string fileType)
        {
            var file = FormFile(fileType);
            Assert.True(file.IsAcceptableImageContentType());
            file = FormFile(fileType.ToUpperInvariant());
            Assert.True(file.IsAcceptableImageContentType());
        }

        private static IFormFile FormFile(string fileType)
        {
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(mock => mock.ContentType).Returns(fileType);
            return mockFormFile.Object;
        }
    }
}
