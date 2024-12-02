using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialSyncData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialSyncData.Data
{
    public class SocialSyncDataContext : IdentityDbContext<User>
    {
        public SocialSyncDataContext(DbContextOptions options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Forgotpassword> forgotpassword { get; set; }
        public DbSet<Useraccount> useraccount { get; set; }
    }
}
