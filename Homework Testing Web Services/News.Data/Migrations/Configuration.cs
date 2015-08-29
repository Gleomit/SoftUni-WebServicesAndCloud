namespace News.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using News.Models;


    internal sealed class Configuration : DbMigrationsConfiguration<NewsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(NewsContext context)
        {
            UserStore<ApplicationUser> userStore = new UserStore<ApplicationUser>(context);

            UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(userStore);

            //Add or Update the initial Users into the database as normal.
            context.Users.AddOrUpdate(
                x => x.UserName,  //Using Username as the Unique Key: If a record exists with the same username, AddOrUpdate skips it.
                new ApplicationUser() { Email = "test@gmail.com", UserName = "test", PasswordHash = new PasswordHasher().HashPassword("testing") }
            );
        }
    }
}
