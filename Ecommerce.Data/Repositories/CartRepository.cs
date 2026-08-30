using System;
using System.Collections.Generic;
using System.Linq;
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

        public IList<CartItem> GetBySessionId(Guid sessionId)
        {
            return _db.CartItems
                .Where(c => c.SessionId == sessionId)
                .OrderBy(c => c.Id)
                .ToList();
        }

        public void SaveAll(Guid sessionId, IList<CartItem> lines)
        {
            var existing = _db.CartItems
                .Where(c => c.SessionId == sessionId)
                .ToList();

            _db.CartItems.RemoveRange(existing);

            foreach (var line in lines)
            {
                line.Id = 0;
                line.SessionId = sessionId;
                _db.CartItems.Add(line);
            }

            _db.SaveChanges();
        }

        public void Clear(Guid sessionId)
        {
            var existing = _db.CartItems
                .Where(c => c.SessionId == sessionId)
                .ToList();

            if (existing.Count > 0)
            {
                _db.CartItems.RemoveRange(existing);
                _db.SaveChanges();
            }
        }
    }
}