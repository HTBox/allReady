using System;
using AllReady.DataAccess;
using AllReady.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AllReady.IntegrationTests
{
    public class IntegrationTestStartup : Startup
    {
        public IntegrationTestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void AddDatabaseServices(IServiceCollection services)
        {
            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<AllReadyContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemory");
                });
        }

        protected override void LoadSeedData(bool purgeRefreshSampleData, SampleDataGenerator sampleDataGenerator)
        {
            // tests will load their own data? Possibly have some default data instead
        }

        protected override void MigrateDatabase(bool purgeRefreshSampleData, IHostingEnvironment hostingEnvironment, AllReadyContext context)
        {
            // for now we load up a default set of data used by all tests - this works for now, but needs review

            var campaign = new Campaign
            {
                EndDateTime = DateTimeOffset.UtcNow.AddDays(10),
                Featured = true,
                Published = true,
                Locked = false,
                Name = "Featured Campaign Name",
                Description = "This is a featured campaign",
                Headline = "This is a featured headline",
                ManagingOrganization = new Organization
                {
                    Name = "Test Organisation"
                }
            };

            context.Campaigns.Add(campaign);

            context.Events.Add(new Event { Campaign = campaign, Name = "Event Name 1", EndDateTime = DateTimeOffset.UtcNow.AddDays(2) });
            context.Events.Add(new Event { Campaign = campaign, Name = "Event Name 2", EndDateTime = DateTimeOffset.UtcNow.AddDays(2) });

            context.SaveChanges();
        }

        protected override void AddHangFire(IServiceCollection services)
        {
            // do nothing for now - may need to be added later in some form
        }

        protected override void RegisterHangFire(IApplicationBuilder app)
        {
            // do nothing for now - may need to be added later in some form
        }
    }
}
