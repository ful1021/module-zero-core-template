﻿using Abp.Extensions;
using Abp.Zero.EntityFrameworkCore;
using AbpCompanyName.AbpProjectName.Authorization.Roles;
using AbpCompanyName.AbpProjectName.Authorization.Users;
using AbpCompanyName.AbpProjectName.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace AbpCompanyName.AbpProjectName.EntityFrameworkCore
{
    public class AbpProjectNameDbContext : AbpZeroDbContext<Tenant, Role, User, AbpProjectNameDbContext>
    {
        /* Define a DbSet for each entity of the application */

        public AbpProjectNameDbContext(DbContextOptions<AbpProjectNameDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            CoreModelCreating(modelBuilder);
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