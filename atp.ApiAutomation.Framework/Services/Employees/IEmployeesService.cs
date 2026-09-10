using atp.ApiAutomation.Framework.Models;
using RestSharp;

namespace atp.ApiAutomation.Framework.Services.Employees
{
    public interface IEmployeesService
    {
        Task<RestResponse> GetAllEmployees(CancellationToken ct = default);
        Task<RestResponse> GetEmployeeById(string id, CancellationToken ct = default);
        Task<RestResponse> CreateEmployee(CreateEmployeeModel employee, CancellationToken ct = default);
        Task<RestResponse> UpdateEmployee(string id, CreateEmployeeModel employee, CancellationToken ct = default);
        Task<RestResponse> UpdateEmployee(string id, string attribute, string value, CancellationToken ct = default);
        Task<RestResponse> DeleteEmployee(string id, CancellationToken ct = default);
    }
}
