using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskSignupCommand : IAsyncRequest<TaskSignupResult>
    {
        public ActivitySignupViewModel TaskSignupModel { get; set; }
    }
}
