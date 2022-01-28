using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public async void EnsureSeedData(UserManager<ApplicationUser> userMgr, RoleManager<IdentityRole> roleMgr)
        {
          //  var userRole = await roleMgr.FindByNameAsync("User");
          //  if (userRole == null)
          //  {
                var userRole = new IdentityRole("User");
                await roleMgr.CreateAsync(userRole);
            //   }

            var userRole2 = await roleMgr.FindByNameAsync("User");

            var canNotifyRole = await roleMgr.FindByNameAsync("CanNotify");
            if (canNotifyRole == null)
            {
                canNotifyRole = new IdentityRole("CanNotify");
                await roleMgr.CreateAsync(canNotifyRole);
            }
             
        }
    }
}
