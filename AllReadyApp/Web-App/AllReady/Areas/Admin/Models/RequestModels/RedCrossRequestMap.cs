using AllReady.Models;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllReady.Areas.Admin.Models.RequestModels
{
    public class RedCrossRequestMap : CsvClassMap<Request>
    {
        public RedCrossRequestMap()
        {
            // columns from Red Cross CSV
            // "id","name","address","city","state","zip","phone","email","date created","region"

            // set defaults
            Map(m => m.RequestId).Default(Guid.NewGuid());
            Map(m => m.Status).Default(RequestStatus.UnAssigned);

            // map from Red Cross data
            Map(m => m.Name).Name("name");
            Map(m => m.Address).Name("address");
            Map(m => m.City).Name("city");
            Map(m => m.State).Name("state");
            Map(m => m.Zip).Name("zip");
            Map(m => m.Phone).Name("phone");
            Map(m => m.Email).Name("email");
            Map(m => m.DateAdded).Name("date created");
            Map(m => m.ProviderData).Name("region");
            Map(m => m.ProviderId).Name("id");
        }

    }
}
