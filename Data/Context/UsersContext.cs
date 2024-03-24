using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using projectUsers.Data.Models;

namespace projectUsers.Data.Context
{
    public class UsersContext : IdentityDbContext <User>
    {
        public UsersContext() { }
        public UsersContext(DbContextOptions<UsersContext> options):base(options)
        { 
        
        }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("Users");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UsersClaims");
        }
    }

}
