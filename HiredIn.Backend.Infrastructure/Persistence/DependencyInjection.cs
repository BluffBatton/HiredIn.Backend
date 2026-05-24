using HiredIn.Backend.Application.Interfaces;
using HiredIn.Backend.Infrastructure.Integration;
using HiredIn.Backend.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HiredIn.Backend.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<ApplicationDbContext>());

            services.Configure<SupabaseStorageOptions>(
                configuration.GetSection(SupabaseStorageOptions.SectionName));

            services.AddScoped<IFileStorageService, SupabaseFileStorageService>();

            services.Configure<OpenAiOptions>(
                configuration.GetSection(OpenAiOptions.SectionName));

            services.AddHttpClient<IOpenAiService, OpenAiService>();

            return services;
        }

        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            services.AddHttpContextAccessor();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IPasswordHasherService, PasswordHasherService>();
            services.AddScoped<IUserContextService, UserContextService>();

            var issuer = configuration["Jwt:Issuer"]
                         ?? throw new InvalidOperationException("Jwt:Issuer is missing");
            var audience = configuration["Jwt:Audience"]
                           ?? throw new InvalidOperationException("Jwt:Audience is missing");
            var key = configuration["Jwt:Key"]
                      ?? throw new InvalidOperationException("Jwt:Key is missing");



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,

                        ValidateAudience = true,
                        ValidAudience = audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;

                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/hubs/chat"))
                            {
                                context.Token = accessToken;
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization();

            return services;
        }
    }
}
