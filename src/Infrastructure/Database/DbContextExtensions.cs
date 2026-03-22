using Bogus;
using Domain.Transactions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

public static class DbContextExtensions
{
    public static async Task SeedDataAsync(this ApplicationDbContext context)
    {
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            await context.Database.MigrateAsync();
        }

        if (await context.Transactions.AnyAsync())
        {
            return;
        }

        Faker<Transaction> faker = new Faker<Transaction>()
            .RuleFor(t => t.Id, f => Guid.NewGuid())
            .RuleFor(t => t.Amount, f => f.Finance.Amount(10, 10000))
            .RuleFor(t => t.Currency, f => f.Finance.Currency().Code)
            .RuleFor(t => t.Description, f => f.Commerce.ProductDescription())
            .RuleFor(t => t.TransactionDate, f => f.Date.Past(2).ToUniversalTime())
            .RuleFor(t => t.Status, f => f.PickRandom("Completed", "Pending", "Failed"))
            .RuleFor(t => t.MerchantName, f => f.Company.CompanyName())
            .RuleFor(t => t.Category, f => f.Commerce.Categories(1)[0]);

        const int totalRecords = 100_000;
        const int batchSize = 5000;
        
        for (int i = 0; i < totalRecords / batchSize; i++)
        {
            List<Transaction> transactions = faker.Generate(batchSize);
            await context.Transactions.AddRangeAsync(transactions);
            await context.SaveChangesAsync();
            
            context.ChangeTracker.Clear();
        }
    }
}
