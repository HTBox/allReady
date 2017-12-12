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
            // do not migrate in memory
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
