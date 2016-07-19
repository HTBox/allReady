﻿using System;
using System.Collections.Generic;
using System.Linq;
using AllReady.Extensions;
using AllReady.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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
            return _context.Organizations.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name });
        }

        //TODO: this needs to be moved out of the SelecListService class b/c it does not return an IEnumerable<SelectListItem>
        public IEnumerable<Skill> GetSkills()
        {
            return _context.Skills.ToList()
                //Project HierarchicalName onto Name
                .Select(s => new Skill { Id = s.Id, Name = s.HierarchicalName, Description = s.Description })
                .OrderBy(s => s.Name);
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
