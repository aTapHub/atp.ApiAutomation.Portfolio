using atp.ApiAutomation.Framework.Configurations;
using atp.ApiAutomation.Framework.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace atp.ApiAutomation.Framework.Tests
{
    public class SimulateTests : SimulateBaseTests
    {
        private ApiSettings settings;
        public override void GlobalSetup()
        {
            base.GlobalSetup();
            settings = ServiceProvider.GetService<ApiSettings>();
        }

        [OneTimeSetUp]

        [Test]
        public async Task GetAllEmployeesWithAuth() 
        {


            Console.WriteLine($"Username from settings: {settings.API_USERNAME}");
            Console.WriteLine($"Password from settings: {settings.API_PASSWORD}");
            Console.WriteLine($"Host from settings: {settings.Host}");
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();

            Console.WriteLine("--- System Environment Variables ---");

            // Iterate through each variable and print its key and value
            foreach (DictionaryEntry variable in environmentVariables)
            {
                Console.WriteLine($"{variable.Key} = {variable.Value}");
            }

            Console.WriteLine("------------------------------------");

            var response = await simulateService.GetAllEmployees();

            // tests
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Content.Should().NotBeNullOrEmpty();

            var employees = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CreateEmployeeModel>>(response.Content);

            employees.Should().NotBeNull();
        }


    }
}
