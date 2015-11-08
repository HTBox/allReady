using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Tasks
{
    public class MessageTaskVolunteersCommand : IRequest
    {
        public MessageTaskVolunteersModel Model {get; set;}
    }
}
