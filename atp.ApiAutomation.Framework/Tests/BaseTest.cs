using atp.ApiAutomation.Framework.Configurations;
using AventStack.ExtentReports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        public IServiceProvider ServiceProvider { get; set; }
        public ExtentTest Test { get; set; }


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

            
            Configuration.GetSection("ApiSettings").Bind(Settings);


            // Logging configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File("logs/test-run.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();


            // Setup a service provider - this will incorporate all needed services
            var services = new ServiceCollection();
            ConfigureServices(services);


            ServiceProvider = services.BuildServiceProvider();

        }

        [SetUp]
        public void Setup()
        {
            Test = SetupFixture.Extent.CreateTest(TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void AfterTest()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            var message = TestContext.CurrentContext.Result.Message;
            var stacktrace = TestContext.CurrentContext.Result.StackTrace;

            // Log the final status of the test.
            if (status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                // If the test failed, log the error message and stack trace.
                Test.Fail($"Test Failed: {message}");
                Test.Log(Status.Fail, stacktrace);
            }
            else if (status == NUnit.Framework.Interfaces.TestStatus.Skipped)
            {
                // If the test was skipped, log it.
                Test.Skip($"Test Skipped: {message}");
            }
            else
            {
                // If the test passed, log it.
                Test.Pass("Test Passed successfully.");
            }
        }



        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            //client.Dispose();
            Log.CloseAndFlush();


            // iterate through instances if ServiceProvider, see if they are disposable
            //and dispose them
            if (ServiceProvider is IDisposable disposableProvider)
            {
                disposableProvider.Dispose();
            }
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {

            // Register services for Depencency Injection container

            services.AddLogging(builder => builder.AddSerilog(Log.Logger));

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddSingleton<ApiSettings>(provider => provider.GetRequiredService<IConfiguration>().GetSection("ApiSettings").Get<ApiSettings>());


            services.AddSingleton(provider => {
                var settings = provider.GetRequiredService<ApiSettings>();
                return new RestClient(settings.Host);
            });

        }
    }
}
