using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Web.Api.Endpoints;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/login", (LoginRequest request, ITokenProvider tokenProvider) =>
        {
            if (request.Username == "admin" && request.Password == "password123")
            {
                string token = tokenProvider.Create(request.Username);
                return Results.Ok(new { Token = token });
            }

            return Results.Unauthorized();
        })
        .WithTags("Auth");
    }
}

public record LoginRequest(string Username, string Password);
