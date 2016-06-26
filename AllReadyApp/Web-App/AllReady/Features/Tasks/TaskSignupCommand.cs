using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskSignupCommandAsync : IAsyncRequest<TaskSignupResult>
    {
        public EventSignupViewModel TaskSignupModel { get; set; }
    }
}
