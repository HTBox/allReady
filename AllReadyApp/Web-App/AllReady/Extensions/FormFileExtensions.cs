using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Extensions
{
    public static class FormFileExtensions
    {
        /// <summary>
        /// IFormFile extension that indicates whether the uploaded
        /// file is an acceptable image type based on the ContentType
        /// property. Acceptable image types are ones determined to
        /// be compatible with standard html rendering.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsAcceptableImageContentType(this IFormFile file)
        {
            bool isImageContentType = false;
            var contentType = file.ContentType.ToLowerInvariant();

            if (contentType == "image/png" ||
                contentType == "image/jpeg" ||
                contentType == "image/gif")
            {
                isImageContentType = true;
            }

            return isImageContentType;
        }
    }
}
