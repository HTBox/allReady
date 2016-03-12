using AllReady.ViewModels;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class TaskSignupCommand : IRequest<TaskSignupResult>, IAsyncRequest<TaskSignupResult>
    {
        public ActivitySignupViewModel TaskSignupModel { get; set; }
    }
}
