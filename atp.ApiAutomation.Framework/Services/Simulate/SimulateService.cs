using atp.ApiAutomation.Framework.Configurations;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace atp.ApiAutomation.Framework.Services.Simulate
{
    public class SimulateService : BaseService
    {
        private readonly ApiSettings _settings;
        public SimulateService(ApiSettings apiSettings, RestClient client) : base(client)
        {
            _settings = apiSettings;
        }

        public async Task<string> GetToken() 
        { 
        
            var request = GetRequest(SimulateEndpoints.TokenEndpoint, Method.Post);
            request.AddBody(new { username = _settings.API_USERNAME, password = _settings.API_PASSWORD });
            
            var result = await client.ExecuteAsync(request);
            
            return JObject.Parse(result.Content)["token"]?.ToString();


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
