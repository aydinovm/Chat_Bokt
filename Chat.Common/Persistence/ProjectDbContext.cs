using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Common.Persistence
{
    public class ProjectDbContext<T> : DbContext, IDbContext where T : DbContext
    {
        public ProjectDbContext() { }

        public ProjectDbContext(DbContextOptions<T> options) : base(options) { }
    }
}
