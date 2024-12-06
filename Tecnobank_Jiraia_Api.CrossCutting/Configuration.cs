using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;

namespace Tecnobank_Jiraia_Api.CrossCutting
{
    public class Configuration
    {
        private readonly IConfiguration _configuration;

        public Configuration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ChatGptApiKey
        {
            get
            {
                return _configuration["ChatGpt:ApiKey"];
            }
        }

        public string ChatGptApiUrlCompletions
        {
            get
            {
                return _configuration["ChatGpt:ApiUrlCompletions"];
            }
        }

        public int ChatGptMaxRetries
        {
            get
            {
                return int.Parse(_configuration["ChatGpt:MaxRetries"]);
            }
        }

        public int ChatGptMaxDelay
        {
            get
            {
                return int.Parse(_configuration["ChatGpt:MaxDelay"]);
            }
        }

        public string ChatGptModel
        {
            get
            {
                return _configuration["ChatGpt:Model"];
            }
        }

        public int ChatGptMaxTokens
        {
            get
            {
                return int.Parse(_configuration["ChatGpt:MaxTokens"]);
            }
        }

        public double ChatGptTemperature
        {
            get
            {
                return double.Parse(_configuration["ChatGpt:Temperature"], CultureInfo.InvariantCulture);
            }
        }

    }
}
