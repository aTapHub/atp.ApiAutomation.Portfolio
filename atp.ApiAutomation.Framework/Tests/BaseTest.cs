using AventStack.ExtentReports;
using NUnit.Framework;

[assembly: LevelOfParallelism(4)]
namespace atp.ApiAutomation.Framework.Tests
{
    public class BaseTest
    {
        protected ExtentTest Test { get; set; }

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
    }
}
