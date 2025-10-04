using atp.ApiAutomation.Framework.Configurations;
using atp.ApiAutomation.Framework.Tests;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
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
        }

        [OneTimeTearDown]
        public void TearDown() 
        {
            
            Log.CloseAndFlush();

            Extent.Flush();
        }   

    }
}
