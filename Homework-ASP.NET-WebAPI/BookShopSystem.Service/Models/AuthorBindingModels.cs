using System.ComponentModel.DataAnnotations;

namespace BookShopSystem.Service.Models
{
    public class AddAuthorBindingModel
    {
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}