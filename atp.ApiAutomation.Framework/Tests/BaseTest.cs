using atp.ApiAutomation.Framework.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Serilog;
using Serilog.Events;

namespace atp.ApiAutomation.Framework.Tests
{
    public class BaseTest
    {
        public static IConfigurationRoot Configuration { get; private set; }
        public static ILoggerFactory LoggerFactory { get; private set; }
        public static ApiSettings Settings { get; private set; }
        public RestClient client;


        [OneTimeSetUp]
        public virtual void GlobalSetup()
        {

            //Configuration builder from multiple sources :
            //appSettings.json, environment variables, user secrets

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<BaseTest>(optional: true)
                .Build();

            Settings = new ApiSettings();
            Configuration.GetSection("ApiSettings").Bind(Settings);


            // Logging configuration

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/test-run.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            LoggerFactory = new LoggerFactory().AddSerilog(Log.Logger);




            client = new RestClient(Settings.Host);

            
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            client.Dispose();
            Log.CloseAndFlush();
        }

    }
}
