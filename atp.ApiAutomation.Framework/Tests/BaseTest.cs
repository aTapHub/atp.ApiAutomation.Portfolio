using atp.ApiAutomation.Framework.Configurations;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace atp.ApiAutomation.Framework.Tests
{
    public class BaseTest
    {
        public static IConfigurationRoot Configuration { get; private set; }
        public static ApiSettings Settings { get; private set; }
        public RestClient client;


        [OneTimeSetUp]
        public virtual void GlobalSetup()
        {
            Console.WriteLine("Starting BaseTest");

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables(prefix: "API_")
                .AddUserSecrets<BaseTest>(optional: true)
                .Build();

            Settings = new ApiSettings();
            Configuration.GetSection("ApiSettings").Bind(Settings);

            client = new RestClient(Settings.Host);

            Console.WriteLine("Ending BaseTest");
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            client.Dispose();
        }

    }
}
