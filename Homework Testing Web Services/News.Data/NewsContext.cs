namespace News.Data
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    using News.Models;

    using Microsoft.AspNet.Identity.EntityFramework;
    using News.Data.Migrations;

    public class NewsContext : IdentityDbContext<ApplicationUser>
    {
        public NewsContext()
            : base("name=NewsContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<NewsContext, Configuration>());
        }

        public virtual IDbSet<News> Newses { get; set; }

        public static NewsContext Create()
        {
            return new NewsContext();
        }
    }
}