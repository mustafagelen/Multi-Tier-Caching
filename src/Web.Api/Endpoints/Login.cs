using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Domain.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Web.Api.Endpoints;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (LoginRequest request, ITokenProvider tokenProvider, IApplicationDbContext dbContext, CancellationToken ct) =>
        {
            User? user = await dbContext.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username && u.PasswordHash == request.Password, ct);

            if (user is not null)
            {
                string token = tokenProvider.Create(user.Username);
                return Results.Ok(new { Token = token });
            }

            return Results.Unauthorized();
        })
        .WithTags("Auth")
        .AllowAnonymous();
    }
}

public record LoginRequest(string Username, string Password);
