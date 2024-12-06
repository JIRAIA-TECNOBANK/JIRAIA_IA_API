using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using Tecnobank_Jiraia_Api.Data.Context;
using Tecnobank_Jiraia_Api.Data.Repository;
using Tecnobank_Jiraia_Api.Domain.Repository;

namespace Tecnobank_Jiraia_Api.CrossCutting.DependencyInjection
{
    public static class ConfigureRepository
    {
        public static void ConfigureDependenciesRepository(IServiceCollection serviceCollection, string regulatorioConnectionString) 
        {
            serviceCollection.AddTransient<IChatGptRepository, ChatGptRepository>();
            serviceCollection.AddTransient<IMonitorNormativoRepository, MonitorNormativoRepository>();

            serviceCollection.AddDbContext<RegulatorioContext>(opt =>
            {
                opt.UseSqlServer(regulatorioConnectionString, options =>
                {
                    options.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null
                    );
                    options.CommandTimeout(120);
                });
                opt.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Singleton);
        }
    }
}
