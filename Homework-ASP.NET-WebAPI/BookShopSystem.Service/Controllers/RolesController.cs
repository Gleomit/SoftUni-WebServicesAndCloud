using System.Linq;
using System.Web.Http;
using BookShopSystem.Data;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookShopSystem.Service.Controllers
{
    [RoutePrefix("api/users/{username/roles}")]
    public class RolesController : ApiController
    {
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AddRoleToUser([FromUri] string username, [FromBody]string roleName)
        {
            using (var context = new BookShopContext())
            {
                if (!context.Users.Any(u => u.UserName == username))
                {
                    return this.NotFound();
                }

                var user = context.Users.First(u => u.UserName == username);

                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    return this.NotFound();
                }

                var role = context.Roles.First(r => r.Name == roleName);

                role.Users.Add(new IdentityUserRole()
                {
                    UserId = user.Id
                });

                context.SaveChanges();

                return this.Ok();
            }       
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult RemoveUserRole([FromUri] string username, [FromBody] string roleName)
        {
            using (var context = new BookShopContext())
            {
                if (!context.Users.Any(u => u.UserName == username))
                {
                    return this.NotFound();
                }

                var user = context.Users.First(u => u.UserName == username);

                if (!context.Roles.Any(r => r.Name == roleName))
                {
                    return this.NotFound();
                }

                var role = context.Roles.First(r => r.Name == roleName);

                var identityUserRole = role.Users.First(u => u.UserId == user.Id);

                role.Users.Remove(identityUserRole);

                context.SaveChanges();

                return this.Ok();
            }
        }
    }
}
