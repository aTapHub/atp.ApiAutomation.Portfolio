# ApiAutomation.Portfolio

This repository contains a C#/.NET API test automation framework designed to demonstrate multiple approaches to parallelization and dependency injection.

---

## Parallelization & Branch Strategy
The project is split into three branches that explore different architectural trade-offs regarding concurrency and resource management.

### 1. Sequential Execution (`main`)
The baseline implementation. Test fixtures and individual tests run one after another on a single thread.

### 2. Fixture-Level Parallelization (`functional/add_parallelization`)
Fixtures run concurrently on separate worker threads (defaulting to 4).
* **Architecture:** Each fixture builds its own `ServiceProvider`.
* **Advantage:** High memory efficiency; services are created and disposed of only when the fixture is active.
* **Disadvantage:** Lack of shared state. Cross-fixture utilities (like a global `TokenBucket`) cannot share data because they reside in different DI containers.
* **Best Use Case:** High-speed execution where rate limiting is handled by the server or a proxy.

### 3. Shared Provider Parallelization (`functional/add_rate_limiter`)
Fixtures run concurrently but share a single `ServiceProvider` initialized in a `SetupFixture`.
* **Architecture:** A singleton `ServiceCollection` is shared across the entire run.
* **Advantage:** Enables global resource management. This allows the **TokenBucket** rate limiter to effectively throttle requests across all threads to avoid `HTTP 429` errors.
* **Disadvantage:** API service instances may persist in memory for the duration of the entire test suite execution.

---

## Project Structure

```text
atp.ApiAutomation.Portfolio/
├── Configurations/  # Mapping appsettings.json to C# POCOs
├── Data/            # Test data generation
├── Models/          # DTOs (Data Transfer Objects) for API Payloads
├── Services/        # API Client Logic (BaseService & Concrete Implementations)
├── Tests/           # NUnit Test Suites & BaseTest setup
└── Utils/           # Helpers (e.g., TokenBucket for Throttling)

```

-------
Here is the core schematic on how the Test and Api Services layers interact on sequential mode - main branch:
Inside TestClass constructor the api service needed for that TestFixture class is added into the service collection.
It is then retreived in the same TestClass but in the [OneTimeSetup] method. The .net service collection manges the ApiService lifetime.

<img width="691" height="761" alt="Api Framework drawio" src="https://github.com/user-attachments/assets/6e33412e-f464-4a1d-ab78-93bd372515c6" />



Below shows how the dependency injection mechanism works for the implementation that uses parallelization. In this case, all the services are added to the ServiceCollection inside the SetupFixture, that runs once per suite run.
TestBase, TestClasses, BaseService, ApiServices run as maby times as there are TestFixtures.

<img width="1376" height="1222" alt="image" src="https://github.com/user-attachments/assets/e458b9f7-1dc4-4a17-809d-10c7c6706f9c" />


----------------------------------------

