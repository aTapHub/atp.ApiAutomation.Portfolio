using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Models
{
    public record CreateEmployeeModel
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dob { get; set; }
        public string email { get; set; }

        public CreateEmployeeModel(string id, string firstName, string lastName, string dob, string email)
        {
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.dob = dob;
            this.email = email;
        }

    }
}
