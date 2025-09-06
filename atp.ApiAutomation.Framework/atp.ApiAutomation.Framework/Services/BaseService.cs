using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Services
{
    public class BaseService
    {

        public RestRequest GetRequest(string endpoint, Method method )
        { 
            return new RestRequest(endpoint, method);
        }
    }
}
