using Chat.Common.Persistence;
using Chat.Domain.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Persistence
{
    public class CoreDbContext : ProjectDbContext<CoreDbContext>
    {
        public CoreDbContext()
        {

        }
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ConfigureMappings(modelBuilder);
        }



        protected void ConfigureMappings(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        }

        #region Tables

        public DbSet<MessageModel> Messages { get; set; }
        public DbSet<ChatRequestModel> ChatRequests { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<ChatReassignmentHistoryModel> ChatReassignmentHistory { get; set; }

        #endregion

        #region Views
        #endregion

    }
}
