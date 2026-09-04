using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces.Repositories;

namespace Ecommerce.Data.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly EcommerceDbContext _db;

        public CartRepository(EcommerceDbContext db)
        {
            _db = db;
        }

        public async Task<IList<CartItem>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            return await _db.CartItems
                .AsNoTracking()
                .Where(c => c.SessionId == sessionId)
                .OrderBy(c => c.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task SaveAllAsync(Guid sessionId, IList<CartItem> lines, CancellationToken cancellationToken = default)
        {
            var existing = await _db.CartItems
                .Where(c => c.SessionId == sessionId)
                .ToListAsync(cancellationToken);

            _db.CartItems.RemoveRange(existing);

            // Clone into fresh entities to avoid EF flipping tracked instances
            // from Deleted back to Added when they are re-added (which would
            // both skip the DELETE and re-INSERT every line, duplicating rows).
            foreach (var line in lines)
            {
                _db.CartItems.Add(new CartItem
                {
                    SessionId = sessionId,
                    ProductId = line.ProductId,
                    ProductName = line.ProductName,
                    Sku = line.Sku,
                    UnitPrice = line.UnitPrice,
                    Quantity = line.Quantity,
                    CreatedAt = line.CreatedAt,
                    UpdatedAt = line.UpdatedAt
                });
            }

            await _db.SaveChangesAsync(cancellationToken);
        }

        public async Task ClearAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            var existing = await _db.CartItems
                .Where(c => c.SessionId == sessionId)
                .ToListAsync(cancellationToken);

            if (existing.Count > 0)
            {
                _db.CartItems.RemoveRange(existing);
                await _db.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
