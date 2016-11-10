using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using AllReady.ViewModels.Organization;

namespace AllReady.Features.Organizations
{
    public class OrganizationPrivacyPolicyQueryHandler : IAsyncRequestHandler<OrganizationPrivacyPolicyQuery, OrganizationPrivacyPolicyViewModel>
    {
        private readonly AllReadyContext _context;

        public OrganizationPrivacyPolicyQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<OrganizationPrivacyPolicyViewModel> Handle(OrganizationPrivacyPolicyQuery message)
        {
            return await _context.Organizations
                .AsNoTracking()
                .Where(t => t.Id == message.OrganizationId)
                .Select(t => new OrganizationPrivacyPolicyViewModel { OrganizationName = t.Name, Content = t.PrivacyPolicy })
                .SingleOrDefaultAsync();
        }
    }
}