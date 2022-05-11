using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CovidInformationPortal.Data
{
    public interface IRepository<TEntity> 
        where TEntity : class, new()
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter);

        Task AddManyAsync(IEnumerable<TEntity> entities);

        Task AddAsync(TEntity entity);
    }
}
