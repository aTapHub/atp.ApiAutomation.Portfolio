using atp.ApiAutomation.Framework.Models;
using RestSharp;
using System.Text.Json;

namespace atp.ApiAutomation.Framework.Services.Employees
{
    public class EmployeesService : BaseService
    {
        public EmployeesService(RestClient client) : base(client)
        {
        }

        public async Task<RestResponse> GetAllEmployees(CancellationToken ct = default) 
        { 
            RestRequest request = new RestRequest(EmployeeEndpoints.Employees, Method.Get);

            return await client.ExecuteAsync(request, ct);
        }

        public async Task<RestResponse> GetEmployeeById(string id, CancellationToken ct = default)
        {
            var request = GetRequest(EmployeeEndpoints.EmployeeById, Method.Get);
            request.AddUrlSegment("id", id);

            return await client.ExecuteAsync(request,ct);
        }

        public async Task<RestResponse> CreateEmployee(CreateEmployeeModel employee, CancellationToken ct = default) 
        {
            var request = GetRequest(EmployeeEndpoints.Employees, Method.Post);
            request.AddJsonBody(JsonSerializer.Serialize(employee));

            return await client.ExecuteAsync(request, ct);
        }

        public async Task<RestResponse> UpdateEmployee(string id, CreateEmployeeModel employee, CancellationToken ct = default) 
        {
            var request = GetRequest(EmployeeEndpoints.EmployeeById, Method.Put);
            request.AddUrlSegment("id", id);
            request.AddJsonBody(JsonSerializer.Serialize(employee));
            return await client.ExecuteAsync(request, ct);
        }

        public async Task<RestResponse> UpdateEmployee(string id, string attribute, string value,  CancellationToken ct = default) 
        { 
            var employeeResponse = await GetEmployeeById(id, ct);


            // verify that the response is not null and is successful
            if (employeeResponse == null || !employeeResponse.IsSuccessful)
            {
                return employeeResponse ?? new RestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ErrorMessage = "GetEmployeeById returned a null response."
                };
            }

            var employeeResponseBody = JsonSerializer.Deserialize<CreateEmployeeModel>(employeeResponse.Content);
            employeeResponseBody?.GetType().GetProperty(attribute)?.SetValue(employeeResponseBody, value);

            return await UpdateEmployee(id, employeeResponseBody, ct);

        }

        public async Task<RestResponse> DeleteEmployee (string id, CancellationToken ct = default) 
        {
            var request = GetRequest(EmployeeEndpoints.EmployeeById, Method.Delete);
            request.AddUrlSegment("id", id);
            return await client.ExecuteAsync(request, ct);
        }



    }
}
