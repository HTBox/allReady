using AllReady.Areas.Admin.Features.Skills;
using AllReady.Areas.Admin.Models;
using System.Linq;
using System.Threading.Tasks;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Skills
{
    public class SkillEditCommandHandlerAsyncTests : InMemoryContextTest
    {
        [Fact(Skip = "RTM Broken Tests")]
        public async Task UpdatingExistingSkill()
        {
            // Arrange
            var handler = new SkillEditCommandHandlerAsync(Context);
            var newSkill = new SkillEditModel { Id = 2, Name = "New", Description = "Desc", OwningOrganizationId = 1 };

            // Act
            var result = await handler.Handle(new SkillEditCommandAsync { Skill = newSkill });
            var savedSkill = Context.Skills.SingleOrDefault(s => s.Id == 2);

            // Assert
            Assert.Equal(7, Context.Skills.Count());
            Assert.Equal(2, result);
            Assert.Equal("New", savedSkill.Name);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task AddNewSkill()
        {
            // Arrange
            var handler = new SkillEditCommandHandlerAsync(Context);
            var newSkill = new SkillEditModel { Name = "New", Description = "Desc" };

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