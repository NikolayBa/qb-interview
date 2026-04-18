using Backend.Services.CountryNormalization;
using Backend.Services.Database;
using Backend.Services.StatService;
using Microsoft.Extensions.DependencyInjection;

namespace Backend.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Singleton: DB connection manager - reuse connections efficiently for the lifetime of the app
            services.AddSingleton<IDbManager, SqliteDbManager>();
            
            // Singleton: Stateless API service - can be safely reused across requests
            services.AddSingleton<IStatService, ConcreteStatService>();
            
            // Singleton: Stateless normalization utility - no mutable state, safe to reuse
            services.AddSingleton<ICountryNormalizationService, CountryNormalizationService>();
            
            // Singleton: Data access service - depends only on Singletons, safe for reuse
            services.AddSingleton<IDbService, DbService>();
            
            // Transient: Orchestrator - created once per request, used once, then discarded
            services.AddTransient<CountryPopulationAggregator>();
            
            return services;
        }
    }
}
