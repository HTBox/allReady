using System.ComponentModel.DataAnnotations;

namespace AllReady.Areas.Admin.ViewModels.VolunteerTask
{
    public class MessageTaskVolunteersViewModel
    {
        public int VolunteerTaskId { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
