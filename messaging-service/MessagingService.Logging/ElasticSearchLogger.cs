using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;

namespace MessagingService.Logging
{
    public class ElasticSearchLogger : ILogger
    {
        public ElasticSearchLogger(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(ConfigureElasticSearchSink(configuration))
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

        }

        private ElasticsearchSinkOptions ConfigureElasticSearchSink(IConfiguration configuration)
        {
            return new ElasticsearchSinkOptions(new Uri(configuration.GetValue<string>("ElasticConfiguration:Uri")))
            {
                AutoRegisterTemplate = true,
                IndexFormat = configuration.GetValue<string>("ElasticConfiguration:IndexFormat")
            };
        }


        public void Error(string error, Exception exception)
        {
            try
            {
                Log.Logger.Error(exception, error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Couldn't log to elastic : {ex.Message}", ex);
            }
        }

        public void Error(string error)
        {
            try
            {
                Log.Logger.Error(error);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Couldn't log to elastic : {ex.Message}", ex);
            }
        }
        public void Info(string info)
        {
            try
            {
                Log.Logger.Information(info);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Couldn't log to elastic : {ex.Message}", ex);
            }
        }
        public void Debug(string text)
        {
            try
            {
                Log.Logger.Debug(text);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Couldn't log to elastic : {ex.Message}", ex);
            }
        }
    }
}
