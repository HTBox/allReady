using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AllReady.ViewModels.Organization;

namespace AllReady.Features.Organizations
{
    public class OrganizationPrivacyPolicyQueryHandlerAsync : IAsyncRequestHandler<OrganziationPrivacyPolicyQueryAsync, OrganizationPrivacyPolicyViewModel>
    {
        private readonly AllReadyContext _context;

        public OrganizationPrivacyPolicyQueryHandlerAsync(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<OrganizationPrivacyPolicyViewModel> Handle(OrganziationPrivacyPolicyQueryAsync message)
        {
            return await _context.Organizations
                .AsNoTracking()
                .Where(t => t.Id == message.OrganizationId)
                .Select(t => new OrganizationPrivacyPolicyViewModel { OrganizationName = t.Name, Content = t.PrivacyPolicy })
                .SingleOrDefaultAsync().ConfigureAwait(false);
        }
    }
}