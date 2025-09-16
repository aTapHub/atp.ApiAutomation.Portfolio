using atp.ApiAutomation.Framework.Configurations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace atp.ApiAutomation.Framework.Services.Simulate
{
    public class SimulateService(ApiSettings apiSettings, RestClient client,
                        ILogger<SimulateService> logger) : BaseService(client, apiSettings, logger)
    {
        public async Task<string> GetToken() 
        { 
        
            var request = GetRequest(SimulateEndpoints.TokenEndpoint, Method.Post);
            request.AddBody(new { username = settings.API_USERNAME, password = settings.API_PASSWORD });
            
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
