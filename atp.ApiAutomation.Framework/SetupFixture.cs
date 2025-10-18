using atp.ApiAutomation.Framework.Configurations;
using atp.ApiAutomation.Framework.Services.Employees;
using atp.ApiAutomation.Framework.Services.Simulate;
using atp.ApiAutomation.Framework.Tests;
using atp.ApiAutomation.Framework.Utils;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RestSharp;
using Serilog;
using System;
using System.IO;

namespace atp.ApiAutomation.Framework
{
    [SetUpFixture]
    public class SetupFixture
    {
        public static IConfigurationRoot Configuration { get; private set; }
        public static ExtentReports Extent { get; set; }
        public static IServiceProvider ServiceProvider { get; set; }

        private static readonly string reportDirectory
            = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestReport");

        

        [OneTimeSetUp]
        public void SetupConfig() {

            //Configuration builder from multiple sources :
            //appSettings.json, environment variables, user secrets

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<BaseTest>(optional: true)
                .Build();


            
            // Logging configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/test-run.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();


            // Extent Report setup 

            if (Directory.Exists(reportDirectory) )
                {
                Directory.Delete(reportDirectory, true);
                }

            Directory.CreateDirectory(reportDirectory);

            Extent = new ExtentReports();

            // Attach the HTML reporter (Spark is a popular choice)
            var sparkReporter = new ExtentSparkReporter(Path.Combine(reportDirectory, "index.html"));
            Extent.AttachReporter(sparkReporter);

            Extent.AddSystemInfo(".NET Version", Environment.Version.ToString());
            Extent.AddSystemInfo("OS", Environment.OSVersion.VersionString);

            //Dependency Injection setup
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();


        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddSerilog(Log.Logger));

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton<TokenBucket>(new TokenBucket(capacity: 5, refillRate: 10));

            services.AddTransient<ApiSettings>(provider => provider.GetRequiredService<IConfiguration>().GetSection(nameof(ApiSettings)).Get<ApiSettings>());

            services.AddSingleton<RestClient>(provider =>
            {
                var settings = provider.GetRequiredService<ApiSettings>();

                
                var options = new RestClientOptions(settings.Host);
                return new RestClient(options);
            });

            services.AddTransient<atp.ApiAutomation.Framework.Services.BaseService>();

            // add other Api services here

            services.AddTransient<EmployeesService>();

            services.AddTransient<SimulateService>();
        }


        [OneTimeTearDown]
        public void TearDown() 
        {
            if (ServiceProvider is IDisposable disposableProvider)
            {
                disposableProvider.Dispose();
            }

            Log.CloseAndFlush();
            Extent.Flush();

        }   

    }
}
