using atp.ApiAutomation.Framework.Configurations;
using atp.ApiAutomation.Framework.Utils;
using Microsoft.Extensions.Logging;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Services
{
    public class BaseService
    {
        protected readonly RestClient client;
        protected readonly ILogger<BaseService> logger;
        protected readonly ApiSettings settings;
        private readonly RateLimiter rateLimiter;

        public BaseService(RestClient client, ApiSettings settings, ILogger<BaseService> logger, RateLimiter rateLimiter)
        {
            this.client = client;
            this.logger = logger;
            this.settings = settings;
            this.rateLimiter = rateLimiter;
        }


        public static RestRequest GetRequest(string endpoint, Method method )
        {
            return new RestRequest(endpoint, method);
        }

        protected async Task<RestResponse> ExecuteAsync(RestRequest request, CancellationToken ct = default)
        {
            await rateLimiter.WaitAsync(ct);
            return await client.ExecuteAsync(request, ct);
        }
    }
}
