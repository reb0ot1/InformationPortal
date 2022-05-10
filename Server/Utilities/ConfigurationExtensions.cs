using Microsoft.Extensions.Configuration;
using System;

namespace CovidInformationPortal.Server.Utilities
{
    public static class ConfigurationExtensions
    {
        public static string ApplyConnectionString(this IConfiguration configuration)
        {
            var host = Environment.GetEnvironmentVariable("DB_HOST");
            var connectionString = String.Format("Server={0},{1};Database={2};User Id={3};Password={4}",
                Environment.GetEnvironmentVariable("DB_HOST"),
                Environment.GetEnvironmentVariable("DB_PORT"),
                Environment.GetEnvironmentVariable("DB_NAME"),
                Environment.GetEnvironmentVariable("SA_USER"),
                Environment.GetEnvironmentVariable("SA_PASSWORD"));

            if (host == null)
            {
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            return connectionString;
        }
    }
}
