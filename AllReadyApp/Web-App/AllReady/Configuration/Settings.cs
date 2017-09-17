using System.Collections.Generic;

namespace AllReady.Configuration
{
    public class AzureStorageSettings
    {
        public string AzureStorage { get; set; }
        public bool EnableAzureQueueService { get; set; }
        public bool EnableAzureBlobImageService { get; set; }
    }

    public class GeneralSettings
    {
        public string SiteBaseUrl { get; set; }
        public string DefaultTimeZone { get; set; }
    }

    public class GetASmokeAlarmApiSettings
    {
        public string BaseAddress { get; set; }
        public string Token { get; set; }
    }

    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
    }

    public class EmailSettings
    {
        public string EmailFolder { get; set; }
    }

    public class SampleDataSettings
    {
        public string DefaultAdminUsername { get; set; }
        public string DefaultAdminPassword { get; set; }
        public string DefaultUsername { get; set; }
        public string DefaultEventManagerUsername { get; set; }
        public string DefaultCampaignManagerUsername { get; set; }
        public string DefaultOrganizationUsername { get; set; }
        public string DefaultFromEmailAddress { get; set; }
        public string DefaultFromDisplayName { get; set; }
        public string InsertSampleData { get; set; }
        public string InsertTestUsers { get; set; }
    }

    public class TwitterAuthenticationSettings
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }
    }

    public class MappingSettings
    {
        public string GoogleMapsApiKey { get; set; }
    }

    public class TwilioSettings
    {
        public string Sid { get; set; }
        public string Token { get; set; }
        public string PhoneNo { get; set; }
    }

    public class ApprovedRegionsSettings
    {
        public bool Enabled { get; set; }
        public List<string> Regions { get; set; }
    }
}
