using AllReady.Models;
using AllReady.ViewModels;
using MediatR;
using Microsoft.Data.Entity;
using Microsoft.Extensions.WebEncoders;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AllReady.Features.Organizations
{
    public class OrganizationPrivacyPolicyQueryHandlerAsync : IAsyncRequestHandler<OrganziationPrivacyPolicyQueryAsync, OrganizationPrivacyPolicyViewModel>
    {
        private readonly AllReadyContext _context;

        public OrganizationPrivacyPolicyQueryHandlerAsync(AllReadyContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        public async Task<OrganizationPrivacyPolicyViewModel> Handle(OrganziationPrivacyPolicyQueryAsync message)
        {
            var encoder = new HtmlEncoder();

            return await _context.Organizations
                .AsNoTracking()
                .Where(t => t.Id == message.Id)
                .Select(t => new OrganizationPrivacyPolicyViewModel { OrganizationName = t.Name, Content = t.PrivacyPolicy })
                .SingleOrDefaultAsync();
        }
    }
}