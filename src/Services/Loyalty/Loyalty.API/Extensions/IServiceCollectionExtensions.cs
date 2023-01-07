﻿using Autofac;
using Loyalty.API.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBus.Abstractions;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusRabbitMQ;
using Microsoft.eShopOnContainers.BuildingBlocks.EventBusServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Loyalty.API.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddLoyaltyServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
        {
            services.AddSingleton<IServiceBusPersisterConnection>(sp =>
            {
                var serviceBusConnectionString = configuration["EventBusConnection"];

                return new DefaultServiceBusPersisterConnection(serviceBusConnectionString);
            });
        }
        else
        {
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
            {
                ILogger<DefaultRabbitMQPersistentConnection> logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                var factory = new ConnectionFactory()
                {
                    HostName = configuration["EventBusConnection"],
                    DispatchConsumersAsync = true
                };

                if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
                {
                    factory.UserName = configuration["EventBusUserName"];
                }

                if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
                {
                    factory.Password = configuration["EventBusPassword"];
                }

                var retryCount = 5;
                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new DefaultRabbitMQPersistentConnection(factory, logger, retryCount);
            });
        }

        services/*.AddTransient<ICouponRepository, CouponRepository>()*/
            .AddTransient<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            //.AddTransient<CouponContext>()
            //.AddTransient<IMapper<CouponDto, Coupon>, Mapper>();

        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "eShopOnContainers - Loyalty HTTP API",
                Version = "v1",
                Description = "The Loyalty Service HTTP API"
            });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                        TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                        Scopes = new Dictionary<string, string>()
                        {
                            { "loyalty", "Loyalty API" }
                        }
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>();
        });

        return services;
    }

    public static IServiceCollection AddCustomSettings(this IServiceCollection services, IConfiguration configuration)
    {
        //services.Configure<CouponSettings>(configuration);
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Instance = context.HttpContext.Request.Path,
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Please refer to the errors property for additional details."
                };

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json", "application/problem+xml" }
                };
            };
        });

        return services;
    }

    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration configuration)
    {
        var subscriptionClientName = configuration["SubscriptionClientName"];

        if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
        {
            services.AddSingleton<IEventBus, EventBusServiceBus>(sp =>
            {
                IServiceBusPersisterConnection serviceBusPersisterConnection = sp.GetRequiredService<IServiceBusPersisterConnection>();
                ILifetimeScope iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                ILogger<EventBusServiceBus> logger = sp.GetRequiredService<ILogger<EventBusServiceBus>>();
                IEventBusSubscriptionsManager eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                return new EventBusServiceBus(
                    serviceBusPersisterConnection,
                    logger,
                    eventBusSubcriptionsManager,
                    iLifetimeScope,
                    subscriptionClientName);
            });
        }
        else
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>(sp =>
            {
                IRabbitMQPersistentConnection rabbitMQPersistentConnection = sp.GetRequiredService<IRabbitMQPersistentConnection>();
                ILifetimeScope iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
                ILogger<EventBusRabbitMQ> logger = sp.GetRequiredService<ILogger<EventBusRabbitMQ>>();
                IEventBusSubscriptionsManager eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

                var retryCount = 5;

                if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
                {
                    retryCount = int.Parse(configuration["EventBusRetryCount"]);
                }

                return new EventBusRabbitMQ(rabbitMQPersistentConnection, logger, iLifetimeScope, eventBusSubcriptionsManager, subscriptionClientName, retryCount);
            });
        }

        return services;
    }

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        //var accountName = configuration.GetValue<string>("AzureStorageAccountName");
        //var accountKey = configuration.GetValue<string>("AzureStorageAccountKey");

        IHealthChecksBuilder hcBuilder = services.AddHealthChecks();

        hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy())
            .AddMongoDb(
                configuration["ConnectionString"],
                name: "CouponCollection-check",
                tags: new string[] { "couponcollection" });

        if (configuration.GetValue<bool>("AzureServiceBusEnabled"))
        {
            hcBuilder.AddAzureServiceBusTopic(
                configuration["EventBusConnection"],
                topicName: "eshop_event_bus",
                name: "loyalty-servicebus-check",
                tags: new string[] { "servicebus" });
        }
        else
        {
            hcBuilder.AddRabbitMQ(
                $"amqp://{configuration["EventBusConnection"]}",
                name: "loyalty-rabbitmqbus-check",
                tags: new string[] { "rabbitmqbus" });
        }

        return services;
    }

    public static IServiceCollection AddCustomPolicies(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder.SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        return services;
    }

    public static IServiceCollection AddAppInsights(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsightsTelemetry(configuration);
        services.AddApplicationInsightsKubernetesEnricher();

        return services;
    }

    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = configuration["IdentityUrl"];
            options.RequireHttpsMetadata = false;
            options.Audience = "loyalty";
        }).Services;
    }

    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services) => services.AddAuthorization();
}
