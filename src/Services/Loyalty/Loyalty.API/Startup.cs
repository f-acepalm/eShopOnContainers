using HealthChecks.UI.Client;
using Loyalty.API.DataAccess;
using Loyalty.API.Extensions;
using Loyalty.API.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace Loyalty.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(options => options.Filters.Add<ValidateModelAttribute>());

        services.AddCustomSettings(Configuration)
            .AddCustomDbContext(Configuration)
            .AddLoyaltyServices(Configuration)
            .AddCustomPolicies()
            .AddAppInsights(Configuration)
            .AddEventBus(Configuration)
            .AddCustomAuthentication(Configuration)
            .AddCustomAuthorization()
            .AddSwagger(Configuration)
            .AddCustomHealthCheck(Configuration);

        //services.AddTransient<IIntegrationEventHandler<OrderStatusChangedToAwaitingCouponValidationIntegrationEvent>, OrderStatusChangedToAwaitingCouponValidationIntegrationEventHandler>();
        //services.AddTransient<IIntegrationEventHandler<OrderStatusChangedToCancelledIntegrationEvent>, OrderStatusChangedToCancelledIntegrationEventHandler>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        var pathBase = Configuration["PATH_BASE"];

        if (!string.IsNullOrEmpty(pathBase))
        {
            app.UsePathBase(pathBase);
        }

        app.UseSwagger()
            .UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"{(!string.IsNullOrEmpty(pathBase) ? pathBase : string.Empty)}/swagger/v1/swagger.json", "Loyalty.API V1");
                options.OAuthClientId("loyaltyswaggerui");
                options.OAuthAppName("eShop-Learn.Loyalty.API Swagger UI");
            })
            .UseCors("CorsPolicy")
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/hc", new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
                {
                    Predicate = r => r.Name.Contains("self")
                });
            });

        ConfigureEventBus(app);
    }   

    private void ConfigureEventBus(IApplicationBuilder app)
    {
        IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

        //eventBus.Subscribe<OrderStatusChangedToCancelledIntegrationEvent, IIntegrationEventHandler<OrderStatusChangedToCancelledIntegrationEvent>>();
    }
}
