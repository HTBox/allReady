using AllReady.Areas.Admin.ViewModels.Task;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    /// <summary>Query for downloading attachments</summary>
    public class DownloadAttachmentQuery : IAsyncRequest<FileAttachmentModel>
    {
        public int AttachmentId { get; set; }
    }
}
