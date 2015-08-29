namespace News.Models
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class ApplicationUser : IdentityUser
    {
        private ICollection<News> newses;

        public ApplicationUser()
        {
            this.newses = new HashSet<News>();
        }

        public virtual ICollection<News> Newses
        {
            get { return this.newses; }
            set { this.newses = value; }
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(
            UserManager<ApplicationUser> manager,
            string authType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authType);

            return userIdentity;
        }
    }
}
