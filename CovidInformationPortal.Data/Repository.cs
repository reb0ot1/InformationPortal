using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CovidInformationPortal.Data
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class, new()
    {
        protected CovidInformationContext databaseContext;
        public Repository(CovidInformationContext dbContext)
        {
            this.databaseContext = dbContext;
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter)
            => await this.databaseContext
                    .Set<TEntity>()
                    .FirstOrDefaultAsync(filter);

        public IQueryable<TEntity> GetAll()
        {
            var collection = this.databaseContext
                .Set<TEntity>();

            return collection;
        }

        public async Task AddManyAsync(IEnumerable<TEntity> entities)
        {
            try
            {
                await this.databaseContext
                    .Set<TEntity>()
                    .AddRangeAsync(entities);

                await this.databaseContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ;
            }
        }
    }
}
