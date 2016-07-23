using AllReady.Models;

namespace AllReady.ViewModels.Task
{
    public class SkillViewModel
    {
        public SkillViewModel() { }

        public SkillViewModel(Skill skill)
        {
            Id = skill.Id;
            Name = skill.Name;
            Description = skill.Description;
            HierarchicalName = skill.HierarchicalName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HierarchicalName { get; set; }
    }
}

