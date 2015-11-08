using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class MessageActivityVolunteersModel
    {
        public int ActivityId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
