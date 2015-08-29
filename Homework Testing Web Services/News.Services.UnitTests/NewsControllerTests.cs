namespace News.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Web.Http;
    using System.Web.Http.Routing;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;
    using News.Services.Controllers;
    using News.Services.Models;
    using Newtonsoft.Json;

    using News.Models;

    [TestClass]
    public class NewsControllerTests
    {
        [TestMethod]
        public void ListAllNewsItemsCorrectly()
        {
            var repo = new RepositoryMock<News>();

            var news = new Dictionary<object, News>();

            news.Add(1, new News()
            {
                Id = 1,
                AuthorId = "test",
                Content = "News1 Content",
                Title = "News1 Title",
                PublishDate = DateTime.Now
            });

            news.Add(2, new News()
            {
                Id = 2,
                AuthorId = "test",
                Content = "News2 Content",
                Title = "News2 Title",
                PublishDate = DateTime.Now
            });

            news.Add(3, new News()
            {
                Id = 3,
                AuthorId = "test",
                Content = "News3 Content",
                Title = "News3 Title",
                PublishDate = DateTime.Now
            });

            repo.Entities = news;

            var controller = new NewsController(repo);

            var result = controller.GetAllNews().ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.Created, result.StatusCode);

            var theNews = JsonConvert.DeserializeObject<Dictionary<object, News>>(result.Content.ReadAsStringAsync().Result);

            // Assert
            CollectionAssert.AreEquivalent(news, theNews);
        }

        [TestMethod]
        public void CreateNewsItemWithCorrectData()
        {
            var repo = new RepositoryMock<News>();
            repo.IsSaveCalled = false;
            repo.Entities = new Dictionary<object, News>();
            var news = new News()
            {
                Id = 1,
                AuthorId = "valid",
                Title = "valid",
                Content = "valid",
                PublishDate = DateTime.Now
            };

            var controller = new NewsController(repo);
            this.SetupController(controller, "news");

            // Act
            var httpResponse = controller.CreateNews(new NewsBindingModel()
            {
                Id = news.Id,
                AuthorId = news.AuthorId,
                Title = news.Title,
                Content = news.Content,
                PublishDate = news.PublishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, httpResponse.StatusCode);
            Assert.IsNotNull(httpResponse.Headers.Location);

            var newsFromService = httpResponse.Content.ReadAsAsync<News>().Result;
            Assert.AreEqual(newsFromService.Title, news.Title);
            Assert.AreEqual(newsFromService.AuthorId, news.AuthorId);
            Assert.AreEqual(newsFromService.Content, news.Content);
            Assert.AreEqual(newsFromService.Id, news.Id);
            Assert.IsNotNull(newsFromService.PublishDate);

            Assert.AreEqual(repo.Entities.Count, 1);
            var newsInRepo = repo.Entities.First().Value;
            Assert.AreEqual(news.Title, newsInRepo.Title);
            Assert.AreEqual(news.Id, newsInRepo.Id);
            Assert.AreEqual(news.Content, newsInRepo.Content);
            Assert.AreEqual(news.AuthorId, newsInRepo.AuthorId);
            Assert.IsNotNull(newsInRepo.PublishDate);
            Assert.IsTrue(repo.IsSaveCalled);
        }

        [TestMethod]
        public void CreateNewsItemWithIncorrectData()
        {
            var repo = new RepositoryMock<News>();
            var controller = new NewsController(repo);
            this.SetupController(controller, "news");

            // Act
            var news = new News()
            {
                Id = 1,
                Title = "valid",
                AuthorId = "valid",
                PublishDate = DateTime.Now,
                Content = null
            };

            var result = controller.CreateNews(new NewsBindingModel()
            {
                Id = news.Id,
                AuthorId = news.AuthorId,
                Title = news.Title,
                Content = news.Content,
                PublishDate = news.PublishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void ModifyExistingNewsItemWithCorrectData()
        {
            var repo = new RepositoryMock<News>();
            var controller = new NewsController(repo);
            this.SetupController(controller, "news");

            // Act
            var news = new News()
            {
                Id = 1,
                Title = "valid",
                AuthorId = "valid",
                PublishDate = DateTime.Now,
                Content = "valid"
            };

            var result = controller.CreateNews(new NewsBindingModel()
            {
                Id = news.Id,
                AuthorId = news.AuthorId,
                Title = news.Title,
                Content = news.Content,
                PublishDate = news.PublishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            var theCreatedNews = repo.Entities.First().Value;

            theCreatedNews.Content = "modified";

            var editResult = controller.EditNews(theCreatedNews.Id, new NewsBindingModel()
            {
                Id = theCreatedNews.Id,
                AuthorId = theCreatedNews.AuthorId,
                Title = theCreatedNews.Title,
                Content = theCreatedNews.Content,
                PublishDate = theCreatedNews.PublishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            
            var newsItemInRepo = repo.Entities.First().Value;

            Assert.IsNotNull(newsItemInRepo);
            Assert.AreEqual(newsItemInRepo.Content, theCreatedNews.Content);
        }

        [TestMethod]
        public void ModifyExistingNewsItemWithIncorrectData()
        {
            var repo = new RepositoryMock<News>();
            var controller = new NewsController(repo);
            this.SetupController(controller, "news");

            // Act
            var news = new News()
            {
                Id = 1,
                Title = "valid",
                AuthorId = "valid",
                PublishDate = DateTime.Now,
                Content = "valid"
            };

            var result = controller.CreateNews(new NewsBindingModel()
            {
                Id = news.Id,
                AuthorId = news.AuthorId,
                Title = news.Title,
                Content = news.Content,
                PublishDate = news.PublishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            var theCreatedNews = repo.Entities.First().Value;

            theCreatedNews.Content = null;

            var editResult = controller.EditNews(theCreatedNews.Id, new NewsBindingModel()
            {
                Id = theCreatedNews.Id,
                AuthorId = theCreatedNews.AuthorId,
                Title = theCreatedNews.Title,
                Content = theCreatedNews.Content,
                PublishDate = theCreatedNews.PublishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void ModifyNonExistingNewsItem()
        {
            var repo = new RepositoryMock<News>();
            var controller = new NewsController(repo);
            this.SetupController(controller, "news");

            // Act
            // Not added
            var news = new News()
            {
                Id = 1,
                Title = "valid",
                AuthorId = "valid",
                PublishDate = DateTime.Now,
                Content = "valid"
            };

            var result = controller.EditNews(news.Id, new NewsBindingModel()
            {
                Id = news.Id,
                AuthorId = news.AuthorId,
                Title = news.Title,
                Content = news.Content,
                PublishDate = news.PublishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode);
        }

        [TestMethod]
        public void DeleteExistingNewsItem()
        {
            var repo = new RepositoryMock<News>();
            var controller = new NewsController(repo);
            this.SetupController(controller, "news");

            // Act
            var news = new News()
            {
                Id = 1,
                Title = "valid",
                AuthorId = "valid",
                PublishDate = DateTime.Now,
                Content = "valid"
            };

            var result = controller.CreateNews(new NewsBindingModel()
            {
                Id = news.Id,
                AuthorId = news.AuthorId,
                Title = news.Title,
                Content = news.Content,
                PublishDate = news.PublishDate
            }).ExecuteAsync(new CancellationToken()).Result;

            var theCreatedNews = repo.Entities.First().Value;

            var deleteResult = controller.DeletelNews(theCreatedNews.Id).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
            Assert.AreEqual(repo.Entities.Count, 0);
        }

        [TestMethod]
        public void DeleteNonExistingNewsItem()
        {
            var repo = new RepositoryMock<News>();
            var controller = new NewsController(repo);
            this.SetupController(controller, "news");

            var deleteResult = controller.DeletelNews(20).ExecuteAsync(new CancellationToken()).Result;

            Assert.AreEqual(HttpStatusCode.BadRequest, deleteResult.StatusCode);
        }

        private void SetupController(ApiController controller, string controllerName)
        {
            string serverUrl = "http://sample-url.com";

            // Setup the Request object of the controller
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(serverUrl)
            };
            controller.Request = request;

            // Setup the configuration of the controller
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            controller.Configuration = config;

            // Apply the routes to the controller
            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary
                {
                    { "controller", controllerName }
                });
        }
    }
}
