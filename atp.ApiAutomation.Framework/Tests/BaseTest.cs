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
        public void GlobalSetup()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<BaseTest>(optional: true)
                .Build();

            // Log the raw values from the configuration sources
            Console.WriteLine($"Raw Env Var API_USERNAME: {Configuration["API_USERNAME"]}");
            Console.WriteLine($"Raw Env Var API_PASSWORD: {Configuration["API_PASSWORD"]}");

            Settings = new ApiSettings();
            Configuration.GetSection("ApiSettings").Bind(Settings);

            // Log the bound values
            Console.WriteLine($"Bound Username: {Settings.API_USERNAME}");
            Console.WriteLine($"Bound Password: {Settings.API_PASSWORD}");

            client = new RestClient(Settings.Host);
        }

        [OneTimeTearDown]
        public void GlobalTearDown()
        {
            client.Dispose();
        }

    }
}
