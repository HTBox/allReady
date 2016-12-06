using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Task
{
    /// <summary>Model for file attachments</summary>
    public class FileAttachmentModel
    {
        /// <summary>ID of the file</summary>
        public int Id { get; set; }

        /// <summary>Name of the file</summary>
        [Display(Name = "Attachment Name")]
        public string Name { get; set; }

        /// <summary>Description of the file</summary>
        [Display(Name = "Attachment Description")]
        public string Description { get; set; }

        /// <summary>MIME type of the file</summary>
        public string MimeType { get; set; }

        /// <summary>URL to download the file contents</summary>
        public string DownloadUrl { get; set; }

        /// <summary>File content</summary>
        public byte[] Content { get; set; }
    }
}
