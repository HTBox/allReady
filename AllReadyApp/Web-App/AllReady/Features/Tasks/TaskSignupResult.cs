using AllReady.Models;

namespace AllReady.Features.Tasks
{
    public class TaskSignupResult
    {
        public string Status { get; set; }
        public AllReadyTask Task { get; set; }
    }
}