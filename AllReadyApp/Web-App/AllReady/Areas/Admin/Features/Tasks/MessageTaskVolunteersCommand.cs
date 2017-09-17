using AllReady.Areas.Admin.ViewModels.VolunteerTask;

using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class MessageTaskVolunteersCommand : IAsyncRequest
    {
        public MessageTaskVolunteersViewModel Model {get; set;}
    }
}
