using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Models
{
    /// <summary>
    ///     Data class responsible for keeping the attachment contents.
    ///     We want to keep this separate from the information about the file so we can retrieve that information without retrieving the actual contents of the file.
    /// </summary>
    public class FileAttachmentContent
    {
        /// <summary>ID of the object</summary>
        public int Id { get; set; }

        /// <summary>Actual file contents</summary>
        public byte[] Bytes { get; set; }
    }
}
