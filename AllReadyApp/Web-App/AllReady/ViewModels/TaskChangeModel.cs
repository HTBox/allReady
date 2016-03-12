using AllReady.Areas.Admin.Features.Tasks;

namespace AllReady.ViewModels
{
    public class TaskChangeModel
    {
        public int TaskId { get; set; }
        public string UserId { get; set; }
        public string StatusDescription { get; set; }
        public TaskStatus Status { get; set; }
    }
}