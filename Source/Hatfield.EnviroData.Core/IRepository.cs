using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hatfield.EnviroData.Core
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
    }
}
