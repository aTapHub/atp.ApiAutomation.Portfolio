using atp.ApiAutomation.Framework.Services.Employees;

namespace atp.ApiAutomation.Framework.Tests
{
    public class EmployeeEndpointTests : BaseTest
    {
        EmployeesService employeesService;
        public EmployeeEndpointTests(string url) : base(url)
        {
            employeesService = new EmployeesService(client);
        }

        [SetUp]
        public void Setup()
        {
            // generate tests data ?
        }

        [Test]
        public void GetAllEmployees()
        {
            var response = employeesService.GetAllEmployees().Result;
            // add assertions
        }

        // get all employee ids and use them in test cases
        [TestCase("123")]
        [TestCase("456")]
        public void GetEmployeeById(string id)
        { 
            var response = employeesService.GetEmployeeById(id).Result;
        }
    }
}