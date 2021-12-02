using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public IQueryable<TEntity> GetAll()
        {
            var collection = this.databaseContext
                .Set<TEntity>();

            return collection;
        }
    }
}
