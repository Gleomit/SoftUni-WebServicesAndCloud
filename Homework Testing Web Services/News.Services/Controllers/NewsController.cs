namespace News.Services.Controllers
{
    using System.Linq;
    using System.Web.Http;
    using News.Data;
    using News.Services.Models;
    using Newtonsoft.Json;

    using News.Models;

    using System.Net;
    using Microsoft.AspNet.Identity;
    using News.Repositories;
    using News.Repositories.Interfaces;


    [RoutePrefix("api/news/")]
    [Authorize]
    public class NewsController : ApiController
    {
        private IRepository<News> repo;

        public NewsController()
            : this(new NewsRepository(new NewsContext()))
        {
            
        }

        public NewsController(IRepository<News> repository)
        {
            this.repo = repository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult GetAllNews()
        {
            var news = this.repo.All()
                .OrderByDescending(n => n.PublishDate)
                .ToList();

            return this.Ok(JsonConvert.SerializeObject(news));
        }

        [HttpPost]
        public IHttpActionResult CreateNews([FromBody]NewsBindingModel model)
        {
            if (model == null)
            {
                return this.BadRequest("Model cannot be null.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Missing parameters.");
            }

            var userId = this.User.Identity.GetUserId();

            var news = new News()
            {
                Title = model.Title,
                Content = model.Content,
                PublishDate = model.PublishDate,
                AuthorId = userId
            };

            this.repo.Add(news);

            this.repo.SaveChanges();

            return this.Content(HttpStatusCode.Created, news);
        }

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditNews([FromUri]int id, [FromBody]NewsBindingModel model)
        {
            var news = this.repo.Find(id);

            if (news == null)
            {
                return this.NotFound();
            }

            if (model == null)
            {
                return this.BadRequest("Model cannot be null.");
            }

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Missing parameters.");
            }

            var userId = this.User.Identity.GetUserId();

            if (news.AuthorId != userId)
            {
                return this.Unauthorized();
            }

            news.Title = model.Title;
            news.Content = model.Content;
            news.PublishDate = model.PublishDate;

            this.repo.Update(news);

            this.repo.SaveChanges();

            return this.Ok(news);
        }

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeletelNews([FromUri]int id)
        {
            var news = this.repo.Find(id);

            if (news == null)
            {
                return this.BadRequest();
            }

            var userId = this.User.Identity.GetUserId();

            if (news.AuthorId != userId)
            {
                return this.Unauthorized();
            }

            this.repo.Delete(news);

            this.repo.SaveChanges();

            return this.Ok();
        }
    }
}