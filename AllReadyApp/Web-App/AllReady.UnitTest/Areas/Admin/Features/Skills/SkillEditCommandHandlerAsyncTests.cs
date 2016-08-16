using AllReady.Areas.Admin.Features.Skills;
using System.Linq;
using System.Threading.Tasks;
using AllReady.Areas.Admin.ViewModels.Skill;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Skills
{
    public class SkillEditCommandHandlerAsyncTests : InMemoryContextTest
    {
        [Fact]
        public async Task UpdatingExistingSkill()
        {
            // Arrange
            var handler = new SkillEditCommandHandlerAsync(Context);
            var newSkill = new SkillEditViewModel { Id = 2, Name = "New", Description = "Desc", OwningOrganizationId = 1 };

            // Act
            var result = await handler.Handle(new SkillEditCommandAsync { Skill = newSkill });
            var savedSkill = Context.Skills.SingleOrDefault(s => s.Id == 2);

            // Assert
            Assert.Equal(7, Context.Skills.Count());
            Assert.Equal(2, result);
            Assert.Equal("New", savedSkill.Name);
        }

        [Fact]
        public async Task AddNewSkill()
        {
            // Arrange
            var handler = new SkillEditCommandHandlerAsync(Context);
            var newSkill = new SkillEditViewModel { Name = "New", Description = "Desc" };

            // Act
            var result = await handler.Handle(new SkillEditCommandAsync { Skill = newSkill });

            // Assert
            Assert.Equal(8, Context.Skills.Count());
            Assert.Equal(8, result);
        }

        protected override void LoadTestData()
        {
            SkillsHandlerTestHelper.LoadSkillsHandlerTestData(Context);
        }
    }
}