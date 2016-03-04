using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;

namespace AllReady.Models
{
    public partial class AllReadyDataAccessEF7 : IAllReadyDataAccess
    {
        Organization IAllReadyDataAccess.GetOrganization(int organizationId)
        {
            return _dbContext.Organizations.Include(t => t.Campaigns).SingleOrDefault(t => t.Id == organizationId);
        }

        Task IAllReadyDataAccess.AddOrganization(Organization value)
        {
            _dbContext.Organizations.Add(value);
            return _dbContext.SaveChangesAsync();
        }

        Task IAllReadyDataAccess.DeleteOrganization(int id)
        {
            var organization = _dbContext.Organizations.SingleOrDefault(c => c.Id == id);

            if (organization != null)
            {
                _dbContext.Organizations.Remove(organization);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IAllReadyDataAccess.UpdateOrganization(Organization value)
        {
            var organization = _dbContext.Organizations.SingleOrDefault(c => c.Id == value.Id);

            if (organization != null)
            {
                _dbContext.Organizations.Update(organization);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        IEnumerable<Organization> IAllReadyDataAccess.Organizations
        {
            get
            {
                return _dbContext.Organizations.Include(x => x.Campaigns).ToList();
            }
        }
    }
}