using System;
using Microsoft.EntityFrameworkCore;

namespace AllReady.Extensions
{
    public static class ContextExtensions
    {
        /// <summary>
        /// Handles correctly attaching an object to the context for cases where it may be new or modified.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="entity"></param>
        public static void AddOrUpdate(this DbContext ctx, object entity)
        {
            var entry = ctx.Entry(entity);

            switch (entry.State)
            {
                case EntityState.Detached:
                    ctx.Add(entity);
                    break;
                case EntityState.Modified:
                    ctx.Update(entity);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
