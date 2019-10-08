using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using StudentsCommunity.Models;

[assembly: OwinStartupAttribute(typeof(StudentsCommunity.Startup))]
namespace StudentsCommunity
{
    public partial class Startup
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
            CreateDefaultRolesAndUsers();
        }

        public void CreateDefaultRolesAndUsers()
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            IdentityRole role = new IdentityRole();
            if (!roleManager.RoleExists("Admins"))
            {
                role.Name = "Admins";
                roleManager.Create(role);
                ApplicationUser user = new ApplicationUser();
                user.UserName = "Mfh";
                user.Email = "Mfh@hotmail.com";
                //to avoid NullReferenceExceptions for img
                user.PersonalInformation = new PersonalInformation { UserImage = "img.png" };
                var Check = userManager.Create(user, "Mfh@011");
                if (Check.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admins");
                }
            }
        }

    }
}
