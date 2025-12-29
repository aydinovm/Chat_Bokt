using Chat.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Persistence.Repositories
{
    public class DepartmentRepository : Repository<DepartmentModel, CoreDbContext>, IDepartmentRepository
    {
        public DepartmentRepository(CoreDbContext context) : base(context)
        {
        }
    }

}
