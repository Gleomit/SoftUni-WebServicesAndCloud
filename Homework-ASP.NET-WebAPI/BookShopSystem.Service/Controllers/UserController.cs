using System.Linq;
using System.Web.Http;
using BookShopSystem.Data;

namespace BookShopSystem.Service.Controllers
{
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        [HttpGet]
        [Route("{username}/purchases")]
        public IHttpActionResult GetUserPurchases([FromUri]string username)
        {
            using (var bookShopContext = new BookShopContext())
            {
                var user = bookShopContext.Users.First(u => u.UserName == username);

                if (user == null)
                {
                    return this.NotFound();
                }

                var result = new
                {
                    username = username,
                    purchases = from purchase in user.Purchases
                                select new
                                {
                                    bookTitle = purchase.Book.Title,
                                    price = purchase.Price,
                                    dateOfPurchase = purchase.DateOfPurchase,
                                    isRecalled = purchase.IsRecalled
                                }
                };

                return this.Ok(result);
            }
        }
    }
}
