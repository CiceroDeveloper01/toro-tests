using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Toro.Testes.Application.Interfaces;
using Toro.Testes.BuildingBlocks.Constants;
using Toro.Testes.BuildingBlocks.Extensions;
using Toro.Testes.Infrastructure.Authentication;
using Toro.Testes.Infrastructure.Data;
using Toro.Testes.Infrastructure.Health;
using Toro.Testes.Infrastructure.Messaging;
using Toro.Testes.Infrastructure.Observability;
using Toro.Testes.Infrastructure.Repositories;
using Toro.Testes.Infrastructure.Security;
using Toro.Testes.Infrastructure.Services;

namespace Toro.Testes.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string serviceName, bool includeAuth = true)
    {
        services.AddToroBuildingBlocks();
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSql")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IInvestmentProductRepository, InvestmentProductRepository>();
        services.AddScoped<IInvestmentOrderRepository, InvestmentOrderRepository>();
        services.AddScoped<IPortfolioPositionRepository, PortfolioPositionRepository>();
        services.AddScoped<IProcessedMessageRepository, ProcessedMessageRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IOutboxSerializer, OutboxSerializer>();
        services.AddScoped<IInvestmentOrderProcessingService, InvestmentOrderProcessingService>();
        services.AddSingleton<IRabbitMqConnectionProvider, RabbitMqConnectionProvider>();
        services.AddSingleton<IMessageBusPublisher, RabbitMqMessageBusPublisher>();
        services.AddHostedService<OutboxDispatcherService>();

        services.AddHttpContextAccessor();
        services.AddToroObservability(configuration, serviceName);
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("postgresql")
            .AddCheck<RabbitMqHealthCheck>("rabbitmq");

        if (includeAuth)
        {
            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(ApplicationConstants.Policies.AdminOnly, policy => policy.RequireRole(ApplicationConstants.Roles.Admin));
                options.AddPolicy(ApplicationConstants.Policies.InvestorAccess, policy => policy.RequireRole(ApplicationConstants.Roles.Investor, ApplicationConstants.Roles.Admin));
            });
        }

        return services;
    }
}
