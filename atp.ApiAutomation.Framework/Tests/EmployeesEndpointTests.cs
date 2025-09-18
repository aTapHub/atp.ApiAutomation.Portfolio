using atp.ApiAutomation.Framework.Services.Employees;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace atp.ApiAutomation.Framework.Tests
{
    public class EmployeeEndpointTests : BaseTest
    {
        EmployeesService employeesService;
        Logger<EmployeeEndpointTests> logger;

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
            logger = (Logger<EmployeeEndpointTests>)ServiceProvider.GetService<ILogger<EmployeeEndpointTests>>();


        }


        [Test]
        public async Task GetAllEmployees()
        {
            var response = await employeesService.GetAllEmployees();
            // add assertions
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Should().NotBeNull();
        }

        // get all employee ids and use them in test cases
        [Test]
        [TestCaseSource(nameof(EmployeeIds))]
        public async Task GetEmployeeByIdTest(string id)
        { 
            var response = await employeesService.GetEmployeeById(id);

            // add assertions
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Should().NotBeNull();
        }

        [Test]
        [Repeat(10)]
        public async Task CreateEmployeeTest()
        {
            // create a new employee model
            var newEmployee = Data.EmployeeGenerator.GenerateEmployee();

            logger.LogInformation("Creating employee with id: {EmployeeId}, Name: {FirstName} {LastName}, DOB: {DOB}, Email: {Email}",
                newEmployee.id, newEmployee.firstName, newEmployee.lastName, newEmployee.dob, newEmployee.email);
            
            // call the create employee method
            var response = await employeesService.CreateEmployee(newEmployee);
            
            // add assertions
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Should().NotBeNull();
        }




        public static IEnumerable<TestCaseData> EmployeeIds
        {
            get
            {
              
                yield return new TestCaseData("1");  
                yield return new TestCaseData("2");  
                yield return new TestCaseData("3");  
                                                     
            }
        }
    }
}