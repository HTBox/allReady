using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.Models;
using MediatR;

namespace AllReady.Areas.Admin.Features.Users
{
    public class UserQuery : IRequest<EditUserModel>
    {
        public string UserId { get; set; }
    }
}
