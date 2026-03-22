using Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<Transaction> Transactions { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
