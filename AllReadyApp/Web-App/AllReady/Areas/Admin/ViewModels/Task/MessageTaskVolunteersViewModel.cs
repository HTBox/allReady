using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.Task
{
    public class MessageTaskVolunteersViewModel
    {
        public int TaskId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
