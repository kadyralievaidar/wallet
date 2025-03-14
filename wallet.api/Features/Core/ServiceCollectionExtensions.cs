using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using wallet.api.Features.DataAccess;
using wallet.api.Features.DataAccess.Models;
using wallet.api.Features.DataAccess.Repositories;
using wallet.api.Features.DataAccess.UOW;
using wallet.api.Features.IdentityServer;

namespace wallet.api.Features.Core;
public static class ServiceCollectionExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<WalletDbContext>()
            .AddDefaultTokenProviders();
        builder.Services.AddScoped<IUserBalanceRepository, UserBalanceRepository>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
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

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Wallet-Api", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите JWT токен в формате: Bearer {токен}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new List<string>()
                }
            });
        });

        builder.Services.AddIdentityServer()
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiScopes(Config.GetApiScopes())
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddDeveloperSigningCredential()
                .AddProfileService<ProfileService>()
                .AddAspNetIdentity<ApplicationUser>();

        builder.Services.AddAuthorization();

        return builder.Build();
    }
}
