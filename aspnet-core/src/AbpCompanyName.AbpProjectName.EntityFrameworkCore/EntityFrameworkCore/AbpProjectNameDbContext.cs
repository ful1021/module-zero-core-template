using Abp.Extensions;
using Abp.Zero.EntityFrameworkCore;
using AbpCompanyName.AbpProjectName.Authorization.Roles;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using AbpCompanyName.AbpProjectName.DataDictionaries;
using AbpCompanyName.AbpProjectName.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore
{
    public class AbpProjectNameDbContext : AbpZeroDbContext<Tenant, Role, User, AbpProjectNameDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<DataDictionary> DataDictionaries { get; set; }

        public AbpProjectNameDbContext(DbContextOptions<AbpProjectNameDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //AbpModelCreating(modelBuilder);
            CoreModelCreating(modelBuilder);
        }

        private void AbpModelCreating(ModelBuilder modelBuilder)
        {
            var prefix = "Core_";
            modelBuilder.ChangeAbpTablePrefix<Tenant, Role, User>(prefix);
        }

        private void CoreModelCreating(ModelBuilder modelBuilder)
        {
            var prefix = "Core_";
            modelBuilder.ChangeTablePrefix(prefix
                , typeof(DataDictionary)
                );
        }
    }
}