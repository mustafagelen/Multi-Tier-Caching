using Scalar.AspNetCore;

namespace Web.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseScalarUi(this WebApplication app)
    {
        app.MapScalarApiReference(options =>
        {
            options.Title = "Web.Api";
            options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
        });

        return app;
    }
}
