using atp.ApiAutomation.Framework.Configurations;
using atp.ApiAutomation.Framework.Utils;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Services
{
    public class BaseService
    {
        // Thread-safe dependencies injected via the constructor
        protected readonly RestClient _client;
        protected readonly ApiSettings _settings;
        protected readonly ILogger<BaseService> _logger;

        // CRITICAL: This is the Singleton TokenBucket instance
        private readonly TokenBucket _rateLimiter;

        /// <summary>
        /// Constructor for BaseService. Dependencies are provided by the global ServiceProvider.
        /// </summary>
        /// <param name="client">The Singleton RestClient for efficient connections.</param>
        /// <param name="settings">The Transient ApiSettings configuration.</param>
        /// <param name="logger">The Logger instance.</param>
        /// <param name="rateLimiter">The Singleton TokenBucket to enforce rate limits.</param>
        public BaseService(RestClient client, ApiSettings settings, ILogger<BaseService> logger, TokenBucket rateLimiter)
        {
            _client = client;
            _settings = settings;
            _logger = logger;
            _rateLimiter = rateLimiter;
    
           _logger.LogInformation($"BaseService initialized. Base URL set to: {_settings.Host}");
            
        }

        /// <summary>
        /// Creates a new RestRequest object with the specified resource and method.
        /// </summary>
        /// <param name="resource">The resource path (e.g., "/employees").</param>
        /// <param name="method">The HTTP method.</param>
        /// <returns>A new RestRequest instance.</returns>
        protected static RestRequest GetRequest(string resource, Method method)
        {
            return new RestRequest(resource, method);
        }

        /// <summary>
        /// Executes an API request after acquiring a token from the shared TokenBucket.
        /// This ensures all parallel test threads adhere to the global rate limit.
        /// </summary>
        /// <param name="request">The RestRequest to execute.</param>
        /// <returns>The API response.</returns>
        protected async Task<RestResponse> ExecuteWithRateLimitAsync(RestRequest request)
        {
            // CRITICAL STEP: Wait for a token. All parallel threads hit this same method 
            // and use the same singleton _rateLimiter instance.
            await _rateLimiter.AcquireToken();

            _logger.LogDebug($"Token acquired. Executing request: {request.Resource}");

            // Execute the request using the Singleton RestClient
            var response = await _client.ExecuteAsync(request);

            _logger.LogDebug($"Request completed for {request.Resource}. Status: {response.StatusCode}");

            return response;
        }
    }
}
