using BookShopSystem.Data.Migrations;
using BookShopSystem.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookShopSystem.Data
{
    using System.Data.Entity;

    public class BookShopContext : IdentityDbContext<ApplicationUser>
    {
        public BookShopContext()
            : base("name=BookShopContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<BookShopContext, Configuration>());
        }

        public static BookShopContext Create()
        {
            return new BookShopContext();
        }

        public virtual DbSet<Author> Authors { get; set; }

        public virtual DbSet<Book> Books { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<Purchase> Purchases { get; set; }

        public virtual DbSet<IdentityRole> Roles { get; set; }
    }
}