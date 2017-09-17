using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using AllReady.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using AllReady.Security;

namespace AllReady.Services
{
    public class SelectListService : ISelectListService
    {
        private readonly AllReadyContext _context;

        public SelectListService(AllReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetOrganizations(ClaimsPrincipal user)
        {
            // Default to authorizing the return of no organizations
            var listOfOrganizations = new List<SelectListItem>();

            if (user.IsUserType(UserType.SiteAdmin))
            {
                listOfOrganizations = GetOrganizationsForSiteAdmin();
            }
            else if (user.IsUserType(UserType.OrgAdmin))
            {
                listOfOrganizations = GetOrganizationForOrgAdmin(user);
            }

            return listOfOrganizations;
        }

        private List<SelectListItem> GetOrganizationsForSiteAdmin()
        {
            return _context.Organizations.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name }).ToList();
        }

        private List<SelectListItem> GetOrganizationForOrgAdmin(ClaimsPrincipal user)
        {
            int? organizationId = user.GetOrganizationId();
            if (organizationId.HasValue)
            {
                return _context.Organizations.Where(o => o.Id == organizationId)
                    .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name })
                    .ToList();
            }

            return new List<SelectListItem>();
        }

        //TODO: this needs to be moved out of the SelecListService class b/c it does not return an IEnumerable<SelectListItem>
        public IEnumerable<Skill> GetSkills()
        {
            return _context.Skills.ToList()
                //Project HierarchicalName onto Name
                .Select(s => new Skill { Id = s.Id, Name = s.HierarchicalName, Description = s.Description })
                .OrderBy(s => s.Name);
        }

        public IEnumerable<SelectListItem> GetCampaignGoalTypes()
        {
            return new List<SelectListItem> {
                new SelectListItem { Value = ((int)GoalType.Text).ToString(), Text = GoalType.Text.GetDisplayName() },
                new SelectListItem { Value = ((int)GoalType.Numeric).ToString(), Text = GoalType.Numeric.GetDisplayName() }
            };
        }

        public IEnumerable<SelectListItem> GetRequestTypes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem {Text = RequestStatus.Assigned.GetDisplayName(), Value = ((int)RequestStatus.Assigned).ToString()},
                new SelectListItem {Text = RequestStatus.Canceled.GetDisplayName(), Value = ((int)RequestStatus.Canceled).ToString()},
                new SelectListItem {Text = RequestStatus.Completed.GetDisplayName(), Value = ((int)RequestStatus.Completed).ToString()},
                new SelectListItem {Text = RequestStatus.Confirmed.GetDisplayName(), Value = ((int)RequestStatus.Confirmed).ToString()},
                new SelectListItem {Text = RequestStatus.PendingConfirmation.GetDisplayName(), Value = ((int)RequestStatus.PendingConfirmation).ToString()},
                new SelectListItem {Text = RequestStatus.Requested.GetDisplayName(), Value = ((int)RequestStatus.Requested).ToString()},
            };
        }

        public IEnumerable<SelectListItem> GetTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().Select(t => new SelectListItem { Value = t.Id, Text = t.DisplayName });
        }
    }

    public static class SelectListExtensions
    {
        public static IEnumerable<SelectListItem> AddNullOptionToFront(this IEnumerable<SelectListItem> items, string text = "<None>", string value = "")
        {
            var list = items.ToList();
            list.Insert(0, new SelectListItem { Text = text, Value = value });
            return list;
        }

        public static IEnumerable<SelectListItem> AddNullOptionToEnd(this IEnumerable<SelectListItem> items, string text = "<None>", string value = "")
        {
            var list = items.ToList();
            list.Add(new SelectListItem { Text = text, Value = value });
            return list;
        }
    }
}
