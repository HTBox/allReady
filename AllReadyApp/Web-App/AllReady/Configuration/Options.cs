using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AllReady.Configuration
{
    internal static class Options
    {
        internal static void LoadConfigurationOptions(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AzureStorageSettings>(configuration.GetSection("Data:Storage"));
            services.Configure<DatabaseSettings>(configuration.GetSection("Data:DefaultConnection"));
            services.Configure<EmailSettings>(configuration.GetSection("Email"));
            services.Configure<SampleDataSettings>(configuration.GetSection("SampleData"));
            services.Configure<GeneralSettings>(configuration.GetSection("General"));
            services.Configure<GetASmokeAlarmApiSettings>(configuration.GetSection("GetASmokeAlarmApiSettings"));
            services.Configure<TwitterAuthenticationSettings>(configuration.GetSection("Authentication:Twitter"));
            services.Configure<TwilioSettings>(configuration.GetSection("Authentication:Twilio"));
            services.Configure<MappingSettings>(configuration.GetSection("Mapping"));
            services.Configure<ApprovedRegionsSettings>(settings => settings.Enabled = false);
            services.Configure<ApprovedRegionsSettings>(configuration.GetSection("ApprovedRegions"));
        }

    }
}
