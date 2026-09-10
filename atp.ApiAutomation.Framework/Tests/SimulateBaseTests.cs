using atp.ApiAutomation.Framework.Services.Simulate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

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
            // Resolve the base services once per fixture, from the shared provider built in SetupFixture.
            _simulateService = SetupFixture.ServiceProvider.GetRequiredService<SimulateService>();
            _logger = SetupFixture.ServiceProvider.GetRequiredService<ILogger<SimulateBaseTests>>();
        }
    }
}
