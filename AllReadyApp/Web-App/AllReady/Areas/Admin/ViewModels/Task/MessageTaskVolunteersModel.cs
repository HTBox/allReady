using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.Models
{
    public class MessageTaskVolunteersModel
    {
        public int TaskId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
