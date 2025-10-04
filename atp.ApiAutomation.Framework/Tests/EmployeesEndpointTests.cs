using atp.ApiAutomation.Framework.Services.Employees;
using AventStack.ExtentReports;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;

namespace atp.ApiAutomation.Framework.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class EmployeeEndpointTests : BaseTest
    {
        EmployeesService employeesService;
        Logger<EmployeeEndpointTests> logger;

        public EmployeeEndpointTests() : base()
        {
            employeesService = ServiceProvider.GetRequiredService<EmployeesService>();
            logger = (Logger<EmployeeEndpointTests>?)ServiceProvider.GetRequiredService<ILogger<EmployeeEndpointTests>>();
        }

        protected override void ConfigureFixtureServices(IServiceCollection services)
        {            
            services.AddTransient<EmployeesService>();
      
        }


        [Test]
        public async Task GetAllEmployees()
        {
            Console.WriteLine($"Test {TestContext.CurrentContext.Test.Name} running on thread {System.Threading.Thread.CurrentThread.ManagedThreadId} at {DateTime.Now:HH:mm:ss.fff}");
            

            // Use the injected field: _employeesService
            var response = await employeesService.GetAllEmployees();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();

            Test.Pass("Successfully retrieved all employees with OK status.");
        }

        // get all employee ids and use them in test cases
        [Test]
        [TestCaseSource(nameof(EmployeeIds))]
        public async Task GetEmployeeByIdTest(string id)
        {
            Console.WriteLine($"Test {TestContext.CurrentContext.Test.Name} running on thread {System.Threading.Thread.CurrentThread.ManagedThreadId} at {DateTime.Now:HH:mm:ss.fff}");

            // Use the injected field: _employeesService
            var response = await employeesService.GetEmployeeById(id);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Should().NotBeNull();

            Test.Pass($"Successfully retrieved employee {id} with OK status.");
        }

        [Test]
        [Repeat(10)]
        public async Task CreateEmployeeTest()
        {
            Console.WriteLine($"Test {TestContext.CurrentContext.Test.Name} running on thread {System.Threading.Thread.CurrentThread.ManagedThreadId} at {DateTime.Now:HH:mm:ss.fff}");
            // create a new employee model
            var newEmployee = Data.EmployeeGenerator.GenerateEmployee();

            // Use the injected field: _logger
            logger.LogInformation("Creating employee with id: {EmployeeId}, Name: {FirstName} {LastName}, DOB: {DOB}, Email: {Email}",
                newEmployee.id, newEmployee.firstName, newEmployee.lastName, newEmployee.dob, newEmployee.email);

            Test.Log(Status.Info, $"Attempting to create employee: {newEmployee.firstName} {newEmployee.lastName}");

            // call the create employee method
            // Use the injected field: _employeesService
            var response = await employeesService.CreateEmployee(newEmployee);

            // add assertions
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Should().NotBeNull();

            Test.Pass("Employee created successfully.");
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