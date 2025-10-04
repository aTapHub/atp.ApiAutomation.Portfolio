using atp.ApiAutomation.Framework.Services.Simulate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework; // Needed for [OneTimeSetUp]
using System;
using System.Net.Security; // Needed for IServiceProvider

namespace atp.ApiAutomation.Framework.Tests
{
    
    public class SimulateBaseTests : BaseTest
    {
        // Fields are now protected (for derived classes) and non-readonly 
        protected SimulateService _simulateService;
        protected ILogger<SimulateBaseTests> _logger;

        

        [OneTimeSetUp]
        public void ResolveBaseServices()
        {
            // Resolve the base services once per fixture, after the ServiceProvider is built.
            _simulateService = ServiceProvider.GetRequiredService<SimulateService>();
            _logger = ServiceProvider.GetRequiredService<ILogger<SimulateBaseTests>>();
        }

        protected override void ConfigureFixtureServices(IServiceCollection services)
        {
            
            // Register the SimulateService
            services.AddTransient<SimulateService>();
        }
    }
}
