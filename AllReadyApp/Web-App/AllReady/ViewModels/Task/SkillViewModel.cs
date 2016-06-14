using AllReady.Models;

namespace AllReady.ViewModels.Task
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

