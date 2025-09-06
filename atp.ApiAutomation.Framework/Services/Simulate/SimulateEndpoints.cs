using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace atp.ApiAutomation.Framework.Services.Simulate
{
    public static class SimulateEndpoints
    {

        public const string  TokenEndpoint = "/api/v1/simulate/token";
        // post

        public const string GetAllEmployeesEndpoint = "/api/v1/simulate/get/employees";
        // get

        public const string SimulateServerErrorEndpoint = "/api/v1/simulate/server/error";
        /// get

    }
}
