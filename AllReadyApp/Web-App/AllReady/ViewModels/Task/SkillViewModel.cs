using AllReady.Models;

namespace AllReady.ViewModels.Task
{
    public class SkillViewModel
    {
        public SkillViewModel() { }

        public SkillViewModel(Skill skill)
        {
            this.Id = skill.Id;
            this.Name = skill.Name;
            this.Description = skill.Description;
            this.HierarchicalName = skill.HierarchicalName;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HierarchicalName { get; set; }
    }
}

