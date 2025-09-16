using atp.ApiAutomation.Framework.Services.Employees;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace atp.ApiAutomation.Framework.Tests
{
    public class EmployeeEndpointTests : BaseTest
    {
        EmployeesService employeesService;

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddTransient<EmployeesService>();
        }

        [OneTimeSetUp]
        public override void GlobalSetup()
        {
            base.GlobalSetup();

            employeesService = ServiceProvider.GetService<EmployeesService>();

        }
        

        [Test]
        public async Task GetAllEmployees()
        {
            var response = await employeesService.GetAllEmployees();
            // add assertions
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        // get all employee ids and use them in test cases
        [TestCase("1")]
        [TestCase("3")]
        public async Task GetEmployeeByIdTest(string id)
        { 
            var response = await employeesService.GetEmployeeById(id);
        }

        [Test]
        [Repeat(10)]
        public async Task CreateEmployeeTest()
        {
            // create a new employee model
            var newEmployee = Data.EmployeeGenerator.GenerateEmployee();
            // call the create employee method
            var response = await employeesService.CreateEmployee(newEmployee);
            // add assertions
            Assert.IsNotNull(response);
        }
    }
}