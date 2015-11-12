using AllReady.Areas.Admin.Models;
using MediatR;
 
namespace AllReady.Areas.Admin.Features.Tenants
{
    public class TenantEditCommand : IRequest<int>
    {
        public TenantEditModel Tenant { get; set; }
    }
}
