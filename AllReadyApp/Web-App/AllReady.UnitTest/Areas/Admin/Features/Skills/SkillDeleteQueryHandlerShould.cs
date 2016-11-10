using AllReady.Areas.Admin.Features.Skills;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Skills
{
    public class SkillDeleteQueryHandlerShould : InMemoryContextTest
    {
        [Fact]
        public async Task CorrectSkillReturnedWhenIdInMessage()
        {
            var handler = new SkillDeleteQueryHandler(Context);
            var result = await handler.Handle(new SkillDeleteQuery { Id = 4 });

            Assert.NotNull(result);
            Assert.Equal("Skill 4", result.HierarchicalName);
            Assert.Equal(2, result.ChildrenNames.Count());
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdDoesNotExists()
        {
            var handler = new SkillDeleteQueryHandler(Context);
            var result = await handler.Handle(new SkillDeleteQuery { Id = 100 });

            Assert.Null(result);
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdNotInMessage()
        {
            var handler = new SkillDeleteQueryHandler(Context);
            var result = await handler.Handle(new SkillDeleteQuery());

            Assert.Null(result);
        }

        protected override void LoadTestData()
        {
            SkillsHandlerTestHelper.LoadSkillsHandlerTestData(Context);
        }
    }
}