using atp.ApiAutomation.Framework.Services.Simulate;
using Microsoft.Extensions.Logging;

namespace atp.ApiAutomation.Framework.Tests
{
    public class SimulateBaseTests : BaseTest
    {
        public SimulateService simulateService;
        
        [OneTimeSetUp]
        public override void GlobalSetup()
        {
           base.GlobalSetup();

            var serviceLogger = LoggerFactory.CreateLogger<SimulateService>();
            simulateService = new SimulateService(Settings, client, serviceLogger);
           
        }

    }
}
