using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatfield.EnviroData.Core.Repositories
{
    public class VariableRepository : Repository<Variable>, IVariableRepository
    {
        private IDbContext _dbContext;

        public VariableRepository(IDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
