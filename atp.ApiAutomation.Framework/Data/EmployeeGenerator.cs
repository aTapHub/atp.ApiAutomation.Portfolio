using atp.ApiAutomation.Framework.Models;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Data
{
    public class EmployeeGenerator
    {

        public static CreateEmployeeModel GenerateEmployee()
        {

            var employeeFaker = new Faker<CreateEmployeeModel>()
                .RuleFor(e => e.id, f => f.Random.Guid().ToString())
                .RuleFor(e => e.firstName, f => f.Name.FirstName())
                .RuleFor(e => e.lastName, f => f.Name.LastName())
                .RuleFor(e => e.dob, f => f.Date.Past(30, DateTime.Now.AddYears(-18)).ToShortDateString())
                .RuleFor(e => e.email, (f, e) => f.Internet.Email(e.firstName, e.lastName));

            return employeeFaker.Generate();

        }

    }
}
