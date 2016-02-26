using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Extensions;
using AllReady.Models;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;

namespace AllReady.Services
{
    public class SelectListService : ISelectListService
    {
        private readonly AllReadyContext _context;

        public SelectListService(AllReadyContext context)
        {
            _context = context;
        }

        public IEnumerable<SelectListItem> GetOrganizations()
        {
            return _context.Organizations.Select(t => new SelectListItem {Value = t.Id.ToString(), Text = t.Name });
        }

        public async Task<IEnumerable<SelectListItem>> GetOrganizationsAsync()
        {
            return await _context.Organizations
                        .Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name })
                        .ToListAsync();
        }

        public IEnumerable<Skill> GetSkills()
        {
            return _context.Skills.ToList()
                //Project HierarchicalName onto Name
                .Select(s => new Skill { Id = s.Id, Name = s.HierarchicalName, Description = s.Description })
                .OrderBy(s => s.Name);
        }

        public async Task<IEnumerable<Skill>> GetSkillsAsync()
        {
            return await _context.Skills.AsNoTracking()
                //Project HierarchicalName onto Name
                .Select(s => new Skill { Id = s.Id, Name = s.HierarchicalName, Description = s.Description })
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        public IEnumerable<SelectListItem> GetCampaignImpactTypes()
        {
            return new List<SelectListItem> {
                new SelectListItem { Value = ((int)ImpactType.Text).ToString(), Text = ImpactType.Text.GetDisplayName() },
                new SelectListItem { Value = ((int)ImpactType.Numeric).ToString(), Text = ImpactType.Numeric.GetDisplayName() }
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
            list.Insert(0, new SelectListItem() { Text = text, Value = value });
            return list;
        }

        public static IEnumerable<SelectListItem> AddNullOptionToEnd(this IEnumerable<SelectListItem> items, string text = "<None>", string value = "")
        {
            var list = items.ToList();
            list.Add(new SelectListItem() { Text = text, Value = value });
            return list;
        }
    }
}
