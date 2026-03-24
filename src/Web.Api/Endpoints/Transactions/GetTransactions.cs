using Application.Abstractions.Data;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Domain.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using System.Globalization;

namespace Web.Api.Endpoints.Transactions;

public class GetTransactions : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        ApiVersionSet apiVersionSet = app.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1, 0))
            .HasApiVersion(new ApiVersion(2, 0))
            .ReportApiVersions()
            .Build();

        RouteGroupBuilder group = app.MapGroup("/api/transactions")
            .WithApiVersionSet(apiVersionSet)
            .WithTags("Transactions");

        group.MapGet("/", async (
            IApplicationDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            List<Transaction> data = await dbContext.Transactions
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Results.Ok(data);
        })
        .MapToApiVersion(1, 0)
        .RequireAuthorization();


        group.MapGet("/", (IApplicationDbContext dbContext) =>
        {
            return dbContext.Transactions
            .AsNoTracking()
            .AsAsyncEnumerable();
        })
        .MapToApiVersion(2, 0)
        .RequireAuthorization("PremiumUserPolicy");

        group.MapGet("/summary", async (
            HybridCache cache,
            IApplicationDbContext dbContext,
            CancellationToken ct) =>
        {
            const string cacheKey = "transactions_summary";

            List<TransactionSummaryItem> summary = await cache.GetOrCreateAsync<List<TransactionSummaryItem>>(
                cacheKey,
                async token =>
                {
                    var rawData = await dbContext.Transactions
                        .AsNoTracking()
                        .GroupBy(t => t.Category)
                        .Select(g => new { Category = g.Key, Total = g.Sum(x => x.Amount) })
                        .ToListAsync(token);

                    return rawData
                        .Select(x => new TransactionSummaryItem(
                            x.Category,
                            x.Total.ToString("N2", CultureInfo.GetCultureInfo("tr-TR"))))
                        .ToList();
                },
                tags: ["transactions"],
                options: new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5) },
                cancellationToken: ct);

            return Results.Ok(summary);
        })
        .MapToApiVersion(1, 0)
        .RequireAuthorization();

        group.MapPost("/", async (
            CreateTransactionRequest request,
            IApplicationDbContext dbContext,
            HybridCache cache,
            CancellationToken ct) =>
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Currency = request.Currency,
                Description = request.Description,
                TransactionDate = request.TransactionDate ?? DateTime.UtcNow,
                Status = request.Status,
                MerchantName = request.MerchantName,
                Category = request.Category
            };

            dbContext.Transactions.Add(transaction);
            await dbContext.SaveChangesAsync(ct);
            await cache.RemoveByTagAsync("transactions", ct);

            return Results.Created($"/api/transactions/{transaction.Id}", transaction);
        })
        .MapToApiVersion(1, 0)
        .RequireAuthorization();
    }
}

public record CreateTransactionRequest(
    decimal Amount = 249.99m,
    string Currency = "USD",
    string Description = "Wireless Headphones",
    DateTime? TransactionDate = null,
    string Status = "Completed",
    string MerchantName = "Amazon",
    string Category = "Electronics");

public record TransactionSummaryItem(string Category, string Total);
