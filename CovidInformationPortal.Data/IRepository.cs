using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CovidInformationPortal.Data
{
    public interface IRepository<TEntity> 
        where TEntity : class, new()
    {
        IQueryable<TEntity> GetAll();
    }
}
