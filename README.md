# atp.ApiAutomation.Portfolio
ApiAutomation Framework Portfolio

This is an API test automation framework written in C#/.NET that explores various parallelization techniques and demonstrates good coding practices.

The parallelization approaches are implemented in the branches described below:

main: No parallelization — test fixtures and tests run sequentially.

functional/add_parallelization: 
Parallelization at the fixture level — fixtures execute concurrently on separate worker threads (up to 4 threads), while tests inside each fixture run sequentially. 
Disadvantage: fixtures cannot share in-memory data (for example, the TokenBucket) because each fixture builds and uses its own dependency injection ServiceProvider. 
Advantage: services are created and disposed by fixtures only when needed, so unused services do not remain in memory. This architecture is appropriate when rate limiting is not a concern and any intermediary proxy does not return HTTP 429 (Too Many Requests).

functional/add_rate_limiter: 
Also fixture-level parallelization, but the ServiceCollection is configured in SetupFixture and a shared ServiceProvider is used across fixtures. 
Disadvantage: API service instances may remain in memory for the duration of the run. 
Advantage: fixtures (running on different threads) can share resources — primarily a token-bucket rate limiter — which helps avoid HTTP 429 (Too Many Requests) from intermediary proxies 


Libraries: RestSharp, NUnit, Newtonsoft.Json, FluentAssertions, Serilog, Microsoft.Extensions.Configuration, Bogus.
Project Layout:
Configurations — configuration model(s).
Models — DTOs used by services/tests.
Services — service layer and endpoint descriptions.
Tests — NUnit test classes.
Utils — helper utilities (e.g., TokenBucket).
Service Layer:
Endpoints: Endpoint definitions and URL constants live in endpoint classes such as EmployeeEndpoints.cs.
Service Implementation: Reusable HTTP wrappers and request helpers are in BaseService and concrete services like EmployeesService.cs.
Tests Layer:
Base Tests: Shared setup/teardown and common test helpers live in BaseTest.cs.
Endpoint Tests: Endpoint-focused tests such as EmployeesEndpointTests.cs and SimulateTests.cs.
Parallelization Strategy:
NUnit Parallel Support: The suite is designed to run tests in parallel using NUnit's parallelization features (test-level configuration or attributes) and CI orchestration via dotnet-test.yml.
Rate/Concurrency Control: A TokenBucket utility in TokenBucket.cs provides request throttling to prevent overload when many tests run concurrently.
Test Isolation: Tests target isolated data per test run (fixtures / generated data) to avoid inter-test interference and enable safe concurrency.
Good Coding Practices Followed:
Separation of Concerns: Clear split between Services (API interactions), Models (payloads), and Tests (assertions).
DRY & Reuse: BaseService and BaseTest centralize common logic and setup.
Config-driven: appsettings.json + ApiSettings.cs for environment/config separation.
Deterministic Test Data: Use of Bogus for predictable test data generation.
Expressive Assertions: FluentAssertions for readable, maintainable assertions.
Logging & Observability: Serilog integration for structured logs during test runs.
CI Integration: Dotnet test in CI with runsettings and workflow defined at dotnet-test.yml.
Small Focused Tests: Tests are endpoint-focused and fast, promoting reliable parallel runs.

-------

Here is the core schematic on how the Test and Api Services layers interact on sequential mode - main branch:


<img width="691" height="761" alt="Api Framework drawio" src="https://github.com/user-attachments/assets/6e33412e-f464-4a1d-ab78-93bd372515c6" />


----------------------------------------

