
using UserProduct.Domain.Entities;

namespace UserProduct.Core.Abstractions
{
    public interface IRepository
    {
        public Task Add<TEntity>(TEntity entity) where TEntity : Entity;

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : Entity;

        public Task<TEntity?> FindById<TEntity>(string id) where TEntity : Entity;

        public void Update<TEntity>(TEntity entity) where TEntity : Entity;

        public void Remove<TEntity>(TEntity entity) where TEntity : Entity;
    }
}
