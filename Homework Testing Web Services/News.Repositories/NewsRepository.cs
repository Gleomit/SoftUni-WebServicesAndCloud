namespace News.Repositories
{
    using System.Linq;
    using News.Repositories.Interfaces;

    using System.Data.Entity;
    using News.Models;

    public class NewsRepository : IRepository<News>
    {
        private readonly DbContext dbContext;

        public NewsRepository(DbContext context)
        {
            this.dbContext = context;
        }

        public News Add(News entity)
        {
            this.dbContext.Set<News>().Add(entity);

            return entity;
        }

        public News Find(object id)
        {
            return this.dbContext.Set<News>().Find(id);
        }

        public IQueryable<News> All()
        {
            return this.dbContext.Set<News>();
        }

        public void Delete(News entity)
        {
            this.ChangeState(entity, EntityState.Deleted);
        }

        public void Update(News entity)
        {
            this.ChangeState(entity, EntityState.Modified);
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }

        private void ChangeState(News news, EntityState state)
        {
            var entry = this.dbContext.Entry(news);
            if (entry.State == EntityState.Detached)
            {
                this.dbContext.Set<News>().Attach(news);
            }

            entry.State = state;
        }
    }
}