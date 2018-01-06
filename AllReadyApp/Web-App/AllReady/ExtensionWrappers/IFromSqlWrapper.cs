using System.Linq;

namespace AllReady.ExtensionWrappers
{
    public interface IFromSqlWrapper
    {
        IQueryable<TEntity> FromSql<TEntity>(
            IQueryable<TEntity> source,
            string sql,
            params object[] parameters) where TEntity : class;
    }
}