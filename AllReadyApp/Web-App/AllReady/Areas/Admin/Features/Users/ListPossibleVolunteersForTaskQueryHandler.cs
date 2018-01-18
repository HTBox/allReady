using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.VolunteerTask;
using AllReady.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Areas.Admin.Features.Users
{
    public class ListPossibleVolunteersForTaskQueryHandler : IAsyncRequestHandler<ListPossibleVolunteersForTaskQuery, IReadOnlyList<VolunteerSummary>>
    {
        private readonly AllReadyContext _context;

        public ListPossibleVolunteersForTaskQueryHandler(AllReadyContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<VolunteerSummary>> Handle(ListPossibleVolunteersForTaskQuery message)
        {
            var ourUsers = _context.VolunteerTaskSignups
                .Where(x => x.VolunteerTaskId == message.TaskId)
                .Select(x => x.User.Id)
                .ToList();

            var items = await (from x in _context.VolunteerTaskSignups
                    join user in _context.Users on x.User equals user
                    where x.VolunteerTask.Organization.Id == message.OrganizationId
                          && !ourUsers.Contains(x.User.Id)
                    group x.User by new {x.User.Id, x.User.Name}
                    into g
                    select new VolunteerSummary
                    {
                        TaskCount = g.Count(),
                        UserId = g.Key.Id,
                        Name = g.Key.Name
                    })
                .ToListAsync();

            return items;
        }
    }
}
