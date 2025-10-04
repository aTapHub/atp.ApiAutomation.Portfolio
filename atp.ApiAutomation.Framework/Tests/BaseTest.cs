using atp.ApiAutomation.Framework.Configurations;
using AventStack.ExtentReports;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using Serilog;
using Serilog.Events;
using NUnit.Framework;

//[assembly: Parallelizable(ParallelScope.All)]
[assembly: LevelOfParallelism(4)]
namespace atp.ApiAutomation.Framework.Tests
{
    public class BaseTest : IDisposable
    {
        public IServiceProvider ServiceProvider { get; set; }

        protected ExtentTest Test { get; set; }

        public BaseTest()
        {
            // Setup a service provider - this will incorporate all needed services
            var services = new ServiceCollection();
            ConfigureServices(services);
            ConfigureFixtureServices(services);
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


        protected virtual void ConfigureServices(IServiceCollection services)
        {

            // Register services for Depencency Injection container

            services.AddLogging(builder => builder.AddSerilog(Log.Logger));

            services.AddSingleton<IConfiguration>(SetupFixture.Configuration);

            services.AddScoped<ApiSettings>(provider => provider.GetRequiredService<IConfiguration>().GetSection("ApiSettings").Get<ApiSettings>());

            services.AddScoped(provider =>
            {
                var settings = provider.GetRequiredService<ApiSettings>();
                return new RestClient(settings.Host);
            });

        }

        protected virtual void ConfigureFixtureServices(IServiceCollection services)
        {
            // Register per-fixture services here in derived classes
        }


        [OneTimeTearDown] 
        public void FixtureTearDown()
        {
            // Calling Dispose() triggers the cleanup of the ServiceProvider
            // which in turn cleans up all scoped IDisposable services (like RestClient).
            Dispose();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of the per-fixture service provider
                if (ServiceProvider is IDisposable disposableProvider)
                {
                    disposableProvider.Dispose();
                }
            }
        }
    }
}
