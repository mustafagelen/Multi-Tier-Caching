using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasIndex(x => x.Username).IsUnique();
        builder.Property(x => x.Username).HasMaxLength(50).IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Email).HasMaxLength(150).IsRequired();

        builder.Property(x => x.PasswordHash).IsRequired();
    }
}
