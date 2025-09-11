using atp.ApiAutomation.Framework.Services.Employees;
using Microsoft.Extensions.Logging;

namespace atp.ApiAutomation.Framework.Tests
{
    public class EmployeeEndpointTests : BaseTest
    {
        EmployeesService employeesService;

        [OneTimeSetUp]
        public override void GlobalSetup()
        {
            base.GlobalSetup();
            var serviceLogger = LoggerFactory.CreateLogger<EmployeesService>();

            employeesService = new EmployeesService(client, Settings, serviceLogger);

        }
        

        [Test]
        public async Task GetAllEmployees()
        {
            var response = await employeesService.GetAllEmployees();
            // add assertions
        }

        // get all employee ids and use them in test cases
        [TestCase("1")]
        [TestCase("3")]
        public async Task GetEmployeeById(string id)
        { 
            var response = await employeesService.GetEmployeeById(id);
        }
    }
}