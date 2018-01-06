using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AllReady.ExtensionWrappers
{
    public class FromSqlWrapper : IFromSqlWrapper
    {
        public IQueryable<TEntity> FromSql<TEntity>(IQueryable<TEntity> source, string sql, params object[] parameters) where TEntity : class
        {
            return source.FromSql(sql, parameters);
        }
    }
}
