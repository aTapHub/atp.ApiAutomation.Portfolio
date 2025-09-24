using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework
{
    [SetUpFixture]
    public class SetupFixture
    {
        public static ExtentReports Extent { get; set; }

        private static readonly string reportDirectory
            = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestReport");

        [OneTimeSetUp]
        public void Setup() { 
        
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
            Extent.Flush();
        }   

    }
}
