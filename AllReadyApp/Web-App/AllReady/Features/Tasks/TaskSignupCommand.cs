using AllReady.ViewModels.Shared;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskSignupCommandAsync : IAsyncRequest<TaskSignupResult>
    {
        public TaskSignupViewModel TaskSignupModel { get; set; }
    }
}
