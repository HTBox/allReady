using AllReady.Models;

namespace AllReady.Areas.Admin.Features.Tasks
{
   public class VolunteerTaskChangeResult
    {
        public string Status { get; set; }
        public VolunteerTask VolunteerTask { get; set; }
    }
}