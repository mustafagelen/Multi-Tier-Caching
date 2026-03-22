using Application.Abstractions.Data;
using Asp.Versioning;
using Asp.Versioning.Builder;
using Domain.Transactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

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
        .WithSummary("Get all transactions V1")
        .WithDescription("Classic buffered approach.");


        group.MapGet("/", (IApplicationDbContext dbContext) =>
        {
            return dbContext.Transactions
            .AsNoTracking()
            .AsAsyncEnumerable();
        })
        .MapToApiVersion(2, 0)
        .RequireAuthorization("PremiumUserPolicy")
        .WithSummary("Get all transactions V2")
        .WithDescription("Modern streamed approach.");
    }
}
