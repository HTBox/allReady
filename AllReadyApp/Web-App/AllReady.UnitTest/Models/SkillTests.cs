using AllReady.Models;
using Shouldly;
using Xunit;

namespace AllReady.UnitTest.ModelTests
{
    public class SkillTests
    {
        [Fact]
        public void HierarchicalName_ShouldNever_IterateMoreThan10_LayersOfHierarchy()
        {
            // DESCRIPTION: It's possible to get an infinite loop with the data so we need to ensure we short circuit that when evaluating the hierarchical name
            // NOTE: sgordon - If the code we're testing still creates a loop this will cause this test to also run infinitely

            // Arrange
            var parentSkill = new Skill {Name = "Parent", Id = 1 };
            var childSkill = new Skill {Name = "Child Skill", ParentSkill = parentSkill, Id = 2, ParentSkillId = 1 };
            parentSkill.ParentSkill = childSkill;

            // Act

            var parentResult = parentSkill.HierarchicalName;
            var childResult = childSkill.HierarchicalName;

            // Assert
            parentResult.ShouldBe(Skill.InvalidHierarchy);
            childResult.ShouldBe(Skill.InvalidHierarchy);
        }
    }
}
