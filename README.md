ApiAutomation.Portfolio
This repository contains a C#/.NET API test automation framework designed to demonstrate multiple approaches to parallelization and dependency injection.

Parallelization & Branch Strategy
The project is split into three branches to showcase different architectural trade-offs regarding concurrency and resource management.

1. Sequential Execution (main)
The baseline implementation. Test fixtures and individual tests run one after another on a single thread.

2. Fixture-Level Parallelization (functional/add_parallelization)
Fixtures run concurrently on separate worker threads (defaulting to 4).

Architecture: Each fixture builds its own ServiceProvider.

Advantage: High memory efficiency; services are created and disposed of only when the fixture is active.

Disadvantage: Lack of shared state. Cross-fixture utilities (like a global TokenBucket) cannot share data because they reside in different DI containers.

Best Use Case: High-speed execution where rate limiting is handled by the server or a proxy.

3. Shared Provider Parallelization (functional/add_rate_limiter)
Fixtures run concurrently but share a single ServiceProvider initialized in a SetupFixture.

Architecture: A singleton ServiceCollection is shared across the entire run.

Advantage: Enables global resource management. This allows the TokenBucket rate limiter to effectively throttle requests across all threads to avoid HTTP 429 errors.

Disadvantage: API service instances may persist in memory for the duration of the entire test suite execution.

Technical Stack
Core: .NET with NUnit as the test runner.

Rest Client: RestSharp for API interactions.

Data & Validation: FluentAssertions for readable checks and Bogus for deterministic data generation.

Infrastructure: Serilog (Logging) and Microsoft.Extensions.Configuration (Environment Management).

Project Architecture
Service Layer
Endpoints: EmployeeEndpoints.cs contains URL constants and route definitions.

Implementation: BaseService.cs provides a reusable wrapper for HTTP requests, extended by concrete classes like EmployeesService.cs.

Test Layer
BaseTest.cs: Handles the core setup, teardown, and common helper methods.

Endpoint Tests: Logical groupings of tests (e.g., EmployeesEndpointTests.cs) focused on specific API domains.

Utilities
TokenBucket.cs: A custom utility providing request throttling logic to manage concurrency limits.

Implementation Highlights
Config-Driven: Uses appsettings.json and ApiSettings.cs for environment-specific configurations.

Test Isolation: Ensures each test uses unique generated data to prevent race conditions during parallel runs.

-------

Here is the core schematic on how the Test and Api Services layers interact on sequential mode - main branch:


<img width="691" height="761" alt="Api Framework drawio" src="https://github.com/user-attachments/assets/6e33412e-f464-4a1d-ab78-93bd372515c6" />


----------------------------------------

