using AllReady.Areas.Admin.Features.Skills;
using System.Threading.Tasks;
using AllReady.UnitTest.Features.Campaigns;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Skills
{
    public class SkillEditQueryHandlerAsyncTests : InMemoryContextTest
    {
        [Fact(Skip = "RTM Broken Tests")]
        public async Task CorrectSkillReturnedWhenIdInMessage()
        {
            var handler = new SkillEditQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillEditQueryAsync { Id = 4 });

            Assert.NotNull(result);
            Assert.Equal("Skill 4", result.Name);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task NullReturnedWhenSkillIdDoesNotExists()
        {
            var handler = new SkillEditQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillEditQueryAsync { Id = 100 });

            Assert.Null(result);
        }

        [Fact(Skip = "RTM Broken Tests")]
        public async Task NullReturnedWhenSkillIdNotInMessage()
        {
            var handler = new SkillEditQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillEditQueryAsync());

            Assert.Null(result);
        }

        protected override void LoadTestData()
        {
            SkillsHandlerTestHelper.LoadSkillsHandlerTestData(Context);
        }
    }
}