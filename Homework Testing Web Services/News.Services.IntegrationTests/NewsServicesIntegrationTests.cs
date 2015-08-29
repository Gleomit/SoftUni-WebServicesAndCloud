namespace News.Services.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Microsoft.Owin.Testing;
    using News.Data;
    using Newtonsoft.Json;
    using Owin;

    using News.Models;

    [TestClass]
    public class NewsServicesIntegrationTests
    {
        private TestServer httpTestServer;
        private HttpClient httpClient;

        [TestInitialize]
        public void TestInit()
        {
            // Start OWIN testing HTTP server with Web API support
            this.httpTestServer = TestServer.Create(appBuilder =>
            {
                var config = new HttpConfiguration();
                WebApiConfig.Register(config);
                appBuilder.Use(config);
            });
            this.httpClient = httpTestServer.HttpClient;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.httpTestServer.Dispose();
        }

        [TestMethod]
        public void ListAllNewsItemsShouldReturn200OkAndJson()
        {
            // Arrange
            CleanDatabase();

            // Act
            var httpResponse = httpClient.GetAsync("/api/news").Result;
            var news = httpResponse.Content.ReadAsAsync<List<News>>().Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.AreEqual(httpResponse.Content.Headers.ContentType.MediaType, "application/json");
            Assert.AreEqual(1, news.Count);
        }

        [TestMethod]
        public void RegisterUserWithCorrectData()
        {
            // Arrange
            CleanDatabase();

            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "top@gmail.com"),
                new KeyValuePair<string, string>("Password", "topPass"),
                new KeyValuePair<string, string>("ConfirmPassword", "topPass")
            });

            // Act
            var httpResponse = httpClient.PostAsync("/api/users/register", bodyData).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [TestMethod]
        public void RegisterUserWithIncorrectData()
        {
            // Arrange
            CleanDatabase();

            // Act
            var httpResponse = httpClient.PostAsync("/api/users/register", null).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [TestMethod]
        public void UserLoginWithCorrectData()
        {
            // Arrange
            CleanDatabase();

            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "test@gmail.com"),
                new KeyValuePair<string, string>("Password", "testing"),
                new KeyValuePair<string, string>("grant_type", "password")
            });

            // Act
            var httpResponse = httpClient.PostAsync("/api/token", bodyData).Result;

            var accessToken = JsonConvert.DeserializeObject<dynamic>(httpResponse.Content.ReadAsStringAsync()
                .Result)["Access_Token"].ToString();

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsNotNull(accessToken);
        }

        [TestMethod]
        public void UserLoginWithIncorrectData()
        {
            // Arrange
            CleanDatabase();

            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "test@gmail.com"),
                new KeyValuePair<string, string>("Password", "tt"),
                new KeyValuePair<string, string>("grant_type", "password")
            });

            // Act
            var httpResponse = httpClient.PostAsync("/api/token", bodyData).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [TestMethod]
        public void CreateNewsItemWithCorrectDataAndSessionToken()
        {
            // Arrange
            CleanDatabase();

            // Act
            var accessToken = this.Login();

            var news = new News()
            {
                AuthorId = this.TestingUser.Id,
                Title = "testTitle",
                Content = "testContent",
                PublishDate = DateTime.Now
            };

            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("AuthorId", news.AuthorId),
                new KeyValuePair<string, string>("Title", news.Title),
                new KeyValuePair<string, string>("Content", news.Content),
                new KeyValuePair<string, string>("PublishDate", news.PublishDate.ToString())
            });

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var httpResponse = httpClient.PostAsync("/api/news", bodyData).Result;

            var theCreatedItem = httpResponse.Content.ReadAsAsync<News>().Result;
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsNotNull(theCreatedItem);
            Assert.AreEqual(news.Title, theCreatedItem.Title);
            Assert.AreEqual(news.Content, theCreatedItem.Content);
            Assert.AreEqual(news.AuthorId, theCreatedItem.AuthorId);
            Assert.AreEqual(news.PublishDate, theCreatedItem.PublishDate);
        }

        [TestMethod]
        public void CreateNewsItemWithInCorrectDataAndSessionToken()
        {
            // Arrange
            CleanDatabase();

            // Act
            var accessToken = this.Login();

            var news = new News()
            {
                AuthorId = this.TestingUser.Id,
                Title = "testTitle",
                PublishDate = DateTime.Now
            };

            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("AuthorId", news.AuthorId),
                new KeyValuePair<string, string>("Title", news.Title),
                new KeyValuePair<string, string>("Content", news.Content),
                new KeyValuePair<string, string>("PublishDate", news.PublishDate.ToString())
            });

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var httpResponse = httpClient.PostAsync("/api/news", bodyData).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [TestMethod]
        public void CreateNewsItemWithCorrectDataAndWithoutSessionToken()
        {
            // Arrange
            CleanDatabase();

            // Act
            var news = new News()
            {
                AuthorId = this.TestingUser.Id,
                Title = "testTitle",
                Content = "testContent",
                PublishDate = DateTime.Now
            };

            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("AuthorId", news.AuthorId),
                new KeyValuePair<string, string>("Title", news.Title),
                new KeyValuePair<string, string>("Content", news.Content),
                new KeyValuePair<string, string>("PublishDate", news.PublishDate.ToString())
            });

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            var httpResponse = httpClient.PostAsync("/api/news", bodyData).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }

        [TestMethod]
        public void ModifyNewsItemWithCorrectDataAndSessionToken()
        {
            // Arrange
            CleanDatabase();

            // Act
            var accessToken = this.Login();

            var news = new News()
            {
                AuthorId = this.TestingUser.Id,
                Title = "Changed Title",
                Content = "Changed Content",
                PublishDate = DateTime.Now
            };

            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("AuthorId", news.AuthorId),
                new KeyValuePair<string, string>("Title", news.Title),
                new KeyValuePair<string, string>("Content", news.Content),
                new KeyValuePair<string, string>("PublishDate", news.PublishDate.ToString())
            });

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var httpResponse = httpClient.PutAsync("/api/news/1", bodyData).Result;

            var theCreatedItem = httpResponse.Content.ReadAsAsync<News>().Result;
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
            Assert.IsNotNull(theCreatedItem);
            Assert.AreEqual(news.Title, theCreatedItem.Title);
            Assert.AreEqual(news.Content, theCreatedItem.Content);
            Assert.AreEqual(news.AuthorId, theCreatedItem.AuthorId);
            Assert.AreEqual(news.PublishDate, theCreatedItem.PublishDate);
        }

        [TestMethod]
        public void ModifyNewsItemWithInCorrectDataAndSessionToken()
        {
            // Arrange
            CleanDatabase();

            // Act
            var accessToken = this.Login();

            var news = new News()
            {
                AuthorId = this.TestingUser.Id,
                Title = "testTitle",
                PublishDate = DateTime.Now
            };

            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("AuthorId", news.AuthorId),
                new KeyValuePair<string, string>("Title", news.Title),
                new KeyValuePair<string, string>("Content", news.Content),
                new KeyValuePair<string, string>("PublishDate", news.PublishDate.ToString())
            });

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                        
            var httpResponse = httpClient.PutAsync("/api/news/1", bodyData).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [TestMethod]
        public void ModifyNewsItemWithCorrectDataAndWithoutSessionToken()
        {
            // Arrange
            CleanDatabase();

            // Act
            var news = new News()
            {
                AuthorId = this.TestingUser.Id,
                Title = "Changed Title",
                Content = "Changed Content",
                PublishDate = DateTime.Now
            };

            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("AuthorId", news.AuthorId),
                new KeyValuePair<string, string>("Title", news.Title),
                new KeyValuePair<string, string>("Content", news.Content),
                new KeyValuePair<string, string>("PublishDate", news.PublishDate.ToString())
            });

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }
            
            var httpResponse = httpClient.PutAsync("/api/news/1", bodyData).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }

        [TestMethod]
        public void DeleteNewsItemWithCorrectDataAndSessionToken()
        {
            // Arrange
            CleanDatabase();

            // Act
            var accessToken = this.Login();

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var httpResponse = httpClient.DeleteAsync("/api/news/1").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        [TestMethod]
        public void DeleteNewsItemWithInCorrectDataAndSessionToken()
        {
            // Arrange
            CleanDatabase();

            // Act
            var accessToken = this.Login();

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            var httpResponse = httpClient.DeleteAsync("/api/news/3000").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        }

        [TestMethod]
        public void DeleteNewsItemWithCorrectDataAndWithoutSessionToken()
        {
            // Arrange
            CleanDatabase();

            // Act

            if (httpClient.DefaultRequestHeaders.Contains("Authorization"))
            {
                httpClient.DefaultRequestHeaders.Remove("Authorization");
            }

            var httpResponse = httpClient.DeleteAsync("/api/news/1").Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, httpResponse.StatusCode);
        }

        private string Login()
        {
            var bodyData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", "test@gmail.com"),
                new KeyValuePair<string, string>("Password", "testing"),
                new KeyValuePair<string, string>("grant_type", "password")
            });

            // Act
            var httpResponse = httpClient.PostAsync("/api/token", bodyData).Result;

            var accessToken = JsonConvert.DeserializeObject<dynamic>(httpResponse.Content.ReadAsStringAsync()
                .Result)["Access_Token"].ToString();

            return accessToken;
        }

        private ApplicationUser TestingUser { get; set; }

        private void CleanDatabase()
        {
            // Clean all data in all database tables
            var dbContext = new NewsContext();

            dbContext.Newses.Count();

            TestingUser = dbContext.Users.First();

            foreach (var news in dbContext.Newses)
            {
                dbContext.Newses.Remove(news);
            }

            dbContext.SaveChanges();

            dbContext.Newses.Add(new News()
            {
                AuthorId = this.TestingUser.Id,
                Title = "Test Title",
                Content = "Test Content",
                PublishDate = DateTime.Now
            });

            dbContext.SaveChanges();
        }
    }
}
