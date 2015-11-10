using AllReady.Areas.Admin.Models;
using MediatR;
 
namespace AllReady.Areas.Admin.Features.Tenants
{
    public class EditTenantCommand : IRequest<int>
    {
        public TenantEditModel Tenant { get; set; }
    }
}
