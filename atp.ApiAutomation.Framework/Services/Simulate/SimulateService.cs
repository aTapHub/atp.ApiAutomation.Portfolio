using atp.ApiAutomation.Framework.Configurations;
using atp.ApiAutomation.Framework.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace atp.ApiAutomation.Framework.Services.Simulate
{
    public class SimulateService(ApiSettings apiSettings, RestClient client,
                        ILogger<SimulateService> logger, TokenBucket rateLimiter) : 
        BaseService(client, apiSettings, logger, rateLimiter)
    {
        public async Task<string> GetToken() 
        { 
        
            var request = GetRequest(SimulateEndpoints.TokenEndpoint, Method.Post);
            request.AddBody(new { username = apiSettings.API_USERNAME, password = apiSettings.API_PASSWORD });
            
            var result = await client.ExecuteAsync(request);

            if (string.IsNullOrEmpty(result?.Content))
            { 
             throw new InvalidOperationException("Failed to retrieve token, response content is null.");
            }
          
            var token = JObject.Parse(result.Content!)["token"]?.ToString();
            return token ?? throw new InvalidOperationException("Token not found in response content.");
      
        }


        public async Task<RestResponse> GetAllEmployees(CancellationToken ct = default) 
        { 
            var token = await GetToken();
            var request = GetRequest(SimulateEndpoints.GetAllEmployeesEndpoint, Method.Get);
            request.AddHeader("Authorization", $"Bearer {token}");
            return await client.ExecuteAsync(request, ct);
        }
    }
}
