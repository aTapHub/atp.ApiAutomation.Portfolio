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
        public static IServiceProvider ServiceProvider { get; private set; }
        public static ExtentReports Extent { get; set; }

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


            // Dependency injection container - built once and shared across all
            // fixtures. RestClient/ApiSettings/logging are registered as singletons
            // since they are safe to share across fixtures running in parallel;
            // fixture services stay transient so each fixture gets its own instance.
            var services = new ServiceCollection();

            services.AddLogging(builder => builder.AddSerilog(Log.Logger));

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton(provider =>
                provider.GetRequiredService<IConfiguration>().GetSection("ApiSettings").Get<ApiSettings>());

            services.AddSingleton(provider =>
                new RestClient(provider.GetRequiredService<ApiSettings>().Host));

            // Shared rate limiter gate - one bucket of 5 tokens, refilled every second,
            // so all fixtures running in parallel throttle against the same limit.
            services.AddSingleton(new RateLimiter(capacity: 5, refillInterval: TimeSpan.FromSeconds(1)));

            services.AddTransient<IEmployeesService, EmployeesService>();
            services.AddTransient<ISimulateService, SimulateService>();

            ServiceProvider = services.BuildServiceProvider();


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
