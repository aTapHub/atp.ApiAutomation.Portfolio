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
    public class BaseTest
    {
        public IServiceProvider ServiceProvider { get; set; }

        protected ExtentTest Test { get; set; }

        public BaseTest()
        {
          
        }

        protected T GetService<T>() where T : notnull
        {
            // This is the key line: it uses the static provider built once for the entire assembly.
            return SetupFixture.ServiceProvider.GetRequiredService<T>();
        }


        [SetUp]
        public void Setup()
        {
            // Standard NUnit Setup for Extent Reporting
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


    }
}
