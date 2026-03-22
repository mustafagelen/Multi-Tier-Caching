using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Web.Api.OpenApi;

internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
    public async Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        IEnumerable<AuthenticationScheme> authenticationSchemes =
            await authenticationSchemeProvider.GetAllSchemesAsync();

        if (!authenticationSchemes.Any(s => s.Name == JwtBearerDefaults.AuthenticationScheme))
        {
            return;
        }

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            Description = "JWT Bearer token"
        };

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document),
                []
            }
        };

        foreach (OpenApiOperation operation in document.Paths?.Values
            .SelectMany(p => (IEnumerable<OpenApiOperation>?)p.Operations?.Values ?? []) ?? [])
        {
            operation.Security ??= [];
            operation.Security.Add(securityRequirement);
        }
    }
}
