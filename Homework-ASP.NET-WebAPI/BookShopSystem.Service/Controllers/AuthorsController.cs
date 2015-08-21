using System.Linq;
using System.Web.Http;
using BookShopSystem.Data.Repositories;
using BookShopSystem.Models;
using BookShopSystem.Service.Models;

namespace BookShopSystem.Service.Controllers
{
    [RoutePrefix("api/authors")]
    public class AuthorsController : ApiController
    {
        private IBookShopData unitOfWork;

        public AuthorsController()
        {
            unitOfWork = new BookShopData();
        }

        public AuthorsController(IBookShopData unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetAuthor([FromUri] int id)
        {
            var author = unitOfWork.AuthorRepository.GetByID(id);

            if (author == null)
            {
                return NotFound();
            }

            return this.Ok(author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult CreateAuthor([FromBody]AddAuthorBindingModel authorModel)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var author = new Author()
            {
                FirstName = authorModel.FirstName,
                LastName = authorModel.LastName
            };

            unitOfWork.AuthorRepository.Insert(author);

            unitOfWork.Save();

            return this.Ok(author);
        }

        [HttpGet]
        [Route("{id}/books")]
        public IHttpActionResult GetAuthorBooks([FromUri] int id)
        {
            var author = unitOfWork.AuthorRepository.GetByID(id);
                
            if (author == null)
            {
                return this.NotFound();
            }

            return this.Ok(author.Books.ToList());
        }
    }
}
