using Microsoft.Extensions.DependencyInjection;
using Tecnobank_Jiraia_Api.CrossCutting.Api;
using Tecnobank_Jiraia_Api.CrossCutting.Util;
using Tecnobank_Jiraia_Api.CrossCutting.Validation;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Api;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Services;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Util;
using Tecnobank_Jiraia_Api.Domain.Interfaces.Validation;
using Tecnobank_Jiraia_Api.Service;

namespace Teckey_KeyReport.CrossCutting.DependencyInjection
{
    public static class ConfigureService
    {
        public static void ConfigureDependenciesService(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IChatGptService, ChatGptService>();
            serviceCollection.AddTransient<IMonitorNormativoService, MonitorNormativoService>();

            serviceCollection.AddTransient<IChatGptApi, ChatGptApi>();
            serviceCollection.AddTransient<IUtilidade, Utilidade>();

            serviceCollection.AddTransient<IChatGptValidation, ChatGptValidation>();
        }
    }
}
