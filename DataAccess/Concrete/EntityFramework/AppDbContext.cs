using Core.Entities.Concrete;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (IsRunningInContainer)
            {
                optionsBuilder.UseSqlServer(@"Server=host.docker.internal,1401;Database=master;User Id=SA;Password=S3cur3P@ssW0rd!;Trust Server Certificate=true;");
            }
            else
            {
                optionsBuilder.UseSqlServer(@"Server=127.0.0.1,1401;Database=master;User Id=SA;Password=S3cur3P@ssW0rd!;Trust Server Certificate=true;");
            }
        }

        static bool IsRunningInContainer => bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var inDocker) && inDocker;

        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<ReferralLink> ReferralLinks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }

    }
}
