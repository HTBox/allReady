using AllReady.ViewModels.Shared;
using MediatR;

namespace AllReady.Features.Tasks
{
    public class VolunteerTaskSignupCommand : IAsyncRequest<VolunteerTaskSignupResult>
    {
        public VolunteerTaskSignupViewModel TaskSignupModel { get; set; }
    }
}
