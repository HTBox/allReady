using System;
using AllReady.Areas.Admin.ViewModels.Import;
using CsvHelper.Configuration;

namespace AllReady.Areas.Admin.ViewModels.Request
{
    //public class RedCrossRequestMap : CsvClassMap<AllReady.Models.Request>
    public class RedCrossRequestMap : CsvClassMap<ImportRequestViewModel>
    {
        public Func<DateTime> DateTimeUtcNow = () => DateTime.UtcNow;

        public RedCrossRequestMap()
        {
            // columns from Red Cross CSV
            // "id","name","address","city","state","zip","phone","email","date created","region","latitude","longitude"

            // map from Red Cross data
            Map(r => r.ProviderRequestId).Name("id");
            Map(r => r.Name).Name("name");
            Map(r => r.Address).Name("address");
            Map(r => r.City).Name("city");
            Map(r => r.State).Name("state");
            Map(r => r.Zip).Name("zip");
            Map(r => r.Phone).Name("phone");
            Map(r => r.Email).Name("email");
            Map(r => r.ProviderData).Name("region");
            Map(r => r.ProviderRequestId).Name("id");
            Map(r => r.Latitude).Name("latitude").Default(0);
            Map(r => r.Longitude).Name("longitude").Default(0);
        }
    }
}