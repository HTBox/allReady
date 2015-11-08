
using Microsoft.WindowsAzure.Storage;

namespace AllReady
{
    public class AzureStorageSettings
    {
        public string StorageAccount { get; set; }
    }

    public class GeneralSettings
    {
        public string SiteBaseUrl { get; set; }
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
        public string DefaultTenantUsername { get; set; }
        public string DefaultFromEmailAddress { get; set; }
        public string DefaultFromDisplayName { get; set; }
        public string InsertSampleData { get; set; }
        public string InsertTestUsers { get; set; }
    }
}
