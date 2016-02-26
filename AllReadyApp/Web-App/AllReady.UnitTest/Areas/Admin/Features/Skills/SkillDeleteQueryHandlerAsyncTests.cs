using AllReady.Areas.Admin.Features.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest.Areas.Admin.Features.Skills
{
    public class SkillDeleteQueryHandlerAsyncTests : InMemoryContextTest
    {
        [Fact]
        public async Task CorrectSkillReturnedWhenIdInMessage()
        {
            var handler = new SkillDeleteQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillDeleteQueryAsync { Id = 4 }).ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal("Skill 4", result.HierarchicalName);
            Assert.Equal(2, result.ChildrenNames.Count());
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdDoesNotExists()
        {
            var handler = new SkillDeleteQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillDeleteQueryAsync { Id = 100 }).ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task NullReturnedWhenSkillIdNotInMessage()
        {
            var handler = new SkillDeleteQueryHandlerAsync(Context);
            var result = await handler.Handle(new SkillDeleteQueryAsync()).ConfigureAwait(false);

            Assert.Null(result);
        }

        protected override void LoadTestData()
        {
            SkillsHandlerTestHelper.LoadSkillsHandlerTestData(Context);
        }
    }
}