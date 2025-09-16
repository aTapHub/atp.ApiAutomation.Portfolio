using atp.ApiAutomation.Framework.Services.Simulate;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace atp.ApiAutomation.Framework.Tests
{
    public class SimulateBaseTests : BaseTest
    {
        public SimulateService simulateService;

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            services.AddTransient<SimulateService>();
        }

        [OneTimeSetUp]
        public override void GlobalSetup()
        {
           base.GlobalSetup();
           simulateService = ServiceProvider.GetService<SimulateService>();

        }

    }
}
