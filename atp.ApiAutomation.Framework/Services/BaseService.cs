using atp.ApiAutomation.Framework.Configurations;
using atp.ApiAutomation.Framework.Utils;
using Microsoft.Extensions.Logging;
using Polly;
using RestSharp;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Services
{
    public class BaseService
    {
        protected readonly RestClient _client;
        protected readonly ApiSettings _settings;
        protected readonly ILogger<BaseService> _logger;
        private readonly TokenBucket _rateLimiter;
        private readonly AsyncPolicy _policy;

        public BaseService(RestClient client, ApiSettings settings, ILogger<BaseService> logger, TokenBucket rateLimiter, AsyncPolicy policy)
        {
            _client = client;
            _settings = settings;
            _logger = logger;
            _rateLimiter = rateLimiter;
            _policy = policy;

            _logger.LogInformation($"BaseService initialized. Base URL set to: {_settings.Host}");
        }

        protected static RestRequest GetRequest(string resource, Method method)
        {
            return new RestRequest(resource, method);
        }

        protected Task<RestResponse> ExecuteWithRateLimitAsync(RestRequest request)
        {
            return _policy.ExecuteAsync(async () =>
            {
                await _rateLimiter.AcquireToken();

                _logger.LogDebug($"Token acquired. Executing request: {request.Resource}");

                var response = await _client.ExecuteAsync(request);

                _logger.LogDebug($"Request completed for {request.Resource}. Status: {response?.StatusCode}");

                if (response == null)
                    throw new TransientHttpException("Null response from API");

                var code = (int)response.StatusCode;
                if (code >= 500 || code == 429)
                    throw new TransientHttpException($"Transient HTTP status code {code}");

                return response;
            });
        }
    }
}