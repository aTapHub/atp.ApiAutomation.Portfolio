using atp.ApiAutomation.Framework.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Tests
{
    public class SimulateTests : SimulateBaseTests
    {

        [Test]
        public void GetAllEmployeesWithAuth() 
        {
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
