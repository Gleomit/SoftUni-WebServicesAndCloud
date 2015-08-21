using System;
using BookShopSystem.Models;

namespace BookShopSystem.Data.Repositories
{
    public class BookShopData : IBookShopData, IDisposable
    {
        private BookShopContext context = new BookShopContext();
        private GenericRepository<Book> bookRepository;
        private GenericRepository<Author> authorRepository;
        private GenericRepository<Category> categoryRepository;
        private GenericRepository<Purchase> purchaseRepository;

        public GenericRepository<Book> BookRepository
        {
            get
            {
                if (this.bookRepository == null)
                {
                    this.bookRepository = new GenericRepository<Book>(context);
                }
                return bookRepository;
            }
        }

        public GenericRepository<Author> AuthorRepository
        {
            get
            {
                if (this.authorRepository == null)
                {
                    this.authorRepository = new GenericRepository<Author>(context);
                }
                return authorRepository;
            }
        }

        public GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new GenericRepository<Category>(context);
                }
                return categoryRepository;
            }
        }

        public GenericRepository<Purchase> PurchaseRepository
        {
            get
            {
                if (this.purchaseRepository == null)
                {
                    this.purchaseRepository = new GenericRepository<Purchase>(context);
                }
                return purchaseRepository;
            }
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
