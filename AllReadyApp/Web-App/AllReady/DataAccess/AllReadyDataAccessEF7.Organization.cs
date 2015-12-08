﻿using System;
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
            var tenant = _dbContext.Organizations.SingleOrDefault(c => c.Id == id);

            if (tenant != null)
            {
                _dbContext.Organizations.Remove(tenant);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        Task IAllReadyDataAccess.UpdateOrganization(Organization value)
        {
            var tenant = _dbContext.Organizations.SingleOrDefault(c => c.Id == value.Id);

            if (tenant != null)
            {
                _dbContext.Organizations.Update(tenant);
                return _dbContext.SaveChangesAsync();
            }
            return null;
        }

        IEnumerable<Organization> IAllReadyDataAccess.Organziations
        {
            get
            {
                return _dbContext.Organizations.Include(x => x.Campaigns).ToList();
            }
        }
    }
}