using System;
using System.Collections.Generic;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface ICartRepository
    {
        IList<CartItem> GetBySessionId(Guid sessionId);

        void SaveAll(Guid sessionId, IList<CartItem> lines);

        void Clear(Guid sessionId);
    }
}