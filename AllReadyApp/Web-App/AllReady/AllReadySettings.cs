namespace AllReady
{
    public class AzureStorageSettings
    {
        public string AzureStorage { get; set; }
        public bool EnableAzureQueueService { get; set; }
    }

    public class GeneralSettings
    {
        public string SiteBaseUrl { get; set; }
        public string DefaultTimeZone { get; set; }
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
        public string OAuthToken { get; set; }
        public string OAuthSecret { get; set; }
    }
}
