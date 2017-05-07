namespace AllReady.Models
{
    /// <summary>
    ///     Data class responsible for keeping the attachment information.
    ///     We want to keep this separate from the content so we can retrieve the file information without retrieving the actual contents of the file.
    /// </summary>
    public class FileAttachment
    {
        /// <summary>ID of the object</summary>
        public int Id { get; set; }

        /// <summary>Name of the file</summary>
        public string Name { get; set; }

        /// <summary>Description of the file</summary>
        public string Description { get; set; }
        
        /// <summary>URL to see or download the file contents</summary>
        public string Url { get; set; }

        /// <summary>The ID of the task where this file is attached to</summary>
        public int VolunteerTaskId { get; set; }

        /// <summary>Reference to the task where this file is attached to</summary>
        public VolunteerTask VolunteerTask { get; set; }
    }
}
