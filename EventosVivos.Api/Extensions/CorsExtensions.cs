using Microsoft.Extensions.DependencyInjection;

namespace EventosVivos.Api.Extensions;

public static class CorsExtensions
{
    public const string AngularPolicy = "AllowAngularApp";

    public static IServiceCollection AddCustomCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(AngularPolicy, builder =>
            {
                builder.WithOrigins("http://localhost:4200", "http://localhost:42000")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        return services;
    }
}
