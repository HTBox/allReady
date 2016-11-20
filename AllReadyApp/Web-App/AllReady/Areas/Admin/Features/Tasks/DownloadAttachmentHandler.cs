using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Task;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Tasks
{
    /// <summary>Handler for downloading attachments.</summary>
    public class DownloadAttachmentHandler : IAsyncRequestHandler<DownloadAttachmentQuery, FileAttachmentModel>
    {
        private readonly AllReadyContext _context;
        
        public DownloadAttachmentHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<FileAttachmentModel> Handle(DownloadAttachmentQuery message)
        {
            var file = await GetFile(message);

            var model = new FileAttachmentModel
            {
                Id = file.Id,
                Name = file.Name,
                MimeType = file.MimeType,
                Content = file.Content.Bytes,
            };

            return model;
        }

        private async Task<FileAttachment> GetFile(DownloadAttachmentQuery message)
        {
            return await _context.Attachments
                .AsNoTracking()
                .Include(f => f.Content)
                .SingleOrDefaultAsync(f => f.Id == message.AttachmentId);
        }
    }
}