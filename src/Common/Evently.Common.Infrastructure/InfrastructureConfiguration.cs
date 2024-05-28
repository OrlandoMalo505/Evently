using Dapper;
using Evently.Common.Application.Caching;
using Evently.Common.Application.Data;
using Evently.Common.Application.EventBus;
using Evently.Common.Infrastructure.Authentication;
using Evently.Common.Infrastructure.Authorization;
using Evently.Common.Infrastructure.Caching;
using Evently.Common.Infrastructure.Data;
using Evently.Common.Infrastructure.Outbox;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Npgsql;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Quartz;
using StackExchange.Redis;
using MongoDB.Driver.Core.Extensions.DiagnosticSources;

namespace Evently.Common.Infrastructure;

public static class InfrastructureConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string serviceName,
        Action<IRegistrationConfigurator>[] moduleConfigureConsumers,
        string databaseConnectionString,
        string redisConnectionString,
        string mongoConnectionString)
    {
        services.AddAuthenticationInternal();

        services.AddAuthorizationInternal();

        NpgsqlDataSource npgsqlDataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        services.TryAddSingleton(npgsqlDataSource);

        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();

        SqlMapper.AddTypeHandler(new GenericArrayHandler<string>());

        services.AddQuartz();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);


        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);

            services.TryAddSingleton(connectionMultiplexer);

            services.AddStackExchangeRedisCache(options =>
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer));
        }
        catch
        {
            services.AddDistributedMemoryCache();
        }

        services.TryAddSingleton<ICacheService, CacheService>();

        services.TryAddSingleton<IEventBus, EventBus.EventBus>();

        services.AddMassTransit(config =>
        {

            foreach (var moduleConsumer in moduleConfigureConsumers)
            {
                moduleConsumer(config);
            }

            config.SetKebabCaseEndpointNameFormatter();

            config.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        services
            .AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddRedisInstrumentation()
                    .AddNpgsql()
                    .AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName)
                    .AddSource("MongoDB.Driver.Core.Extensions.DiagnosticSources");

                tracing.AddOtlpExporter();
            });

        var mongoClientSettings = MongoClientSettings.FromConnectionString(mongoConnectionString);

        mongoClientSettings.ClusterConfigurator = c => c.Subscribe(
            new DiagnosticsActivityEventSubscriber(
                new InstrumentationOptions
                {
                    CaptureCommandText = true
                }));

        services.AddSingleton<IMongoClient>(new MongoClient(mongoClientSettings));

#pragma warning disable CS0618 // Type or member is obsolete
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
#pragma warning restore CS0618 // Type or member is obsolete
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        return services;
    }

}
