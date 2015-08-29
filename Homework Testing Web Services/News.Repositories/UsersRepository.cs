namespace News.Repositories
{
    using System.Data.Entity;
    using System.Linq;
    using News.Models;
    using News.Repositories.Interfaces;

    public class UsersRepository : IRepository<ApplicationUser>
    {
        private readonly DbContext dbContext;

        public UsersRepository(DbContext context)
        {
            this.dbContext = context;
        }

        public ApplicationUser Add(ApplicationUser entity)
        {
            this.dbContext.Set<ApplicationUser>().Add(entity);

            return entity;
        }

        public ApplicationUser Find(object id)
        {
            return this.dbContext.Set<ApplicationUser>().Find(id);
        }

        public IQueryable<ApplicationUser> All()
        {
            return this.dbContext.Set<ApplicationUser>();
        }

        public void Delete(ApplicationUser entity)
        {
            
        }

        public void Update(ApplicationUser entity)
        {
            throw new System.NotImplementedException();
        }

        public void SaveChanges()
        {
            this.dbContext.SaveChanges();
        }
    }
}