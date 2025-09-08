using atp.ApiAutomation.Framework.Services.Simulate;

namespace atp.ApiAutomation.Framework.Tests
{
    public class SimulateBaseTests : BaseTest
    {
        public SimulateService simulateService;
        
        [OneTimeSetUp]
        public override void GlobalSetup()
        {
           base.GlobalSetup();
           simulateService = new SimulateService(Settings, client);
        }

    }
}
