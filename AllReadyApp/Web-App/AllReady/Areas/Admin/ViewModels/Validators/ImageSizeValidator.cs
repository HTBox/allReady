using Microsoft.Extensions.Configuration;

namespace AllReady.Areas.Admin.ViewModels.Validators
{
    public class ImageSizeValidator : IImageSizeValidator
    {
        private const long DefaultImageSize = 1048576;

        public ImageSizeValidator(IConfiguration configuration)
        {
            try
            {
                FileSizeInBytes = configuration.GetValue<long>("General:MaxImageSizeInBytes");
            }
            catch
            {
                FileSizeInBytes = DefaultImageSize;

            }
        }

        public long FileSizeInBytes { get; private set; }

        public long BytesToMb()
        {
            return FileSizeInBytes / 1024 / 1024;
        }
    }

    public interface IImageSizeValidator
    {
        long FileSizeInBytes { get; }

        long BytesToMb();
    }
}
