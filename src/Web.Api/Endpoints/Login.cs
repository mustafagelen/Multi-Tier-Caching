using Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Web.Api.Endpoints;

public class Login : IEndpoint
{
    private const string HardcodedUsername = "admin";
    private const string HardcodedPassword = "password";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/login", (LoginRequest request, ITokenProvider tokenProvider) =>
        {
            if (request.Username == HardcodedUsername && request.Password == HardcodedPassword)
            {
                string token = tokenProvider.Create(request.Username, request.IsPremium);
                return Results.Ok(new { Token = token });
            }

            return Results.Unauthorized();
        })
        .WithTags("Auth")
        .AllowAnonymous();
    }
}

public record LoginRequest(
    string Username = "admin",
    string Password = "password",
    bool IsPremium = false);
