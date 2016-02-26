using AllReady.Areas.Admin.Features.Skills;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Skills
{
    public class SkillEditQueryHandlerAsyncTests : InMemoryContextTest
    {
        [Fact]
        public async Task CorrectSkillReturnedWhenIdInMessage()
        {
            var handler = new SkillEditQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillEditQueryAsync { Id = 4 }).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal("Skill 4", result.Name);
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdDoesNotExists()
        {
            var handler = new SkillEditQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillEditQueryAsync { Id = 100 }).ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdNotInMessage()
        {
            var handler = new SkillEditQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillEditQueryAsync()).ConfigureAwait(false);

            Assert.Null(result);
        }

        protected override void LoadTestData()
        {
            SkillsHandlerTestHelper.LoadSkillsHandlerTestData(Context);
        }
    }
}