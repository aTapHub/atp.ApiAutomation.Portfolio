using atp.ApiAutomation.Framework.Configurations;
using atp.ApiAutomation.Framework.Models;
using Microsoft.Extensions.Logging;
using RestSharp;
using System.Text.Json;

namespace atp.ApiAutomation.Framework.Services.Employees
{
    public class EmployeesService(RestClient client, ApiSettings settings,
        ILogger<EmployeesService> logger) : BaseService(client, settings, logger)
    {
        public async Task<RestResponse> GetAllEmployees(CancellationToken ct = default) 
        { 
            RestRequest request = new RestRequest(EmployeeEndpoints.Employees, Method.Get);

            var response = await client.ExecuteAsync(request, ct);

            if (response == null)
            {
                throw new InvalidOperationException($"Failed to get employees, response is null. Status Code: {response?.StatusCode}, Error Message: {response?.ErrorMessage}");
            }

            return response;
        }

        public async Task<RestResponse> GetEmployeeById(string id, CancellationToken ct = default)
        {
            var request = GetRequest(EmployeeEndpoints.EmployeeById, Method.Get);
            request.AddUrlSegment("id", id);

            var response = await client.ExecuteAsync(request, ct);

            if (response == null)
            {
                throw new InvalidOperationException($"Failed to get employee by id, response is null. Status Code: {response?.StatusCode}, Error Message: {response?.ErrorMessage}");
            }

            return response;
        }

        public async Task<RestResponse> CreateEmployee(CreateEmployeeModel employee, CancellationToken ct = default) 
        {
            var request = GetRequest(EmployeeEndpoints.Employees, Method.Post);
            request.AddJsonBody(JsonSerializer.Serialize(employee));

            var response = await client.ExecuteAsync(request, ct);

            if (response == null)
            {
                throw new InvalidOperationException($"Failed to create employee, response is null. Status Code: {response?.StatusCode}, Error Message: {response?.ErrorMessage}");
            }

            return response;
        }

        public async Task<RestResponse> UpdateEmployee(string id, CreateEmployeeModel employee, CancellationToken ct = default) 
        {
            var request = GetRequest(EmployeeEndpoints.EmployeeById, Method.Put);
            request.AddUrlSegment("id", id);
            request.AddJsonBody(JsonSerializer.Serialize(employee));

            var response = await client.ExecuteAsync(request, ct);

            if (response == null)
            {
                throw new InvalidOperationException($"Failed to update employee, response is null. Status Code: {response?.StatusCode}, Error Message: {response?.ErrorMessage}");
            }

            return response;

            
        }

        public async Task<RestResponse> UpdateEmployee(string id, string attribute, string value,  CancellationToken ct = default) 
        { 
            var employeeResponse = await GetEmployeeById(id, ct);


            // verify that the response is not null and is successful
            if (employeeResponse.Content == null)
            {
                throw new InvalidOperationException("Response content body is null");
            }

            
            var employeeResponseBody = JsonSerializer.Deserialize<CreateEmployeeModel>(employeeResponse.Content);
            employeeResponseBody?.GetType().GetProperty(attribute)?.SetValue(employeeResponseBody, value);

            if (employeeResponseBody == null)
            {
                throw new InvalidOperationException("Failed to deserialize employee response body or body is null");
            }

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
