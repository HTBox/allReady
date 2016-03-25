using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class MessageTaskVolunteersCommand : IAsyncRequest
    {
        public MessageTaskVolunteersModel Model {get; set;}
    }
}
