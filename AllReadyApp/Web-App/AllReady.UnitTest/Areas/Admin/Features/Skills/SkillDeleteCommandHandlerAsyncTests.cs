using AllReady.Areas.Admin.Features.Skills;
using System.Linq;
using System.Threading.Tasks;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Skills
{
    public class SkillDeleteCommandHandlerAsyncTests : InMemoryContextTest
    {
        [Fact]
        public async Task DeleteCommandRemovesSkill()
        {
            // Arrange
            var handler = new SkillDeleteCommandHandlerAsync(Context);

            // Act
            await handler.Handle(new SkillDeleteCommandAsync { Id = 1 });

            // Assert
            Assert.Equal(6, Context.Skills.Count());
        }

        protected override void LoadTestData()
        {
            SkillsHandlerTestHelper.LoadSkillsHandlerTestData(Context);
        }
    }
}