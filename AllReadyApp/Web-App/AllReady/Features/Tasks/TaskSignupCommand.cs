using AllReady.ViewModels.Shared;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskSignupCommand : IAsyncRequest<TaskSignupResult>
    {
        public TaskSignupViewModel TaskSignupModel { get; set; }
    }
}
