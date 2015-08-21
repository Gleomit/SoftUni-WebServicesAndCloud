using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using BookShopSystem.Data.Repositories;
using BookShopSystem.Models;
using BookShopSystem.Service.Models;
using Microsoft.AspNet.Identity;

namespace BookShopSystem.Service.Controllers
{
    [RoutePrefix("api/books")]
    public class BooksController : ApiController
    {
        private IBookShopData unitOfWork;

        public BooksController()
        {
            unitOfWork = new BookShopData();
        }

        public BooksController(IBookShopData unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetBook([FromUri]int id)
        {
            var book = unitOfWork.BookRepository.GetByID(id);

            if (book == null)
            {
                return this.NotFound();
            }

            return this.Ok(book);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult EditBook([FromUri]int id, [FromBody]EditBookBindingModel bookModel)
        {
            var book = unitOfWork.BookRepository.GetByID(id);

            if (book == null)
            {
                return this.NotFound();
            }

            if (!ModelState.IsValid)
            {
                return this.BadRequest();
            }

            if (unitOfWork.AuthorRepository.GetByID(bookModel.AuthorId) == null)
            {
                return Content(HttpStatusCode.BadRequest, "There is no author with this ID.");
            }

            if (unitOfWork.BookRepository.Get(b => b.Title == bookModel.Title).Any())
            {
                return Content(HttpStatusCode.Conflict, "There is already book with this title.");
            }

            book.Title = bookModel.Title;
            book.Description = bookModel.Description;
            book.Price = bookModel.Price;
            book.Copies = bookModel.Copies;
            book.Edition = bookModel.Edition;
            book.AgeRestriction = bookModel.AgeRestriction;
            book.ReleaseDate = bookModel.ReleaseDate;
            book.AuthorId = bookModel.AuthorId;

            unitOfWork.BookRepository.Update(book);

            unitOfWork.Save();

            return this.Ok();
        }

        [HttpPut]
        [Route("recall/{id}")]
        [Authorize]
        public IHttpActionResult RecallBook([FromUri]int id)
        {
            var book = unitOfWork.BookRepository.GetByID(id);

            if (book == null)
            {
                return this.NotFound();
            }

            var purchase = book.Purchases.First(p => p.ApplicationUserId == this.User.Identity.GetUserId<int>());

            if (purchase == null)
            {
                return this.BadRequest("There is no purchase from you for this book.");
            }

            if (purchase.IsRecalled)
            {
                return this.BadRequest("The book is already recalled.");
            }

            if ((DateTime.Now - purchase.DateOfPurchase).TotalDays > 30)
            {
                return this.BadRequest("You can't recall a purchase after 30 days from the purchase date.");
            }

            purchase.IsRecalled = true;
            book.Copies++;

            unitOfWork.Save();

            return this.Ok();
        }

        [HttpPut]
        [Route("buy/{id}")]
        [Authorize]
        public IHttpActionResult BuyBook([FromUri]int id)
        {
            var book = unitOfWork.BookRepository.GetByID(id);
           
            if (book == null)
            {
                return this.NotFound();
            }

            if (book.Copies > 1)
            {
                unitOfWork.PurchaseRepository.Insert(new Purchase()
                {
                    ApplicationUserId = this.User.Identity.GetUserId<int>(),
                    IsRecalled = false,
                    BookId = book.Id,
                    DateOfPurchase = DateTime.Now,
                    Price = book.Price
                });

                book.Copies--;
                unitOfWork.Save();

                return this.Ok();
            }
            else
            {
                return this.BadRequest("No more copies");
            }    
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteBook([FromUri]int id)
        {
            var book = unitOfWork.BookRepository.GetByID(id);

            if (book == null)
            {
                return this.NotFound();
            }

            unitOfWork.BookRepository.Delete(book);

            unitOfWork.Save();

            return this.Ok();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult CreateBook([FromBody]AddBookBindingModel bookModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Wrong model data.");
            }

            if (unitOfWork.AuthorRepository.GetByID(bookModel.AuthorId) == null)
            {
                return Content(HttpStatusCode.BadRequest, "There is no author with this ID.");
            }

            if (unitOfWork.BookRepository.Get(b => b.Title == bookModel.Title).Any())
            {
                return Content(HttpStatusCode.Conflict, "There is already book with this title.");
            }

            string[] categoryNames = bookModel.Categories.Split(new[] {' '});

            Book book = new Book()
            {
                Title = bookModel.Title,
                Description = bookModel.Description,
                Price = bookModel.Price,
                Copies = bookModel.Copies,
                Edition = bookModel.Edition,
                AgeRestriction = bookModel.AgeRestriction,
                ReleaseDate = bookModel.ReleaseDate,
                AuthorId = bookModel.AuthorId
            };

            foreach (string categoryName in categoryNames)
            {
                if (unitOfWork.CategoryRepository.Get(c => c.Name == categoryName).Any())
                {
                    book.Categories.Add(unitOfWork.CategoryRepository.Get(c => c.Name == categoryName).First());
                }
            }

            unitOfWork.BookRepository.Insert(book);

            unitOfWork.Save();

            return this.Ok(book);
        }
    }
}
