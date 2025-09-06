using atp.ApiAutomation.Framework.Services.Employees;

namespace atp.ApiAutomation.Framework.Tests
{
    public class EmployeeEndpointTests : BaseTest
    {
        EmployeesService employeesService;

        [OneTimeSetUp]
        public void GlobalSetup()
        {
            base.GlobalSetup();

            employeesService = new EmployeesService(client);

        }
        

        [Test]
        public void GetAllEmployees()
        {
            var response = employeesService.GetAllEmployees().Result;
            // add assertions
        }

        // get all employee ids and use them in test cases
        [TestCase("1")]
        [TestCase("3")]
        public void GetEmployeeById(string id)
        { 
            var response = employeesService.GetEmployeeById(id).Result;
        }
    }
}