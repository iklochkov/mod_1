using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace MyCRM.Infrastructure.Config
{
    public class AppConfig
    {
        public StorageConfig Storage { get; set; } = new();
        public LoggingSettings Logging { get; set; } = new();
        public ExportConfig Export { get; set; } = new();

        public static AppConfig Load()
        {
            var environment = "Production";
#if DEBUG
            environment = "Debug";
#else
                environment = "Release";
#endif

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            var config = new AppConfig();
            configuration.Bind(config);

            return config;
        }

        public void PrintInfo()
        {
            Console.WriteLine("=== Конфигурация ===");
            Console.WriteLine($"Storage Mode: {Storage.Mode}");
            Console.WriteLine($"Log Level: {Logging.LogLevel}");
            Console.WriteLine($"CSV Delimiter: {Export.CsvDelimiter}");
        }
    }

    public class StorageConfig
    {
        public string Mode { get; set; } = "InMemory";
        public string ConnectionString { get; set; } = "";
    }

    public class LoggingSettings
    {
        public bool ConsoleEnabled { get; set; } = true;
        public string LogLevel { get; set; } = "Information";
        public string FilePath { get; set; } = "logs.txt";
    }

    public class ExportConfig
    {
        public string CsvDelimiter { get; set; } = ",";
        public int MaxConcurrentExports { get; set; } = 5;
    }
}