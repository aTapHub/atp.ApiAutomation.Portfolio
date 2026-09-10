# ApiAutomation.Portfolio

This repository contains a C#/.NET API test automation framework designed to demonstrate multiple approaches to parallelization and dependency injection.

---

## Parallelization & Branch Strategy
The project is split into three branches that explore different architectural trade-offs regarding concurrency and resource management.

### 1. Sequential Execution (`main`)
The baseline implementation. Test fixtures and individual tests run one after another on a single thread.

### 2. Fixture-Level Parallelization (`functional/add_paralelization`)
Fixtures run concurrently on separate worker threads (defaulting to 4); `[Parallelizable(ParallelScope.Fixtures)]` keeps the tests *inside* a given fixture sequential.

This branch originally gave each fixture its own `ServiceProvider`, built inside the fixture's own constructor. That design turned out to be incompatible with real parallel execution: NUnit reuses a single instance of a fixture class across all of its test methods by default, so anything using `ParallelScope.All` for method-level parallelism raced on that instance's shared fields (the Extent `Test` handle, resolved services) — corrupting the HTML report and occasionally throwing. Restricting parallelism to fixture-level removed the method-level race, but exposed the next problem: per-fixture containers can't share anything across fixtures, including the singletons (like `RestClient`) that are supposed to be reused rather than recreated per instance.

* **Architecture (current):** A single `ServiceProvider` is built once in `SetupFixture`, before any fixture runs. `RestClient`/`ApiSettings`/logging are registered as singletons — safe to share, since concurrently running fixtures only read from them (RestClient/HttpClient are explicitly designed to be reused across concurrent calls). Fixture-specific services (`EmployeesService`, `SimulateService`) stay transient, so each fixture still gets its own instance.
* **Advantage:** No isolation-related races; fixtures run in parallel safely without every fixture paying the cost of rebuilding config/logging/HTTP client setup.
* **Disadvantage:** Because everything lives in one process-wide container, there's no built-in way to scope a resource (like a rate limiter) to a subset of fixtures — any cross-cutting concern that needs coordinating across all parallel work has to be a shared singleton itself.
* **Best Use Case:** Parallel execution against a target that doesn't need active request throttling from the client side.

### 3. Shared Provider + Rate Limiting (`functional/add_rate_limiter`)
Builds on the same shared-`ServiceProvider`-in-`SetupFixture` foundation as branch 2, and adds a **TokenBucket** rate limiter and resilience policies on top.
* **Architecture:** The single, shared `ServiceProvider` doubles as the natural place to host a process-wide `TokenBucket` singleton, so it can throttle requests across every parallel fixture.
* **Advantage:** Enables global resource management — the `TokenBucket` effectively throttles requests across all threads to avoid `HTTP 429` errors.
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

