using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatfield.EnviroData.Core.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private IDbContext _dbContext;

        public Repository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> GetAll()
        {
            return _dbContext.Query<T>();
        }
    }
}
