using AllReady.Areas.Admin.ViewModels;
using MediatR;

namespace AllReady.Areas.Admin.Features.Activities
{
    public class MessageActivityVolunteersCommand : IRequest
    {
        public MessageActivityVolunteersModel Model {get; set;}
    }
}
