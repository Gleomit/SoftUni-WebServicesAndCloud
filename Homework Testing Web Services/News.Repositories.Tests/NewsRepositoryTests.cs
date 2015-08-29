namespace News.Repositories.Tests
{
    using News.Models;

    using System;
    using System.Data.Entity.Validation;
    using System.Linq;
    using System.Transactions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using News.Data;

    [TestClass]
    public class NewsRepositoryTests
    {
        private static TransactionScope tran;

        [TestInitialize]
        public void TestInit()
        {
            // Start a new temporary transaction
            tran = new TransactionScope();
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            // Rollback the temporary transaction
            tran.Dispose();
        }

        [TestMethod]
        public void ListAllNewsItemsCorrectly()
        {
            var context = new NewsContext();

            var newsRepo = new NewsRepository(context);

            var news = newsRepo.All();
        }

        [TestMethod]
        public void CreateNewsItemWithCorrectData()
        {
            var context = new NewsContext();

            var newsRepo = new NewsRepository(context);
            var usersRepo = new UsersRepository(context);

            var user = usersRepo.All().First();

            var validNewsItem = new News()
            {
                AuthorId = user.Id,
                Title = "Valid Title",
                Content = "Valid Content",
                PublishDate = DateTime.Now
            };

            newsRepo.Add(validNewsItem);

            newsRepo.SaveChanges();

            var checkNewsItem = newsRepo.All().First(n => n.Title == "Valid Title");

            Assert.IsNotNull(checkNewsItem);
            Assert.IsNotNull(checkNewsItem.PublishDate);
            Assert.AreEqual(checkNewsItem.AuthorId, validNewsItem.AuthorId);
            Assert.AreEqual(checkNewsItem.Title, validNewsItem.Title);
            Assert.AreEqual(checkNewsItem.Content, validNewsItem.Content);           
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void CreateNewsItemWithIncorrectData()
        {
            var context = new NewsContext();

            var newsRepo = new NewsRepository(context);
            var usersRepo = new UsersRepository(context);

            var user = usersRepo.All().First();

            var invalidNewsItem = new News()
            {
                AuthorId = user.Id,
                Content = "Valid Content",
                PublishDate = DateTime.Now
            };

            newsRepo.Add(invalidNewsItem);

            newsRepo.SaveChanges();
        }

        [TestMethod]
        public void ModifyExistingNewsItemWithCorrectData()
        {
            var context = new NewsContext();

            var newsRepo = new NewsRepository(context);
            var usersRepo = new UsersRepository(context);

            var user = usersRepo.All().First();

            var newsItem = new News()
            {
                AuthorId = user.Id,
                PublishDate = DateTime.Now,
                Title = "Test Title",
                Content = "Test Content"
            };

            newsRepo.Add(newsItem);

            newsRepo.SaveChanges();

            newsItem.Content = "changed content";

            newsRepo.Update(newsItem);

            newsRepo.SaveChanges();

            var checkNewsItem = newsRepo.Find(newsItem.Id);

            Assert.AreEqual(newsItem.Content, checkNewsItem.Content);
        }

        [TestMethod]
        [ExpectedException(typeof(DbEntityValidationException))]
        public void ModifyExistingNewsItemWithIncorrectData()
        {
            var context = new NewsContext();

            var newsRepo = new NewsRepository(context);
            var usersRepo = new UsersRepository(context);

            var user = usersRepo.All().First();

            var newsItem = new News()
            {
                AuthorId = user.Id,
                PublishDate = DateTime.Now,
                Title = "Test Title",
                Content = "Test Content"
            };

            newsRepo.Add(newsItem);

            newsRepo.SaveChanges();

            newsItem.Title = null;

            newsRepo.Update(newsItem);

            newsRepo.SaveChanges();
        }

        [TestMethod]
        [ExpectedException(typeof(DBNull))]
        public void ModifyNonExistingNewsItem()
        {
            var context = new NewsContext();

            var newsRepo = new NewsRepository(context);
            var usersRepo = new UsersRepository(context);

            var user = usersRepo.All().First();

            var fakeNews = new News()
            {
                Id = 500000,
                AuthorId = user.Id,
                PublishDate = DateTime.Now,
                Title = "Test Title",
                Content = "Test Content"
            };

            newsRepo.Update(fakeNews);

            newsRepo.SaveChanges();
        }

        [TestMethod]
        public void DeleteExistingNewsItem()
        {
            var context = new NewsContext();

            var newsRepo = new NewsRepository(context);
            var usersRepo = new UsersRepository(context);

            var user = usersRepo.All().First();

            var newsItem = new News()
            {
                AuthorId = user.Id,
                PublishDate = DateTime.Now,
                Title = "Test Title",
                Content = "Test Content"
            };

            newsRepo.Add(newsItem);

            newsRepo.SaveChanges();

            int id = newsItem.Id;

            newsRepo.Delete(newsItem);

            newsRepo.SaveChanges();

            var checkNewsItem = newsRepo.Find(id);

            Assert.IsNull(checkNewsItem);
        }

        [TestMethod]
        [ExpectedException(typeof(DBNull))]
        public void DeleteNonExistingNewsItem()
        {
            var context = new NewsContext();

            var newsRepo = new NewsRepository(context);
            var usersRepo = new UsersRepository(context);

            var user = usersRepo.All().First();

            var fakeNews = new News()
            {
                Id = 1000,
                AuthorId = user.Id,
                PublishDate = DateTime.Now,
                Title = "Test Title",
                Content = "Test Content"
            };

            newsRepo.Delete(fakeNews);

            newsRepo.SaveChanges();
        }
    }
}
