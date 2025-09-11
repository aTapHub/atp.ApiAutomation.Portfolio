using atp.ApiAutomation.Framework.Configurations;
using Microsoft.Extensions.Logging;
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
        public RestClient client;
        protected readonly ILogger<BaseService> logger;
        protected readonly ApiSettings settings;

        public BaseService(RestClient client, ApiSettings settings, ILogger<BaseService> logger)
        {
            this.client = client;
            this.logger = logger;
            this.settings = settings;
        }


        public static RestRequest GetRequest(string endpoint, Method method )
        { 
            return new RestRequest(endpoint, method);
        }
    }
}
