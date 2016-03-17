using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class MessageActivityVolunteersCommand : IAsyncRequest
    {
        public MessageActivityVolunteersModel Model {get; set;}
    }
}
