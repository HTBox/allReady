using AllReady.Areas.Admin.ViewModels.Site;
using MediatR;

namespace AllReady.Areas.Admin.Features.Users
{
    public class UserQuery : IAsyncRequest<EditUserModel>
    {
        public string UserId { get; set; }
    }
}
