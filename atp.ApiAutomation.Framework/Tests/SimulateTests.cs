using atp.ApiAutomation.Framework.Models;
using FluentAssertions;

namespace atp.ApiAutomation.Framework.Tests
{
    public class SimulateTests : SimulateBaseTests
    {

        [Test]
        public void GetAllEmployeesWithAuth() 
        {

            Console.WriteLine($"Username from settings: {Settings.Username}");
            Console.WriteLine($"Password from settings: {Settings.Password}");

            var response = simulateService.GetAllEmployees().Result;

            // tests
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Content.Should().NotBeNullOrEmpty();

            var employees = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CreateEmployeeModel>>(response.Content);

            employees.Should().NotBeNull();
        }


    }
}
