using RestSharp;

namespace atp.ApiAutomation.Framework.Services.Simulate
{
    public interface ISimulateService
    {
        Task<string> GetToken();
        Task<RestResponse> GetAllEmployees(CancellationToken ct = default);
    }
}
