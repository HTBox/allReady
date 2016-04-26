using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskSignupCommand : IAsyncRequest<TaskSignupResult>
    {
        public EventSignupViewModel TaskSignupModel { get; set; }
    }
}
