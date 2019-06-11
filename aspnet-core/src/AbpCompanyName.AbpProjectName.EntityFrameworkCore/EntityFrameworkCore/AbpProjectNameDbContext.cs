using Abp.Extensions;
using Abp.Zero.EntityFrameworkCore;
using AbpCompanyName.AbpProjectName.Authorization.Roles;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using AbpCompanyName.AbpProjectName.MultiTenancy;
using AbpCompanyName.AbpProjectName.Storage;
using Microsoft.EntityFrameworkCore;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore
{
    public class AbpProjectNameDbContext : AbpZeroDbContext<Tenant, Role, User, AbpProjectNameDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public AbpProjectNameDbContext(DbContextOptions<AbpProjectNameDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CoreModelCreating(modelBuilder);

            IndexCreating(modelBuilder);
        }

        private static void IndexCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BinaryObject>(b =>
            {
                b.HasIndex(e => new { e.TenantId });
            });
        }

        private void CoreModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ChangeAbpTablePrefix<Tenant, Role, User>("Core_");
        }

        private void XXModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ChangeTablePrefix("xxx_", typeof(User));
        }
    }
}