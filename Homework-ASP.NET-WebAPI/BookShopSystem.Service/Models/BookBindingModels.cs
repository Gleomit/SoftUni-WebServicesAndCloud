using System;
using BookShopSystem.Models;

namespace BookShopSystem.Service.Models
{
    public class AddBookBindingModel : EditBookBindingModel
    {
        public string Categories { get; set; }
    }

    public class EditBookBindingModel
    {
        public string Title { get; set; }
        
        public string Description { get; set; }

        public Decimal Price { get; set; }

        public int Copies { get; set; }

        public Edition Edition { get; set; }

        public AgeRestriction AgeRestriction { get; set; }

        public DateTime ReleaseDate { get; set; }

        public int AuthorId { get; set; }
    }
}