using Domain.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Transactions;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Amount).HasPrecision(18, 2);

        builder.HasIndex(x => x.TransactionDate);
        
        builder.Property(x => x.MerchantName).HasMaxLength(255);
    }
}
