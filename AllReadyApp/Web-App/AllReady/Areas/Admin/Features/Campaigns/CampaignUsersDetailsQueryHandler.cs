using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AllReady.Areas.Admin.ViewModels.Campaign;
using AllReady.Areas.Admin.Extensions;
using AllReady.Models;
using Microsoft.EntityFrameworkCore.Internal;

namespace AllReady.Areas.Admin.Features.Campaigns
{
    public class CampaignUsersDetailsQueryHandler : IAsyncRequestHandler<CampaignUsersDetailsQuery, CampaignUsersDetailsViewModel>
    {
        private readonly AllReadyContext _context;

        public CampaignUsersDetailsQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<CampaignUsersDetailsViewModel> Handle(CampaignUsersDetailsQuery message)
        {
            var result = await _context.Campaigns
                .AsNoTracking()
                .Include(vm => vm.Events)
                .Select(vm => new CampaignUsersDetailsViewModel
                {
                    CampaignId = vm.Id,
                    CampaignName = vm.Name,
                    OrganizationId = vm.ManagingOrganizationId,
                    CampaignManagerList = vm.CampaignManagers
                    .Select(cm => new CampaignUserViewModel
                    {
                        Name = cm.User.Name,
                        UserName = cm.User.UserName
                    })
                    .ToList(),
                    EventManagerList = vm.Events
                    .SelectMany(e => e.EventManagers)
                    .Select(em => new CampaignUserViewModel
                    {
                        Name = em.User.Name,
                        UserName = em.User.UserName
                    })
                    .ToList(),
                    VolunteerList = vm.Events
                    .SelectMany(e => e.VolunteerTasks)
                    .SelectMany(vt => vt.AssignedVolunteers)
                    .Select(v => new CampaignUserViewModel
                    {
                        Name = v.User.Name,
                        UserName = v.User.UserName
                    })
                    .OrderBy(u => u.Name)
                    .GroupBy(u => u.Name)
                    .Select(g => g.First())
                    .ToList()
                })
                .SingleOrDefaultAsync(c => c.CampaignId == message.CampaignId);

            return result;
        }
    }
}
