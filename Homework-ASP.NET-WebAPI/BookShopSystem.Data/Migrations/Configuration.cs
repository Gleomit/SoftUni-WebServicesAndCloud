using System.Globalization;
using System.IO;
using BookShopSystem.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookShopSystem.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BookShopSystem.Data.BookShopContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BookShopSystem.Data.BookShopContext context)
        {
            if (!context.Roles.Any())
            {
                context.Roles.Add(new IdentityRole()
                {
                    Name = "Admin"
                });

                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.Add(new ApplicationUser()
                {
                    Email = "admin@mail.bg",
                    PhoneNumber = "0999222333",
                    PasswordHash = "AAFTSuXZFz6dgw6tUbYFRh61nxmrhig7KgZNL2yqAq2sCriz0GdUsf5WgfaA5tPCwA==", //password = Admin1?
                    SecurityStamp = "4d357add-b279-4461-8ddd-584ab98a960b"
                });

                context.SaveChanges();
            }

            var adminRole = context.Roles.First();

            var user = context.Users.First();

            adminRole.Users.Add(new IdentityUserRole()
            {
                UserId = user.Id
            });

            context.SaveChanges();

            /*try
            {
                if (!context.Authors.Any())
                {
                    LoadAuthors(context);
                }

                if (!context.Categories.Any())
                {
                    LoadCategories(context);
                }

                if (context.Categories.Any() && !context.Books.Any())
                {
                    LoadBooks(context);
                }
            }
            catch (Exception ex)
            {
                ex.Message.Clone();
            }*/
        }

        private void LoadAuthors(BookShopContext context)
        {
            using (var reader = new StreamReader("../../SeedData/authors.txt"))
            {
                var line = reader.ReadLine();
                line = reader.ReadLine();

                while (line != null)
                {
                    var data = line.Split(new[] { ' ' });

                    string firstName = (data.Length < 2 ? null : data[0]);
                    string lastName = (data.Length < 2 ? data[0] : data[1]);

                    context.Authors.AddOrUpdate(new Author()
                    {
                        FirstName = firstName,
                        LastName = lastName
                    });

                    line = reader.ReadLine();
                }

                context.SaveChanges();
            }
        }

        private void LoadCategories(BookShopContext context)
        {
            using (var reader = new StreamReader("../../SeedData/categories.txt"))
            {
                var line = reader.ReadLine();

                while (line != null)
                {
                    context.Categories.AddOrUpdate(c => c.Name, new Category()
                    {
                        Name = line.Trim()
                    });

                    line = reader.ReadLine();
                }

                context.SaveChanges();
            }
        }

        private void LoadBooks(BookShopContext context)
        {
            using (var reader = new StreamReader("../../SeedData/books.txt"))
            {
                Random random = new Random();
                var authors = context.Authors.Select(a => a.Id)
                                             .ToList();
                var line = reader.ReadLine();
                line = reader.ReadLine();

                while (line != null)
                {
                    var data = line.Split(new[] { ' ' }, 6);
                    var authorIndex = random.Next(0, authors.Count);
                    var authorId = authors[authorIndex];
                    var edition = (Edition)int.Parse(data[0]);
                    var releaseDate = DateTime.ParseExact(data[1], "d/M/yyyy", CultureInfo.InvariantCulture);
                    var copies = int.Parse(data[2]);
                    var price = decimal.Parse(data[3]);
                    var ageRestriction = (AgeRestriction)int.Parse(data[4]);
                    var title = data[5];

                    context.Books.Add(new Book()
                    {
                        AuthorId = authorId,
                        Edition = edition,
                        ReleaseDate = releaseDate,
                        Copies = copies,
                        Price = price,
                        AgeRestriction = ageRestriction,
                        Title = title
                    });

                    line = reader.ReadLine();
                }

                context.SaveChanges();
            }
        }
    }
}
