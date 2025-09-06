using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace atp.ApiAutomation.Framework.Tests
{
    public class BaseTest
    {
        public RestClient client;

        public BaseTest(string url) 
        { 
        RestClient client = new RestClient(url);
        }   

    }
}
