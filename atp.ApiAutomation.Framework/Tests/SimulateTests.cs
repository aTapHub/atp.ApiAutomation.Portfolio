using atp.ApiAutomation.Framework.Configurations;
using atp.ApiAutomation.Framework.Models;
using atp.ApiAutomation.Framework.Services.Simulate;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Fixtures)]
    public class SimulateTests : BaseTest
    {       
        private ApiSettings _settings;
        private SimulateService _simulateService;
        private Logger<SimulateTests> _logger;
 
        public SimulateTests() 
        
        {
        }

        [Test]
        public async Task GetAllEmployeesWithAuth()
        {
            Console.WriteLine($"Test {TestContext.CurrentContext.Test.Name} running on thread {System.Threading.Thread.CurrentThread.ManagedThreadId} at {DateTime.Now:HH:mm:ss.fff}");

            // _simulateService is now available from the base class's [OneTimeSetUp]
            var response = await _simulateService.GetAllEmployees();

            // tests
            response.IsSuccessful.Should().BeTrue();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Should().NotBeNullOrEmpty();

            // Note: Assuming Newtonsoft.Json is referenced in the project
            var employees = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CreateEmployeeModel>>(response.Content);

            employees.Should().NotBeNull();

        }
    }
}
