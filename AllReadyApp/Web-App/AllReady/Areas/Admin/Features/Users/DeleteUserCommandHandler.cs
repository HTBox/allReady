using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Features.Users
{
    public class DeleteUserCommandHandler : RequestHandler<DeleteUserCommand>
    {
        protected override void HandleCore(DeleteUserCommand message)
        {
            throw new NotImplementedException();
        }
    }
}
