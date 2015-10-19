using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        Tenant IAllReadyDataAccess.GetTenant(int tenantId)
        {
            return _dbContext.Tenants.Include(t => t.Campaigns).SingleOrDefault(t => t.Id == tenantId);
        }

        Task IAllReadyDataAccess.AddTenant(Tenant value)
        {
            _dbContext.Tenants.Add(value);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteTenant(int id)
        {
            var tenant = _dbContext.Tenants.SingleOrDefault(c => c.Id == id);

            if (tenant != null)
            {
                _dbContext.Tenants.Remove(tenant);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IAllReadyDataAccess.UpdateTenant(Tenant value)
        {
            var tenant = _dbContext.Tenants.SingleOrDefault(c => c.Id == value.Id);

            if (tenant != null)
            {
                _dbContext.Tenants.Update(tenant);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        IEnumerable<Tenant> IAllReadyDataAccess.Tenants
        {
            get
            {
                return _dbContext.Tenants.Include(x => x.Campaigns).ToList();
            }
        }
    }
}
