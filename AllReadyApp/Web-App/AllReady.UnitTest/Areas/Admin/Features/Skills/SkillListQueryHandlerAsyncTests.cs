using AllReady.Areas.Admin.Features.Skills;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Skills
{
    public class SkillListQueryHandlerAsyncTests : InMemoryContextTest
    {
        [Fact]
        public async Task AllSkillsReturnedWhenNoIdInMessage()
        {
            var handler = new SkillListQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillListQueryAsync());

            Assert.Equal(7, result.Count());
        }

        [Fact]
        public async Task CorrectSkillsReturnedWhenIdInMessage()
        {
            var handler = new SkillListQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillListQueryAsync { OrganizationId = 1 });

            Assert.Equal(3, result.Count());
        }

        protected override void LoadTestData()
        {
            SkillsHandlerTestHelper.LoadSkillsHandlerTestData(Context);
        }
    }
}