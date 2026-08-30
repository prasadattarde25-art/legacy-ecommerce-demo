using Ecommerce.Core.Interfaces;
using Ecommerce.Data;

namespace Ecommerce.Data.Infrastructure
{
    /// <summary>
    /// Optional transaction wrapper around a single EcommerceDbContext.
    /// Repositories share the same per-request instance through DI, so
    /// SaveChanges() commits one logical unit of work per HTTP request.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommerceDbContext _db;

        public UnitOfWork(EcommerceDbContext db)
        {
            _db = db;
        }

        public int Save()
        {
            return _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}