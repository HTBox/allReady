using AllReady.Models;

namespace AllReady.UnitTest.Areas.Admin.Features.Skills
{
    public static class SkillsHandlerTestHelper
    {
        public static void LoadSkillsHandlerTestData(AllReadyContext context)
        {
            // Organizations
            context.Organizations.Add(new Organization { Id = 1, Name = "Org 1" });
            context.Organizations.Add(new Organization { Id = 2, Name = "Org 2" });

            // Skills
            context.Skills.Add(new Skill { Name = "Skill 1", Description = "Description 1", OwningOrganizationId = 1 });
            context.Skills.Add(new Skill { Name = "Skill 2", Description = "Description 2", OwningOrganizationId = 1 });
            context.Skills.Add(new Skill { Name = "Skill 3", Description = "Child of Skill 2", OwningOrganizationId = 1, ParentSkillId = 2 });

            context.Skills.Add(new Skill { Name = "Skill 4", Description = "Description 4", });
            context.Skills.Add(new Skill { Name = "Skill 5", Description = "Child of Skill 4", ParentSkillId = 4 });
            context.Skills.Add(new Skill { Name = "Skill 6", Description = "Child of skill 4", ParentSkillId = 4 });

            context.Skills.Add(new Skill { Name = "Skill 7", Description = "Description 7", OwningOrganizationId = 2 });

            context.SaveChanges();
        }
    }
}
