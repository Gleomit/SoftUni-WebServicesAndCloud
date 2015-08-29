namespace News.Services.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using News.Repositories.Interfaces;

    public class RepositoryMock<T> : IRepository<T>
    {
        public RepositoryMock()
        {
            this.Entities = new Dictionary<object, T>();
        }

        public Dictionary<object, T> Entities { get; set; }

        public bool IsSaveCalled { get; set; }

        public T Add(T entity)
        {
            this.Entities.Add((entity as dynamic).Id, entity);

            return entity;
        }

        public T Find(object id)
        {
            var entity = this.Entities[id];
            return entity;
        }

        public IQueryable<T> All()
        {
            return this.Entities.Values.AsQueryable();
        }

        public void Delete(T entity)
        {
            var theEntity = this.Entities[(entity as dynamic).Id];

            if (theEntity != null)
            {
                this.Entities.Remove((theEntity as dynamic).Id);
            }
        }

        public void Update(T entity)
        {
            var theEntity = this.Entities[(entity as dynamic).Id];

            if (theEntity != null)
            {
                this.Entities[(theEntity as dynamic).Id] = entity;
            }
        }

        public void SaveChanges()
        {
            this.IsSaveCalled = true;
        }
    }
}