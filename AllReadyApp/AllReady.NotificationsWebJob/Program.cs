using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace AllReady.NotificationsWebJob
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connectionString = EnvironmentHelper.TryGetEnvironmentVariable("Data:Storage:AzureStorage");

            var config = new JobHostConfiguration
            {
                StorageConnectionString = connectionString,
                DashboardConnectionString = connectionString
            };

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
