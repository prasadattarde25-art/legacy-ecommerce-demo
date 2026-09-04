using System.Threading.Tasks;
using Ecommerce.Core.Interfaces;

namespace Ecommerce.Data.Infrastructure
{
    /// <summary>
    /// Transaction wrapper around a single EcommerceDbContext.
    /// Repositories share the same scoped instance through DI, so
    /// SaveAsync() commits one logical unit of work per HTTP request.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommerceDbContext _db;

        public UnitOfWork(EcommerceDbContext db)
        {
            _db = db;
        }

        public Task<int> SaveAsync(CancellationToken cancellationToken = default)
        {
            return _db.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
