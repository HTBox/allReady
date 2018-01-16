using System;

namespace AllReady.Models
{
    /// <summary>
    /// Represents a comment/update on a <see cref="Request"/> from an authorized user.
    /// </summary>
    public class RequestComment
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        public Guid RequestId { get; set; }

        public Request Request { get; set; }

        public DateTime CreatedUtcDate { get; set; }

        public string Comment { get; set; }

        // the specifications currently do not state anything about edits/deletes
        // if that is a future requirement we would probably want a soft delete or event source stream for comments
    }
}
