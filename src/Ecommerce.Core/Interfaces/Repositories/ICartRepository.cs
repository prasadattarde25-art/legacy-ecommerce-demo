using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<IList<CartItem>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

        Task SaveAllAsync(Guid sessionId, IList<CartItem> lines, CancellationToken cancellationToken = default);

        Task ClearAsync(Guid sessionId, CancellationToken cancellationToken = default);
    }
}
