using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hatfield.EnviroData.Core.Repositories
{
    public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
    {
        private IDbContext _dbContext;

        public OrganizationRepository(IDbContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}
