using AllReady.Areas.Admin.ViewModels.Event;
using MediatR;

namespace AllReady.Areas.Admin.Features.Events
{
    public class MessageEventVolunteersCommand : IAsyncRequest
    {
        public MessageEventVolunteersModel Model {get; set;}
    }
}
