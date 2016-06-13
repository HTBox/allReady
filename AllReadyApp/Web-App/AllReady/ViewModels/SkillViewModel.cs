using AllReady.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.ViewModels
{
    public class SkillViewModel
    {
        public SkillViewModel() { }

        public SkillViewModel(Skill skill)
        {
            this.Name = skill.Name;
            this.Description = skill.Description;
            this.HierarchicalName = skill.HierarchicalName;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string HierarchicalName { get; set; }
    }
}

