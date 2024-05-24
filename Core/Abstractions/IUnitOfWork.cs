
namespace UserProduct.Core.Abstractions
{
    public interface IUnitOfWork
    {
        public Task<int> SaveChangesAsync();
    }
}
