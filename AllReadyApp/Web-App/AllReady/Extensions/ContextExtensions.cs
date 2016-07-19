using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Extensions
{
  public static class ContextExtensions
  {
    public static void AddOrUpdate(this DbContext ctx, object entity)
    {
      var entry = ctx.Entry(entity);
      if (entry.State == EntityState.Detached)
      {
        ctx.Add(entity);
      }
      else if (entry.State == EntityState.Modified)
      {
        ctx.Update(entity);
      }
    }

  }
}
