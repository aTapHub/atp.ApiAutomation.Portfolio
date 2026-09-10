# ApiAutomation.Portfolio

A C#/.NET API test automation framework built with NUnit, RestSharp, and Microsoft.Extensions.DependencyInjection — designed to showcase a parallel-safe test architecture with a shared DI composition root, client-side rate limiting, and interface-based service abstractions.

---

## Architecture

### Parallel execution, safely

Fixtures run concurrently on separate worker threads (`[assembly: LevelOfParallelism(4)]`), using `[Parallelizable(ParallelScope.Fixtures)]`: different fixtures run at the same time, but the tests *inside* a given fixture run sequentially. That split matters because NUnit reuses a single instance of a fixture class across all of its test methods by default — method-level parallelism (`ParallelScope.All`) would race on that instance's shared fields (the Extent `Test` handle, resolved services), corrupting the HTML report or throwing intermittently. Fixture-level parallelism sidesteps that without needing a per-test-case instance lifecycle.

### One shared DI container

`SetupFixture` (`[SetUpFixture]`) is the single composition root, built once in `[OneTimeSetUp]` before any fixture runs — not per fixture. It registers:
- `IConfiguration` / `ApiSettings` / logging — read-only after binding, safe to share as singletons.
- `RestClient` — a **singleton**. RestSharp/`HttpClient` are explicitly designed to be reused across concurrent requests (creating a new one per fixture is the antipattern), so one shared client is both simpler and more correct.
- `RateLimiter` — a **singleton**, so every fixture running in parallel throttles against the same shared gate (see below).
- `IEmployeesService` / `ISimulateService` — **transient**, resolved fresh per fixture from `SetupFixture.ServiceProvider`, so each fixture gets its own instance while still sharing the underlying singletons.

`BaseTest` no longer builds anything itself — it just exposes `SetupFixture.ServiceProvider` for derived fixtures to pull services from in their own `[OneTimeSetUp]`, and disposal happens once, for the whole run, in `SetupFixture`'s `[OneTimeTearDown]`.

### Client-side rate limiting

`Utils/RateLimiter.cs` is a token-bucket limiter built on `SemaphoreSlim`: a fixed number of permits, refilled back up to capacity on a timer. `BaseService.ExecuteAsync(...)` is the single place every request goes through — it awaits a permit from the limiter before calling `RestClient.ExecuteAsync`, so every concrete service gets throttling for free without having to know about it. Because the limiter is registered as a singleton in the shared container, it throttles across *all* fixtures running in parallel, not just within one.

### Interface-based services

`EmployeesService` and `SimulateService` each implement an interface (`IEmployeesService`, `ISimulateService`) that mirrors their public surface. Tests depend on the interface, resolved from the DI container — not the concrete class — so a test's dependency is the contract it actually needs, and a fake/mock implementation could be substituted without touching the test.

### Resilient by construction, not by convention

Nothing here relies on tests being run in a particular order or on a particular thread. Config, logging, the HTTP client, and the rate limiter are all set up exactly once per suite run and shared safely; only what genuinely needs to vary per fixture (the concrete services) is created per fixture.

---

## Project Structure

```text
atp.ApiAutomation.Framework/
├── Configurations/  # Mapping appsettings.json to C# POCOs (ApiSettings)
├── Data/            # Test data generation
├── Models/          # DTOs (Data Transfer Objects) for API Payloads
├── Services/        # BaseService (rate-limited request execution) + Employees/Simulate services & their interfaces
├── Tests/           # NUnit Test Suites, BaseTest, SetupFixture wiring
└── Utils/           # RateLimiter (SemaphoreSlim-based token bucket)
```

---

Here is the core schematic on how the Test and Api Services layers interact on sequential mode - main branch:
Inside TestClass constructor the api service needed for that TestFixture class is added into the service collection.
It is then retreived in the same TestClass but in the [OneTimeSetup] method. The .net service collection manges the ApiService lifetime.

<img width="691" height="761" alt="Api Framework drawio" src="https://github.com/user-attachments/assets/6e33412e-f464-4a1d-ab78-93bd372515c6" />



Below shows how the dependency injection mechanism works for the implementation that uses parallelization. In this case, all the services are added to the ServiceCollection inside the SetupFixture, that runs once per suite run.
TestBase, TestClasses, BaseService, ApiServices run as maby times as there are TestFixtures.

<img width="1376" height="1222" alt="image" src="https://github.com/user-attachments/assets/e458b9f7-1dc4-4a17-809d-10c7c6706f9c" />
