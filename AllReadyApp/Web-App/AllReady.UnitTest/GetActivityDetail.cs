using AllReady.Areas.Admin.Features.Activities;
using AllReady.Models;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AllReady.UnitTest
{
    public class GetActivityDetail : TestBase
    {
        protected override void LoadTestData()
        {
            var allReadyContext = ServiceProvider.GetService<AllReadyContext>();
            allReadyContext.Activities.Add(new Activity { Id = 1 });
        }

        [Fact]
        public void ActivityExists()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new ActivityDetailQuery { ActivityId = 1 };
            var handler = new ActivityDetailQueryHandler(context);
            var result = handler.Handle(query);
            Assert.Null(result);
        }

        [Fact]
        public void ActivityDoesNotExist()
        {
            var context = ServiceProvider.GetService<AllReadyContext>();
            var query = new ActivityDetailQuery();
            var handler = new ActivityDetailQueryHandler(context);
            var result = handler.Handle(query);
            Assert.Null(result);
        }

    }
}
