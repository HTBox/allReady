using AllReady.Areas.Admin.ViewModels.Task;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class MessageTaskVolunteersCommandAsync : IAsyncRequest
    {
        public MessageTaskVolunteersModel Model {get; set;}
    }
}
