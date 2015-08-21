using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BookShopSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        private ICollection<Purchase> purchases; 

        public ApplicationUser()
        {
            this.purchases = new HashSet<Purchase>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager,
            string authethicationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authethicationType);

            return userIdentity;
        }

        public virtual ICollection<Purchase> Purchases
        {
            get { return this.purchases; }
            set { this.purchases = value; }
        } 
    }
}
