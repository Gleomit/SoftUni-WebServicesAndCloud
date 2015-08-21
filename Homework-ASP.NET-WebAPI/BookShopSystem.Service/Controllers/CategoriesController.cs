using System.Linq;
using System.Net;
using System.Web.Http;
using BookShopSystem.Data.Repositories;
using BookShopSystem.Models;
using BookShopSystem.Service.Models;

namespace BookShopSystem.Service.Controllers
{
    [RoutePrefix("api/categories")]
    public class CategoriesController : ApiController
    {
        private IBookShopData unitOfWork;

        public CategoriesController()
        {
            unitOfWork = new BookShopData();
        }

        public CategoriesController(IBookShopData unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IHttpActionResult GetCategories()
        {
            return this.Ok(unitOfWork.CategoryRepository.Get());
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetCategory([FromUri]int id)
        {
            var category = unitOfWork.CategoryRepository.GetByID(id);

            if (category == null)
            {
                return this.NotFound();
            }

            return this.Ok(category);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult EditCategory([FromUri]int id, [FromBody]AddCategoryBindingModel categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var category = unitOfWork.CategoryRepository.GetByID(id);

            if (category == null)
            {
                return this.NotFound();
            }

            if (unitOfWork.CategoryRepository.Get(c => c.Name == categoryModel.CategoryName).Any())
            {
                return Content(HttpStatusCode.Conflict, "There is already category with this name.");
            }

            category.Name = categoryModel.CategoryName;   

            unitOfWork.CategoryRepository.Update(category);

            unitOfWork.Save();

            return this.Ok(category);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteCategory([FromUri]int id)
        {
            var category = unitOfWork.CategoryRepository.GetByID(id);

            if (category == null)
            {
                return this.NotFound();
            }

            unitOfWork.CategoryRepository.Delete(category);

            unitOfWork.Save();

            return this.Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult CreateCategory([FromBody] AddCategoryBindingModel categoryModel)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var category = new Category()
            {
                Name = categoryModel.CategoryName
            };

            unitOfWork.CategoryRepository.Insert(category);

            unitOfWork.Save();

            return this.Ok(category);
        }
    }
}
