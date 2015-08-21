using BookShopSystem.Models;

namespace BookShopSystem.Data.Repositories
{
    public interface IBookShopData
    {
        GenericRepository<Book> BookRepository { get;}
        GenericRepository<Author> AuthorRepository { get;}
        GenericRepository<Category> CategoryRepository { get;}
        GenericRepository<Purchase> PurchaseRepository { get; }

        void Save();
    }
}
