using Duende.IdentityServer;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using wallet.api.Features.DataAccess;
using wallet.api.Features.IdentityServer;

namespace wallet.api.Features.Core;
public static class ServiceCollectionExtensions 
{
    private static void InitializeDatabase(IApplicationBuilder app)
    {
        using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }

    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        builder.Services.AddDbContext<WalletDbContext>(options =>
        {
            options.EnableSensitiveDataLogging();

            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), b =>
            {
                b.MigrationsAssembly(typeof(WalletDbContext).Assembly.FullName);
                b.MigrationsHistoryTable(HistoryRepository.DefaultTableName);
            });
        });

        builder.Services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = Consts.Bearer;
            opts.DefaultChallengeScheme = Consts.Bearer;
        })
            .AddJwtBearer(Consts.Bearer, options =>
            {
                options.Authority = configuration.GetSection("IdentityProperties")["ValidIssuer"];
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetSection("IdentityProperties")["ValidIssuer"],
                };
                options.RequireHttpsMetadata = false;
            })
            .AddCookie("Cookies");

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = 429;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                if (httpContext.Request.Path == "/.well-known/openid-configuration") 
                {
                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: "LoginWindow",
                        factory: partition => new FixedWindowRateLimiterOptions
                        {
                            AutoReplenishment = true,
                            PermitLimit = 3, 
                            QueueLimit = 0,
                            Window = TimeSpan.FromMinutes(1)
                        });
                }

                return RateLimitPartition.GetNoLimiter("_defaultPolicyKey");
            });
        });

        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
        string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;

        builder.Services.AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b => b.UseNpgsql(connectionString,
                    sql => sql.MigrationsAssembly(migrationsAssembly));
            });

        var authenticationBuilder = builder.Services.AddAuthentication();

        authenticationBuilder.AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
        {
            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            options.SignOutScheme = IdentityServerConstants.SignoutScheme;
            options.SaveTokens = true;

            options.Authority = "https://demo.duendesoftware.com";
            options.ClientId = "interactive.confidential";
            options.ClientSecret = "secret";
            options.ResponseType = "code";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "role"
            };
        });

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        InitializeDatabase(app);
        app.UseStaticFiles();
        app.UseRouting();
        
        app.UseIdentityServer();

        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
