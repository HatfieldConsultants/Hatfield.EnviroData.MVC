using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatfield.EnviroData.Core
{
    public interface IDbContext
    {
        // having this interface in place is mainly for mocking purposes.
        // unit test
        // and generic processing.
        IQueryable<T> Query<T>() where T : class;
        void Add<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Remove<T>(T entity) where T : class;
        int SaveChanges();
    }
}
