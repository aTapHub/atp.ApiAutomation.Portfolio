using atp.ApiAutomation.Framework.Services.Simulate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atp.ApiAutomation.Framework.Tests
{
    public class SimulateBaseTests : BaseTest
    {
        public SimulateService simulateService;
        
        [OneTimeSetUp]
        public void GlobalSetup()
        {
           base.GlobalSetup();
           simulateService = new SimulateService(Settings, client);
        }

    }
}
